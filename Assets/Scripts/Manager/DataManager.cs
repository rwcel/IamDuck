using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UniRx;
using UniRx.Triggers;
using System.Linq;
using System.Security.Cryptography;
using System.Text;


public struct FGoods
{
    public int coin;
    public int cashDia;
    public int freeDia;

    public FGoods(int coin, int cashDia, int freeDia)
    {
        this.coin = coin;
        this.cashDia = cashDia;
        this.freeDia = freeDia;
    }
}

[System.Serializable]
public struct FDailyCheck
{
    public int day;
    public int id;
    public int value;
}

// 인게임 기록 데이터
public class RecordData
{
    public int coin = 0;
    public int height = 0;
    public int[] items;
}


/// <summary>
/// 데이터 로드
/// </summary>
public partial class DataManager : Singleton<DataManager>
{
    #region reactive

    private readonly ReactiveProperty<int> heightRP = new IntReactiveProperty(0);
    private readonly ReactiveProperty<int> accuHeightRP = new IntReactiveProperty(0);
    private readonly ReactiveProperty<int> coinRP = new IntReactiveProperty(0);
    private readonly ReactiveProperty<int> cashDiaRP = new IntReactiveProperty(0);
    private readonly ReactiveProperty<int> freeDiaRP = new IntReactiveProperty(0);
    private readonly ReactiveProperty<int> diaRP = new IntReactiveProperty(0);
    private readonly ReactiveProperty<int> missionRewardRP = new IntReactiveProperty(0);
    private readonly ReactiveProperty<bool> missionNotifyRP = new BoolReactiveProperty(false);
    private readonly ReactiveProperty<bool> manageNotifyRP = new BoolReactiveProperty(false);
    private readonly ReactiveProperty<int> profileRP = new IntReactiveProperty(0);
    private readonly ReactiveProperty<int> attendanceRP = new IntReactiveProperty(0);
    private readonly ReactiveCollection<int> upgradeRC = new ReactiveCollection<int>();
    private readonly ReactiveCollection<int> useItemRC = new ReactiveCollection<int>();
    private readonly ReactiveCollection<int> itemAdCountRC = new ReactiveCollection<int>();
    private readonly ReactiveCollection<bool> characterRC = new ReactiveCollection<bool>();
    //private readonly ReactiveDictionary<EUpgradeType, int> upgradeRD = new ReactiveDictionary<EUpgradeType, int>();

    public ReadOnlyReactiveProperty<int> Height => heightRP.ToReadOnlyReactiveProperty();
    public ReadOnlyReactiveProperty<int> AccuHeight => accuHeightRP.ToReadOnlyReactiveProperty();
    public ReadOnlyReactiveProperty<int> Coin => coinRP.ToReadOnlyReactiveProperty();
    public ReadOnlyReactiveProperty<int> CashDia => cashDiaRP.ToReadOnlyReactiveProperty();
    public ReadOnlyReactiveProperty<int> FreeDia => freeDiaRP.ToReadOnlyReactiveProperty();
    public ReadOnlyReactiveProperty<int> Dia => diaRP.ToReadOnlyReactiveProperty();
    public ReadOnlyReactiveProperty<int> MissionReward => missionRewardRP.ToReadOnlyReactiveProperty();
    public ReadOnlyReactiveProperty<bool> IsMissionNotify => missionNotifyRP.ToReadOnlyReactiveProperty();
    public ReadOnlyReactiveProperty<bool> IsManageNotify => manageNotifyRP.ToReadOnlyReactiveProperty();
    public ReadOnlyReactiveProperty<int> Profile => profileRP.ToReadOnlyReactiveProperty();
    public ReadOnlyReactiveProperty<int> TodayAttendance => attendanceRP.ToReadOnlyReactiveProperty();
    public System.IObservable<CollectionReplaceEvent<int>> UpgradeObservable => upgradeRC.ObserveReplace();
    public System.IObservable<CollectionReplaceEvent<int>> UseItemObservable => useItemRC.ObserveReplace();
    public System.IObservable<CollectionReplaceEvent<int>> ItemAdCountObservable => itemAdCountRC.ObserveReplace();
    public System.IObservable<CollectionReplaceEvent<bool>> CharacterObservable => characterRC.ObserveReplace();
    //public System.IObservable<DictionaryReplaceEvent<EUpgradeType, int>> UpgradeObservable2 => upgradeRD.ObserveReplace();

    public List<int> UpgradeLevels => upgradeRC.ToList();
    public List<int> UseItems => useItemRC.ToList();
    public List<int> ItemAdCounts => itemAdCountRC.ToList();
    public List<int> IngameItems;
    public List<bool> UnlockCharacters => characterRC.ToList();

    //public Dictionary<EUpgradeType, int> UpgradeLevels2 => upgradeRD.ToDictionary(pair => pair.Key, pair => pair.Value);

    //public int GetUpgradeCost(UpgradeData data, EUpgradeType type)
    //{
    //    return data.GetCost(upgradeRD[type]);
    //}

    //public Dictionary<EUseItemType, bool> selectedUseItems;
    //public Dictionary<EUseItemType, bool> SelectedUseItems => selectedUseItems;
    private List<bool> selectedUseItems;
    public List<bool> SelectedUseItems => selectedUseItems;

    #endregion reactive

    private string lastLogin = "";
    public bool IsLogin;

    private bool isRecvMissionReward;

    // 데일리 미션
    private MissionData[] missionDatas;
    public MissionData[] MissionDatas => missionDatas;
    private MissionRewardData[] missionRewardDatas;
    public MissionRewardData[] MissionRewardDatas => missionRewardDatas;


    [SerializeField] BackendManager _Server;
    LevelData _LevelData;

    private FDailyCheck[] dailyChecks;
    public FDailyCheck[] DailyChecks => dailyChecks;

    #region Achievement

    //public System.Action<EGameplay, int> OnMission_GamePlay;
    //public System.Action<EGameItem, int> OnMission_AddItem;
    //public System.Action<EUseItemType, int> OnMission_UseItem;
    //public System.Action<EIngameItem, int> OnMission_AddInGameItem;

    public System.Action<int, int> OnMission_GamePlay;
    public System.Action<int, int> OnMission_AddItem;
    public System.Action<int, int> OnMission_UseItem;
    public System.Action<int, int> OnMission_AddInGameItem;

    #endregion


    protected override void AwakeInstance()
    {
        IsLogin = false;
        _LevelData = LevelData.Instance;
    }

    protected override void DestroyInstance()
    {
        Debug.Log("Destroy DataManager");

        OnMission_GamePlay = null;
        OnMission_AddItem = null;
        OnMission_UseItem = null;
        OnMission_AddInGameItem = null;

        ClearAd();
    }

    public void AddItem(FGetItem item)
    {
        AddItem(item.type, item.count);
    }

    public void AddItem(EGameItem type, int count)
    {
        Debug.Log($"Add Item : {type} - {count}");
        switch (type)
        {
            case EGameItem.Coin:
                coinRP.Value += count;
                OnMission_AddItem?.Invoke((int)EGameItem.Coin, count);
                Debug.Log($"코인 차이 : {coinRP.Value}");
                break;
            case EGameItem.CashDia:
                cashDiaRP.Value += count;
                break;
            case EGameItem.FreeDia:
                freeDiaRP.Value += count;
                break;
            case EGameItem.StartBoost1:
            case EGameItem.StartBoost2:
            case EGameItem.BonusCoin:
            case EGameItem.Continue:
            case EGameItem.IgnoreBomb:
            case EGameItem.ObstacleAppear:
                useItemRC[type - EGameItem.StartBoost1] += count;
                break;
            case EGameItem.ItemAppear:
            case EGameItem.PowerPotion:
            case EGameItem.Spring:
            case EGameItem.Magnet:
            case EGameItem.Resist:
                upgradeRC[type - EGameItem.ItemAppear] += count;
                break;
            case EGameItem.DailyCheckChar:
                // 캐릭터 보유했으면 fish 10
                freeDiaRP.Value += count;
                break;
        }
    }

    /// <summary>
    /// **게임 끝나고 처리 필요
    /// </summary>
    public void UpdateMission(int idx, int value)
    {
        Debug.Log($"DM UpdateMission : {idx} - {value}");
        var data = missionDatas[idx];
        bool isClear = false;
        if (data.isBar)
        {
            data.currentValue += value;
            _Server.UpdateMission(idx, data.currentValue);
            if (data.currentValue >= data.missionValue)
            {
                isClear = true;
            }
        }
        else
        {
            if (value >= data.missionValue)
            {
                isClear = true;
            }
        }

        if (isClear)
        {
            data.currentValue = data.missionValue;
            // Action에서 제외
            data.OnAchieveRecv?.Invoke();

            missionNotifyRP.Value = true;
        }
    }

    public void RecvMission(int idx, int value)
    {
        Debug.Log($"Recv UpdateMission : {idx} - {value}");
        missionDatas[idx].currentValue = value;
        _Server.UpdateMission(idx, value);
    }

    public void UnlockCharacter(int idx) => characterRC[idx] = true;

    public void InitializeGoods(FGoods goods)
    {
        coinRP.Value = goods.coin;
        cashDiaRP.Value = goods.cashDia;
        freeDiaRP.Value = goods.freeDia;
        UpdateDia();

        coinRP.Skip(System.TimeSpan.Zero)
            .Subscribe(value =>
            {
                //OnMission_AddItem?.Invoke((int)EGameItem.Gold, value);
                _Server.UpdateGoods(EGoods.Coin, value);
                CanUpgrade(value);
            });
        cashDiaRP.Skip(System.TimeSpan.Zero)
            .Subscribe(value =>
            {
                _Server.UpdateGoods(EGoods.CashDia, value);
                UpdateDia();
            });
        freeDiaRP.Skip(System.TimeSpan.Zero)
            .Subscribe(value =>
            {
                _Server.UpdateGoods(EGoods.FreeDia, value);
                UpdateDia();
            });

        CanUpgrade(goods.coin);
    }

    private void UpdateDia()
    {
        diaRP.Value = cashDiaRP.Value + freeDiaRP.Value;
    }

    public void InitializeHeight(int bestValue, int accuValue)
    {
        heightRP.Value = bestValue;
        accuHeightRP.Value = accuValue;

        heightRP.Skip(System.TimeSpan.Zero)
            .Subscribe(value =>
            {
                _Server.UpdateRank(value);
            });

        accuHeightRP.Skip(System.TimeSpan.Zero)
            .Subscribe(value =>
            {
                Debug.Log($"accuHeight : {value}");
                _Server.AddHeight(value);
            });
    }

    public void InitializeProfileIcon(int value)
    {
        profileRP.Value = value;

        profileRP.Skip(System.TimeSpan.Zero)
            .Subscribe(value =>
            {
                _Server.UpdateProfile(value);
            });
    }

    public void InitializeCharacters(List<bool> items)
    {
        characterRC.Clear();

        foreach (var item in items)
        {
            characterRC.Add(item);
        }

        characterRC.ObserveReplace()
            .Skip(System.TimeSpan.Zero)
            .Subscribe(x =>
            {
                    Debug.Log($"Change [{(ECharacter)x.Index}] : [{x.OldValue}] => [{x.NewValue}]");
                    _Server.UnlockCharacter((ECharacter)x.Index, x.NewValue);
            });
    }

    public void InitializeUseItems(List<int> items)
    {
        selectedUseItems = new List<bool>(items.Count);

        useItemRC.Clear();

        foreach (var item in items)
        {
            useItemRC.Add(item);
            selectedUseItems.Add(false);
        }

        useItemRC.ObserveReplace()
            .Skip(System.TimeSpan.Zero)
            .Subscribe(x =>
            {
                //saveJsonData.useItmes[x.Index] = x.NewValue;
                Debug.Log($"Change [{x.Index}] : [{x.OldValue}] => [{x.NewValue}]");
                //_Server.UpdateUseItem(UseItems);
                _Server.UpdateUseItem(x.Index, x.NewValue);
            });

        //selectedUseItems = new Dictionary<EUseItemType, bool>();
        //foreach (EUseItemType useItem in System.Enum.GetValues(typeof(EUseItemType)))
        //{
        //    selectedUseItems.Add(useItem, false);
        //}
    }

    public void InitializeUpgrades(List<int> items)
    {
        upgradeRC.Clear();

        foreach (var item in items)
        {
            upgradeRC.Add(item);
        }

        upgradeRC.ObserveReplace()
            .Skip(System.TimeSpan.Zero)
            .Subscribe(x =>
            {
                //saveJsonData.upgrades[x.Index] = x.NewValue;
                Debug.Log($"Change [{x.Index}] : [{x.OldValue}] => [{x.NewValue}]");
                _Server.UpdateUpgrade(x.Index, x.NewValue);

                CanUpgrade(coinRP.Value);
                //_Server.UpdateUpgrade(UpgradeLevels);
            });

        CanUpgrade(coinRP.Value);
    }
    
    public void InitializeAttendance(FDailyCheck[] dailyChecks)
    {
        this.dailyChecks = dailyChecks;
    }

    public void InitializeMissionProgress(List<int> progresses, int reward)
    {
        for (int i = 0, length = progresses.Count; i < length; i++)
        {
            missionDatas[i].currentValue = progresses[i];
        }

        // _Server.UpdateMission(x.Index, x.NewValue);

        missionRewardRP.Value = reward;

        missionRewardRP.Skip(System.TimeSpan.Zero)
            .Subscribe(value =>
            {
                _Server.UpdateMissionReward(value);
            });

        isRecvMissionReward = true;

        Debug.Log("Mission Progress");
        AddOnMissions();
    }

    public void InitializeDailyMission(MissionData[] items)
    {
        missionDatas = items;
        GameSceneManager.Instance.RestartAction += RemoveOnMissions;

        Debug.Log("DailyMission");
        AddOnMissions();
    }

    private void AddOnMissions()
    {
        // 둘다 들어왔을 때 실행
        if (missionDatas == null || !isRecvMissionReward)
            return;

        Debug.Log($"MissionData Length : {missionDatas.Length}");
        for (int i = 0, length = missionDatas.Length; i < length; i++)
        {
            int idx = i;
            var data = missionDatas[i];
            if (missionDatas[i].currentValue == Values.Mission_Rewarded)
            {
                continue;
            }
            else if(missionDatas[i].currentValue >= missionDatas[i].missionValue)
            {
                missionNotifyRP.Value = true;
                continue;
            }

            Debug.Log($"Mission {idx} - UpdateMission : {data.category}");
            data.SetData(UpdateMission);
            switch (data.category)
            {
                case EMissionCategory.Gameplay:
                    OnMission_GamePlay += data.OnAchieve;
                    data.OnAchieveRecv = () => OnMission_GamePlay -= data.OnAchieve;
                    break;
                case EMissionCategory.AddInGameItem:
                    OnMission_AddInGameItem += data.OnAchieve;
                    data.OnAchieveRecv = () => OnMission_AddInGameItem -= data.OnAchieve;
                    break;
                case EMissionCategory.UseItem:
                    OnMission_UseItem += data.OnAchieve;
                    data.OnAchieveRecv = () => OnMission_UseItem -= data.OnAchieve;
                    break;
                case EMissionCategory.AddItem:
                    OnMission_AddItem += data.OnAchieve;
                    data.OnAchieveRecv = () => OnMission_AddItem -= data.OnAchieve;
                    break;
            }
        }
    }

    private void RemoveOnMissions()
    {
        for (int i = 0, length = missionDatas.Length; i < length; i++)
        {
            int idx = i;
            var data = missionDatas[i];
            if (missionDatas[i].currentValue == Values.Mission_Rewarded)
            {
                continue;
            }
            else if (missionDatas[i].currentValue >= missionDatas[i].missionValue)
            {
                continue;
            }

            data.SetData(UpdateMission);
            switch (data.category)
            {
                case EMissionCategory.Gameplay:
                    OnMission_GamePlay -= data.OnAchieve;
                    break;
                case EMissionCategory.AddInGameItem:
                    OnMission_AddInGameItem -= data.OnAchieve;
                    break;
                case EMissionCategory.UseItem:
                    OnMission_UseItem -= data.OnAchieve;
                    break;
                case EMissionCategory.AddItem:
                    OnMission_AddItem -= data.OnAchieve;
                    break;
            }
        }

        missionNotifyRP.Value = false;
        isRecvMissionReward = false;
        missionDatas = null;
    }

    public void InitializeMissionReward(MissionRewardData[] items)
    {
        missionRewardDatas = items;
    }
    
    public void InitializeInGameItems(List<int> items)
    {
        IngameItems = items;

        //foreach (var item in items)
        //{
        //    ingameItemRC.Add(item);
        //}

        //ingameItemRC.ObserveReplace()
        //    .Skip(System.TimeSpan.Zero)
        //    .Subscribe(x =>
        //    {
        //        if (x.OldValue != x.NewValue)
        //        {
        //            Debug.Log($"Change [{x.Index}] : [{x.OldValue}] => [{x.NewValue}]");
        //            // _Server.UpdateInGameItem(x.Index, x.NewValue);           // **한번에 보내기
        //        }
        //    });
    }

    /// <summary>
    /// 현재 해당 번호만 받게
    /// </summary>
    /// <returns></returns>
    public bool RewardMission(MissionRewardData data)
    {
        if (missionRewardRP.Value != data.num)
            return false;

        AddItem((EGameItem)data.id, data.value);
        missionRewardRP.Value++;

        return true;
    }

    public void InitializeInAttendance(string date, int count)
    {
        lastLogin = date;
        attendanceRP.Value = count;

        // (lastLogin, attendanceCount) = _Server.GetDailyLogin(lastLogin, attendanceCount);
    }

    /// <summary>
    /// 오늘 최초접속인지 파악
    /// true면 출석체크 UI 표시
    /// 확인안해도 여기서 자동 수령
    /// </summary>
    /// <returns></returns>
    public bool DailyLogin()
    {
        string today = System.DateTime.Now.ToString("yyyy-MM-dd");
        Debug.Log($"Day{attendanceRP.Value + 1} : {lastLogin} != {today}");

        return lastLogin != today;
    }

    public void RecvDailyLogin()
    {
        //var reward = _Server.GetDailyChecks(attendanceCount);
        var reward = dailyChecks[attendanceRP.Value % Values.Length_Attendance];

        if (++attendanceRP.Value != Values.Length_Attendance)
        {
            AddItem((EGameItem)reward.id, reward.value);
        }
        else
        {
            Debug.Log($"캐릭터 받는날 : {attendanceRP.Value}");
        }

        _Server.UpdateDailyLogin(attendanceRP.Value);
    }

    /// <summary>
    /// 데이터 늘어날수록 처리 어렵기 때문에 호출방식으로 변경
    /// </summary>
    public void SaveRecord(RecordData recordData)
    {
        Debug.Log($"save Record : {recordData.height}");

        if (heightRP.Value < recordData.height)
            //&& !GameApplication.Instance.IsTestMode)
        {
            heightRP.Value = recordData.height;
            // 최고기록 갱신 -> 서버 랭킹에 저장
        }
        accuHeightRP.Value += recordData.height;
        coinRP.Value += recordData.coin;

        OnMission_GamePlay?.Invoke((int)EGameplay.GamePlay, 1);
        OnMission_GamePlay?.Invoke((int)EGameplay.ResultHeight, recordData.height);
        OnMission_GamePlay?.Invoke((int)EGameplay.AccuHeight, recordData.height);
        OnMission_AddItem?.Invoke((int)EGameItem.Coin, recordData.coin);

        for (int i = 0, length = recordData.items.Length; i < length; i++)
        {
            OnMission_AddInGameItem?.Invoke(i, recordData.items[i]);
            //ingameItemRC[i] += recordData.items[i];
            IngameItems[i] += recordData.items[i];
        }
        _Server.UpdateInGameItem(IngameItems);
    }

    public bool UseGoods(EGoods type, int value)
    {
        switch (type)
        {
            case EGoods.Coin:
                if (coinRP.Value >= value)
                {
                    coinRP.Value -= value;
                    return true;
                }
                break;
            case EGoods.CashDia:
                if (cashDiaRP.Value >= value)
                {
                    cashDiaRP.Value -= value;
                    return true;
                }
                break;
            case EGoods.FreeDia:
                if (freeDiaRP.Value >= value)
                {
                    freeDiaRP.Value -= value;
                    return true;
                }
                break;
            case EGoods.Dia:
                // 무료재화 먼저 사용
                if (UseGoods(EGoods.FreeDia, value))
                    return true;
                else if (UseGoods(EGoods.CashDia, value))
                    return true;
                break;
        }

        return false;
    }

    public bool BuyUpgrade(UpgradeData data, int coin)
    {
        // 최대 강화
        Debug.Log($"{upgradeRC[(int)data.type]} >= {data.maxLevel}");
        if (upgradeRC[(int)data.type] >= data.maxLevel)
            return false;

        if (coinRP.Value >= coin)
        {
            coinRP.Value -= coin;

            upgradeRC[(int)data.type]++;         // 데이터 갱신 : 코인 이후에 실행

            return true;
        }

        return false;
    }

    public bool BuyUseItem(EUseItemType type, int coin)
    {
        if (coinRP.Value >= coin)
        {
            useItemRC[(int)type]++;

            coinRP.Value -= coin;

            return true;
        }

        return false;
    }

    public void UpdateProfile(int value)
    {
        profileRP.Value = value;
    }

    private void CanUpgrade(int coinValue)
    {
        //if (coinValue == 0 || UpgradeLevels == null)
        if(UpgradeLevels == null)
            return;

        bool flag = false;
        int cost = 0;
        for (int i = 0, length = upgradeRC.Count; i < length; i++)
        {
            cost = _LevelData.UpgradeDatas[i].GetCost(upgradeRC[i]);
            if (cost > 0 && 
                coinValue >= cost )
            {
                Debug.Log($"{coinValue} >= {_LevelData.UpgradeDatas[i].GetCost(upgradeRC[i])}");
                flag = true;
                break;
            }
        }

        SetNotify(EHomeNotify.Manage, flag);
    }

    public void SetNotify(EHomeNotify type, bool value)
    {
        switch (type)
        {
            case EHomeNotify.Mission:
                missionNotifyRP.Value = value;
                break;
            case EHomeNotify.Manage:
                manageNotifyRP.Value = value;
                break;
        }
    }

    public void SetUseItemTypeValue(EUseItemType type, bool value)
    {
        // Debug.Log(type + "," + value);
        selectedUseItems[(int)type] = value;
    }

    public float GetUseItemTypeValue(EUseItemType type)
    {
        int typeNum = (int)type;

        return selectedUseItems[typeNum] ? UseItemType(type) : 0;
    }

    /// <summary>
    /// 갖고 있으면 자동사용 : 이어하기
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public float UseItemType(EUseItemType type)
    {
        int typeNum = (int)type;

        useItemRC[typeNum]--;
        // selectedUseItems[typeNum] = false;               // 씬이 종료될 때 실행하도록 변경
        OnMission_UseItem?.Invoke(typeNum, 1);

        //LevelData.Instance.

        // UseItemData => GameData
        return _LevelData.UseItemDatas[typeNum].value;
    }

    public void ResetUseItems()
    {
        for (int i = 0, length = selectedUseItems.Count; i < length; i++)
        {
            selectedUseItems[i] = false;
        }
    }

    public bool HaveUseItemType(EUseItemType type)
    {
        return useItemRC[(int)type] > 0;
    }
}