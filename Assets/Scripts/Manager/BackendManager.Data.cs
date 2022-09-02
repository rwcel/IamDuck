using UnityEngine;
using BackEnd;
using System.Collections;
using System.Collections.Generic;
using static BackEnd.SendQueue;
using LitJson;
using UniRx;
using System.Linq;

public struct FMailInfo
{
    public string title;
    public string contents;
    public EPostType type;
    public Sprite icon;
    public string inDate;
    public System.TimeSpan limit;

    public FGetItem[] getItems;
    public int itemValue => getItems[0].count;
    //public EGameItem itemID;
    //public int value;
}

public class RankInfo
{
    public int iconNum;
    public string nickname;
    public int rank;
    public int score;
    public string inDate;

    public RankInfo()
    {
    }

    public RankInfo(string nickname, int iconNum, int rank, int score, string inDate)
    {
        this.nickname = nickname;
        this.iconNum = iconNum;
        this.rank = rank;
        this.score = score;
        this.inDate = inDate;
    }
}

public partial class BackendManager
{
    [HideInInspector]
    public System.Action AllLoadAction;

    private DataManager _DataManager;
    private int loadCount = 0;
    private string lastLoginDate;
    private bool isReset;

    public string Owner_InDate => Backend.UserInDate;

    private static readonly string GoodsTableName = "Goods";
    private static readonly string CoinColName = "coin", CashDiaColName = "cashDia", FreeDiaColName = "freeDia";
    // private static readonly string PaymentColName = "isPayment";        // 첫결제      //**위치 미정

    private static readonly string RankTableName = "Rank";
    private static readonly string HeightColName = "height", IconColName = "icon";
    private static readonly string AccuHeightColName = "accuHeight";

    private static readonly string LoginTableName = "Login";
    private static readonly string LastloginColName = "lastLogin", AttendColName = "attendCount";

    private static readonly string UseItemTableName = "UseItem";
    private static readonly string UpgradeTableName = "Upgrade";

    private static readonly string MissionTableName = "Mission";
    private static readonly string MissionProgressColName = "mission", MissionRewardColName = "reward";

    private static readonly string CharacterTableName = "Character";

    private static readonly string InGameItemTableName = "InGameItem";

    private static readonly string ShopTableName = "Shop";
    private static readonly string BuyWeeklyColName = "weekly", BuyDoubleCoinColName = "doubleCoin";
    private static readonly string RemainRemoveAdColName = "removeAd", IsBoughtColName = "isBuy";              // 첫결제 확인

    private static readonly string AdsTableName = "Ad";

    // private static readonly string MailTableName = "Mail";


    private static readonly string rankUUID = "1ed9b8d0-f914-11ec-907f-535b8c4c9106";

    private string userIndate_Rank;                // 유저 데이터


    #region Reactive Properties

    //private readonly ReactiveProperty<int> attendanceRP = new IntReactiveProperty(0);
    //public ReadOnlyReactiveProperty<int> Attendance => attendanceRP.ToReadOnlyReactiveProperty();

    private readonly ReactiveCollection<FMailInfo> mailRC = new ReactiveCollection<FMailInfo>();
    public System.IObservable<int> MailCount => mailRC.ObserveCountChanged();
    public List<FMailInfo> MailList => mailRC.ToList();


    private readonly ReactiveProperty<System.DateTime> buyWeeklyRP = new ReactiveProperty<System.DateTime>();
    private readonly ReactiveProperty<System.DateTime> adRemoveRP = new ReactiveProperty<System.DateTime>();
    private readonly ReactiveProperty<bool> buyDoubleCoinRP = new BoolReactiveProperty(false);
    private readonly ReactiveProperty<int> paymentRP = new IntReactiveProperty(0);


    public ReadOnlyReactiveProperty<System.DateTime> BuyWeeklyTime => buyWeeklyRP.ToReadOnlyReactiveProperty();
    public ReadOnlyReactiveProperty<System.DateTime> RemainAdRemove => adRemoveRP.ToReadOnlyReactiveProperty();
    public ReadOnlyReactiveProperty<bool> IsBuyDoubleCoin => buyDoubleCoinRP.ToReadOnlyReactiveProperty();
    public ReadOnlyReactiveProperty<int> PaymentPrice => paymentRP.ToReadOnlyReactiveProperty();

    public bool IsBuyWeekly => BuyWeeklyTime.Value > System.DateTime.Now;
    public bool IsAdRemove => adRemoveRP.Value > System.DateTime.Now;

    #endregion Reactive Properties


    public void GetGameDatas()
    {
        _DataManager = DataManager.Instance;

        GetLoginData();
        GetGoodsData();
        GetRankData();
        GetUseItemData();
        GetUpgradeData();
        GetMissionData();
        GetCharacterData();
        GetInGameItemData();
        GetShopData();
        GetAdsData();

        GetMails();

        StartCoroutine(nameof(CoWaitDataAllLoad));
    }

    private void ResetDatas()
    {
        isReset = true;
        ResetMissionData();
        ResetAdsData();
        // ResetWeeklyData();
    }

    IEnumerator CoWaitDataAllLoad()
    {
        while (loadCount > 0)
        {
            yield return null;
        }

        //Debug.Log("All Load");
        AllLoadAction?.Invoke();
    }

    #region 재화

    private void GetGoodsData()
    {
        loadCount++;

        Backend.GameData.GetMyData(GoodsTableName, new Where(), bro =>
        {
            if (FailError(bro))
                return;

            if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
            {
                InsertGoods();
                return;
            }

            var rows = bro.FlattenRows();

            FGoods goods;
            goods.coin = int.Parse(rows[0][CoinColName].ToString());
            goods.cashDia = int.Parse(rows[0][CashDiaColName].ToString());
            goods.freeDia = int.Parse(rows[0][FreeDiaColName].ToString());

            _DataManager.InitializeGoods(goods);

            loadCount--;
        });
    }

    private void InsertGoods()
    {
        Param param = new Param();
        param.Add(CoinColName, 0);
        param.Add(CashDiaColName, 0);
        param.Add(FreeDiaColName, 0);

        Backend.GameData.Insert(GoodsTableName, param, bro =>
        {
            if (FailError(bro))
                return;

            _DataManager.InitializeGoods(new FGoods(0, 0, 0));
            loadCount--;
        });
    }

    public void UpdateGoods(FGoods goods)
    {
        Param param = new Param();
        param.Add(CoinColName, goods.coin);
        param.Add(CashDiaColName, goods.cashDia);
        param.Add(FreeDiaColName, goods.freeDia);

        var bro = Backend.GameData.Update(GoodsTableName, new Where(), param);
        if (FailError(bro))
            return;

        Debug.Log("Update Goods");
    }

    public void UpdateGoods(EGoods type, int value)
    {
        Param param = new Param();
        string colName = "";
        switch (type)
        {
            case EGoods.Coin:
                colName = CoinColName;
                break;
            case EGoods.CashDia:
                colName = CashDiaColName;
                break;
            case EGoods.FreeDia:
                colName = FreeDiaColName;
                break;
        }

        param.Add(colName, value);

        //var bro = Backend.GameData.Update(GoodsTableName, new Where(), param);
        //if (FailError(bro))
        //    return;

        Backend.GameData.Update(GoodsTableName, new Where(), param, bro =>
        {
            if (FailError(bro))
                return;

            Debug.Log($"Update Goods : {type} - {value}");
        });
    }

    #endregion 재화

    #region 사용 아이템

    private void GetUseItemData()
    {
        loadCount++;

        Backend.GameData.GetMyData(UseItemTableName, new Where(), bro =>
        {
            if (FailError(bro))
                return;

            if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
            {
                InsertUseItem();
                return;
            }

            var rows = bro.FlattenRows();

            var type = System.Enum.GetValues(typeof(EUseItemType));
            List<int> useItems = new(type.Length);

            foreach (EUseItemType item in type)
            {
                useItems.Add(int.Parse(rows[0][item.ToString()].ToString()));
            }

            _DataManager.InitializeUseItems(useItems);

            loadCount--;
        });
    }

    private void InsertUseItem()
    {
        int length = System.Enum.GetValues(typeof(EUseItemType)).Length;
        List<int> useItems = new List<int>(length);
        Param param = new Param();

        for (int i = 0; i < length; i++)
        {
            param.Add(((EUseItemType)i).ToString(), 0);
            useItems.Add(0);
        }

        Backend.GameData.Insert(UseItemTableName, param, bro =>
        {
            if (FailError(bro))
                return;

            _DataManager.InitializeUseItems(useItems);

            loadCount--;
        });
    }

    //public void UpdateUseItem(List<int> datas)
    //{
    //    Param param = new Param();
    //    for (int i = 0, length = datas.Count; i < length; i++)
    //    {
    //        param.Add(((EUseItemType)i).ToString(), datas[i]);
    //    }

    //    var bro = Backend.GameData.Update(UseItemTableName, new Where(), param);
    //    if (FailError(bro))
    //        return;

    //    Debug.Log("Update UseItem");
    //}

    public void UpdateUseItem(int idx, int value)
    {
        Param param = new Param();
        param.Add(((EUseItemType)idx).ToString(), value);

        //var bro = Backend.GameData.Update(UseItemTableName, new Where(), param);
        //if (FailError(bro))
        //    return;

        Backend.GameData.Update(UseItemTableName, new Where(), param, bro =>
        {
            if (FailError(bro))
                return;

            Debug.Log("Update UseItem");
        });
    }

    #endregion 사용 아이템

    #region 업그레이드

    private void GetUpgradeData()
    {
        loadCount++;

        Backend.GameData.GetMyData(UpgradeTableName, new Where(), bro =>
        {
            if (FailError(bro))
                return;

            if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
            {
                InsertUpgrade();
                return;
            }

            var rows = bro.FlattenRows();

            var type = System.Enum.GetValues(typeof(EUpgradeType));
            List<int> upgrades = new(type.Length);

            foreach (EUpgradeType item in type)
            {
                upgrades.Add(int.Parse(rows[0][item.ToString()].ToString()));
            }

            _DataManager.InitializeUpgrades(upgrades);

            loadCount--;
        });
    }

    private void InsertUpgrade()
    {
        int length = System.Enum.GetValues(typeof(EUpgradeType)).Length;
        List<int> upgrades = new List<int>(length);
        Param param = new Param();

        for (int i = 0; i < length; i++)
        {
            param.Add(((EUpgradeType)i).ToString(), 0);
            upgrades.Add(0);
        }

        Backend.GameData.Insert(UpgradeTableName, param, bro =>
        {
            if (FailError(bro))
                return;

            _DataManager.InitializeUpgrades(upgrades);

            loadCount--;
        });
    }

    public void UpdateUpgrade(List<int> datas)
    {
        Param param = new Param();
        for (int i = 0, length = datas.Count; i < length; i++)
        {
            param.Add(((EUpgradeType)i).ToString(), datas[i]);
        }

        var bro = Backend.GameData.Update(UpgradeTableName, new Where(), param);
        if (FailError(bro))
            return;

        Debug.Log("Update Upgrade");
    }

    public void UpdateUpgrade(int idx, int value)
    {
        Param param = new Param();
        param.Add(((EUpgradeType)idx).ToString(), value);

        //var bro = Backend.GameData.Update(UpgradeTableName, new Where(), param);
        //if (FailError(bro))
        //    return;

        Backend.GameData.Update(UpgradeTableName, new Where(), param, bro =>
        {
            if (FailError(bro))
                return;

            Debug.Log("Update Upgrade");
        });
    }

    #endregion 업그레이드

    #region 캐릭터

    private void GetCharacterData()
    {
        loadCount++;

        Backend.GameData.GetMyData(CharacterTableName, new Where(), bro =>
        {
            if (FailError(bro))
                return;

            if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
            {
                InsertCharacter();
                return;
            }

            var rows = bro.FlattenRows();

            // GameData.Instance.CharacterDatas.Length == type
            var type = System.Enum.GetValues(typeof(ECharacter));

            if ((rows[0].Count - Values.Backend_Base) != type.Length)
            {
                ReInsertCharacter(rows[0]["inDate"].ToString());
                return;
            }

            List<bool> characters = new(type.Length);

            int idx = 0;
            foreach (ECharacter item in type)
            {
                characters.Add(bool.Parse(rows[0][string.Format("C{0:00}{1}", idx, item)].ToString()));
                idx++;
            }

            _DataManager.InitializeCharacters(characters);

            loadCount--;
        });
    }

    private void ReInsertCharacter(string inDate)
    {
        Debug.Log($"{inDate}");

        Backend.GameData.DeleteV2(CharacterTableName, inDate, Backend.UserInDate, (bro) =>
        {
            // 이후 처리
            Debug.Log("Delete Character");

            InsertCharacter();
        });
    }

    private void InsertCharacter()
    {
        int length = System.Enum.GetValues(typeof(ECharacter)).Length;
        List<bool> characters = new(length);
        Param param = new Param();

        for (int i = 0; i < length; i++)
        {
            param.Add(string.Format("C{0:00}{1}", i, (ECharacter)i), i == 0);
            characters.Add(i == 0);
        }

        Backend.GameData.Insert(CharacterTableName, param, bro =>
        {
            if (FailError(bro))
                return;

            _DataManager.InitializeCharacters(characters);
            loadCount--;
        });
    }

    /// <summary>
    /// Update = Unlock
    /// </summary>
    public void UnlockCharacter(ECharacter type, bool value = true)
    {
        Param param = new Param();

        param.Add(string.Format("C{0:00}{1}", (int)type, type), value);

        var bro = Backend.GameData.Update(CharacterTableName, new Where(), param);
        if (FailError(bro))
            return;

        Debug.Log($"Unlock Character : {type}");
    }

    #endregion 캐릭터

    #region 미션

    /// <summary>
    /// **일일 초기화
    /// </summary>
    private void GetMissionData()
    {
        if (isReset)
            return;

        loadCount++;

        Backend.GameData.GetMyData(MissionTableName, new Where(), bro =>
        {
            if (FailError(bro))
                return;

            if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
            {
                InsertMission();
                return;
            }

            var rows = bro.FlattenRows();

            List<int> progresses = new(_GameData.MissionDatas.Length);

            for (int i = 0, length = _GameData.MissionDatas.Length; i < length; i++)
            {
                progresses.Add(int.Parse(rows[0][MissionProgressColName + i].ToString()));
            }
            int reward = int.Parse(rows[0][MissionRewardColName].ToString());

            _DataManager.InitializeMissionProgress(progresses, reward);

            loadCount--;
        });
    }

    /// <summary>
    /// Reward - 보상 받은 개수 (int)
    /// Mission - 진행도[int], 보상 받은 여부[bool] => -1 ?
    /// </summary>
    private void InsertMission()
    {
        List<int> missionProgresses = new(_GameData.MissionDatas.Length);

        Param param = new Param();
        for (int i = 0, length = _GameData.MissionDatas.Length; i < length; i++)
        {
            param.Add(MissionProgressColName + i, 0);
            missionProgresses.Add(0);
        }
        param.Add(MissionRewardColName, 0);

        Backend.GameData.Insert(MissionTableName, param, bro =>
        {
            if (FailError(bro))
                return;

            _DataManager.InitializeMissionProgress(missionProgresses, 0);

            loadCount--;
        });
    }

    private void ResetMissionData()
    {
        loadCount++;

        List<int> progresses = new(_GameData.MissionDatas.Length);

        Param param = new Param();
        for (int i = 0, length = _GameData.MissionDatas.Length; i < length; i++)
        {
            param.Add(MissionProgressColName + i, 0);
            progresses.Add(0);
        }
        param.Add(MissionRewardColName, 0);

        Backend.GameData.Update(MissionTableName, new Where(), param, bro =>
        {
            if (FailError(bro))
                return;

            _DataManager.InitializeMissionProgress(progresses, 0);

            loadCount--;
        });
    }

    public void UpdateMission(int idx, int value)
    {
        Param param = new Param();
        param.Add(MissionProgressColName + idx, value);

        //var bro = Backend.GameData.Update(MissionTableName, new Where(), param);
        //if (FailError(bro))
        //    return;

        Backend.GameData.Update(MissionTableName, new Where(), param, bro =>
        {
            if (FailError(bro))
                return;

            Debug.Log($"Update Mission : {idx} - {value}");
        });
    }

    public void UpdateMissionReward(int value)
    {
        Param param = new Param();
        param.Add(MissionRewardColName, value);

        //var bro = Backend.GameData.Update(MissionTableName, new Where(), param);
        //if (FailError(bro))
        //    return;

        Backend.GameData.Update(MissionTableName, new Where(), param, bro =>
        {
            if (FailError(bro))
                return;

            Debug.Log("Update Mission Reward");
        });
    }

    #endregion 미션


    #region 랭킹

    public void GetRankData()
    {
        ++loadCount;

        // **where을 정해줘야하나?
        Backend.GameData.GetMyData(RankTableName, new Where(), bro =>
        {
            if (FailError(bro))
                return;

            if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
            {
                InsertRank();
                return;
            }

            userIndate_Rank = bro.GetInDate();
            var rows = bro.FlattenRows();

            _DataManager.InitializeHeight(int.Parse(rows[0][HeightColName].ToString())
                                                  , int.Parse(rows[0][AccuHeightColName].ToString()));
            _DataManager.InitializeProfileIcon(int.Parse(rows[0][IconColName].ToString()));

            --loadCount;
        });
    }

    private void InsertRank()
    {
        Param param = new Param();
        param.Add(HeightColName, 0);
        param.Add(IconColName, 0);
        param.Add(AccuHeightColName, 0);

        Backend.GameData.Insert(RankTableName, param, bro =>
        {
            if (FailError(bro))
                return;

            userIndate_Rank = bro.GetInDate();

            _DataManager.InitializeHeight(0, 0);
            _DataManager.InitializeProfileIcon(0);

            --loadCount;
        });
    }

    public void UpdateProfile(int icon)
    {
        Param param = new Param();
        param.Add(IconColName, icon);

        Backend.GameData.Update(RankTableName, new Where(), param, bro =>
        {
            if (FailError(bro))
                return;

            Debug.Log("Update ProfileIcon");
        });
    }

    public int GetMyRank()
    {
        var bro = Backend.URank.User.GetMyRank(rankUUID);
        if (FailError(bro))
            return -1;

        var rows = bro.FlattenRows();

        return int.Parse(rows[0]["rank"].ToString());
    }

    public List<RankInfo> GetRankLists(int limit, int offset)
    {
        var result = new List<RankInfo>(limit);

        var bro = Backend.URank.User.GetRankList(rankUUID, limit, offset);
        if (FailError(bro))
            return null;

        JsonData rows = bro.FlattenRows();

        foreach (JsonData row  in rows)
        {
            RankInfo rankInfo = new RankInfo();

            rankInfo.inDate = row["gamerInDate"].ToString();
            rankInfo.nickname = row["nickname"]?.ToString()?? Values.Key_Nickname_Null;
            rankInfo.score = int.Parse(row["score"].ToString());
            rankInfo.iconNum = int.Parse(row[IconColName].ToString());
            rankInfo.rank = int.Parse(row["rank"].ToString());

            result.Add(rankInfo);
        }

        return result;
    }

    /// <summary>
    /// 누적 층 수 더하기
    /// </summary>
    public void AddHeight(int value)
    {
        Param param = new Param();
        param.Add(AccuHeightColName, value);

        Backend.GameData.Update(RankTableName, new Where(), param, bro =>
        {
            if (FailError(bro))
                return;

            Debug.Log($"누적 높이 갱신 : {value}");
        });

        //param.AddCalculation(AccuHeightColName, GameInfoOperator.addition, value);

        //Backend.GameData.UpdateWithCalculationV2(RankTableName, userIndate_Rank, Backend.UserInDate, param, bro =>
        //{
        //    if (FailError(bro))
        //        return;

        //    Debug.Log($"누적 높이 갱신 : {value}");
        //});
    }

    public void UpdateRank(int height)
    {
        if (string.IsNullOrEmpty(userIndate_Rank))
        {   // *가능성 없음
            Backend.GameData.Get(RankTableName, new Where(), bro =>
            {
                if (FailError(bro))
                    return;

                var rows = bro.FlattenRows();
                userIndate_Rank = rows[0]["inDate"].ToString();

                UpdateResultToRank(height);
            });
        }
        else
        {
            UpdateResultToRank(height);
        }
    }

    private void UpdateResultToRank(int height)
    {
        if (height <= 0)
            return;

        Param param = new Param();
        param.Add(HeightColName, height);

        Backend.GameData.Update(RankTableName, new Where(), param, bro =>
        {
            if (FailError(bro))
                return;

            Enqueue(Backend.URank.User.UpdateUserScore, rankUUID, RankTableName, userIndate_Rank, param, bro =>
            {
                Debug.Log($"최고기록 갱신 : {height}");
            });
        });

        // 랭킹에 추가
        Debug.Log("Update Result");
    }

    #endregion 랭킹

    #region 출석체크

    private void GetLoginData()
    {
        var bro = Backend.GameData.GetMyData(LoginTableName, new Where());
        if (FailError(bro))
            return;

        if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
        {
            // 초기화
            InsertDailyLogin();
            return;
        }

        var rows = bro.FlattenRows();
        lastLoginDate = rows[0][LastloginColName].ToString();
        string today = System.DateTime.Now.ToString("yyyy-MM-dd");
        if (lastLoginDate != today)
        {
            // 일일 초기화 데이터
            ResetDatas();
        }

        _DataManager.InitializeInAttendance(rows[0][LastloginColName].ToString()
                                                    , int.Parse(rows[0][AttendColName].ToString()));
    }

    private void InsertDailyLogin()
    {
        Param param = new Param();
        param.Add(LastloginColName, "");
        param.Add(AttendColName, 0);

        var bro = Backend.GameData.Insert(LoginTableName, param);
        if (FailError(bro))
            return;

        // var rows = bro.FlattenRows();

        _DataManager.InitializeInAttendance("", 0);
    }

    /// <summary>
    /// 자동증가
    /// </summary>
    public void UpdateDailyLogin(int attendanceCount)
    {
        lastLoginDate = System.DateTime.Now.ToString("yyyy-MM-dd");

        Debug.Log(attendanceCount);

        Param param = new Param();
        param.Add(LastloginColName, lastLoginDate);
        param.Add(AttendColName, attendanceCount);


        var bro = Backend.GameData.Update(LoginTableName, new Where(), param);
        if (FailError(bro))
            return;

        _DataManager.InitializeInAttendance(lastLoginDate, attendanceCount);
        Debug.Log("Update dailyLogin");
    }

    public void RecvDailyLogin()
    {
        // 보상 보내주기
        //var reward = DailyChecks[saveJsonData.dailyCheck];
        //AddItem((EGameItem)reward.id, reward.value);

        //++saveJsonData.dailyCheck;
        //if (saveJsonData.dailyCheck > Values.Length_DailyCheck)
        //{
        //    saveJsonData.dailyCheck = 1;
        //}
        //saveJsonData.lastLogin = System.DateTime.Now.ToString("yyyy-MM-dd");

        //SaveData();
    }

    #endregion 출석체크


    #region 인게임 아이템

    private void GetInGameItemData()
    {
        loadCount++;

        Backend.GameData.GetMyData(InGameItemTableName, new Where(), bro =>
        {
            if (FailError(bro))
                return;

            if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
            {
                InsertInGameItem();
                return;
            }

            var rows = bro.FlattenRows();

            var type = System.Enum.GetValues(typeof(EIngameItem));
            List<int> ingameItems = new(type.Length);

            foreach (EIngameItem item in type)
            {
                ingameItems.Add(int.Parse(rows[0][item.ToString()].ToString()));
            }

            _DataManager.InitializeInGameItems(ingameItems);

            loadCount--;
        });
    }

    private void InsertInGameItem()
    {
        int length = System.Enum.GetValues(typeof(EIngameItem)).Length;
        List<int> ingameItems = new(length);
        Param param = new Param();

        for (int i = 0; i < length; i++)
        {
            param.Add(((EIngameItem)i).ToString(), 0);
            ingameItems.Add(0);
        }

        Backend.GameData.Insert(InGameItemTableName, param, bro =>
        {
            if (FailError(bro))
                return;

            _DataManager.InitializeInGameItems(ingameItems);

            loadCount--;
        });
    }

    /// <summary>
    /// **하나당 서버 호출하기때문에 바람직 하지 않음 => 삭제
    /// </summary>
    /// <param name="idx"></param>
    /// <param name="value"></param>
    public void UpdateInGameItem(int idx, int value)
    {
        Param param = new Param();        
        param.AddCalculation(((EIngameItem)idx).ToString(), GameInfoOperator.addition, value);

        Backend.GameData.UpdateWithCalculation(InGameItemTableName, new Where(), param, bro =>
        {
            if (FailError(bro))
                return;

            Debug.Log($"인게임 아이템 누적 : {(EIngameItem)idx} - {value}");
        });
    }

    public void UpdateInGameItem(List<int> items)
    {
        Param param = new Param();
        var type = System.Enum.GetValues(typeof(EIngameItem));
        int idx = 0;
        foreach (EIngameItem item in type)
        {
            param.Add(item.ToString(), items[idx++]);
            //Debug.Log($"{item} - {items[idx]}");
        }

        Backend.GameData.Update(InGameItemTableName, new Where(), param, bro => 
        {
            if (FailError(bro))
                return;

            Debug.Log("Update ingameItems");
        });
        
    }

    #endregion 인게임 아이템


    #region 상점

    private void GetShopData()
    {
        loadCount++;

        Backend.GameData.GetMyData(ShopTableName, new Where(), bro =>
        {
            if (FailError(bro))
                return;

            if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
            {
                InsertShopItem();
                return;
            }
            var rows = bro.FlattenRows();

            
            buyWeeklyRP.Value = System.DateTime.Parse(rows[0][BuyWeeklyColName].ToString());
            adRemoveRP.Value =  System.DateTime.Parse(rows[0][RemainRemoveAdColName].ToString());
            buyDoubleCoinRP.Value = rows[0][BuyDoubleCoinColName].ToString() == "True";
            paymentRP.Value = int.Parse(rows[0][IsBoughtColName].ToString());
            

            UnityAdsManager.Instance.SwitchAds(IsAdRemove);

            //if(isReset)
            //    ResetWeeklyData();

            loadCount--;
        });
    }

    private void InsertShopItem()
    {
        Param param = new Param();

        buyWeeklyRP.Value = System.DateTime.MinValue;
        adRemoveRP.Value = System.DateTime.MinValue;
        buyDoubleCoinRP.Value = false;
        paymentRP.Value = 0;

        param.Add(BuyWeeklyColName, buyWeeklyRP.Value);
        param.Add(RemainRemoveAdColName, adRemoveRP.Value);
        param.Add(BuyDoubleCoinColName, buyDoubleCoinRP.Value);
        param.Add(IsBoughtColName, paymentRP.Value);

        Backend.GameData.Insert(ShopTableName, param, bro =>
        {
            if (FailError(bro))
                return;

            // _DataManager.InitializeShopItems(ingameItems);
            UnityAdsManager.Instance.SwitchAds(false);

            loadCount--;
        });
    }

    /// <summary>
    /// 월요일 초기화
    /// </summary>
    public void UpdateShopWeeklyItem()
    {
        if (buyWeeklyRP.Value > System.DateTime.Now)
            return;

        // 다음 clear 날짜 : 다음주 월요일
        var today = System.DateTime.Today;
        var untilMonday = ((int)System.DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
        if (untilMonday == 0)
            untilMonday = 7;
        var nextMonday = today.AddDays(untilMonday);

        Param param = new Param();
        param.Add(BuyWeeklyColName, nextMonday);

        Backend.GameData.Update(ShopTableName, new Where(), param, bro =>
        {
            if (FailError(bro))
                return;

            Debug.Log($"주간 아이템 : {nextMonday}");
            buyWeeklyRP.Value = nextMonday;
        });
    }

    /// <summary>
    /// 매주 월요일 리셋 : 
    ///  * 화요일 접속 시에는?
    /// </summary>
    private void ResetWeeklyData()
    {
        var servertime = Backend.Utils.GetServerTime();
        var serverDateTime = System.DateTime.Parse(servertime.GetReturnValuetoJSON()["utcTime"].ToString());

        Debug.Log($"{serverDateTime} == {lastLoginDate} ?");
        if(serverDateTime >= buyWeeklyRP.Value)
        {
            // ?굳이 리셋?
        }
    }

    /// <summary>
    /// 중첩 없어짐
    /// </summary>
    //public void UpdateShopRemoveAdItem(Time value)
    public void UpdateShopRemoveAdItem(int value)
    {
        Param param = new Param();

        var remainTime = adRemoveRP.Value < System.DateTime.Now 
                                ? System.DateTime.Now.AddDays(value)
                                : adRemoveRP.Value.AddDays(value);
        param.Add(RemainRemoveAdColName, remainTime);

        Backend.GameData.Update(ShopTableName, new Where(), param, bro =>
        {
            if (FailError(bro))
                return;

            Debug.Log($"광고 제거 구입");

            adRemoveRP.Value = remainTime;

            UnityAdsManager.Instance.SwitchAds(true);
        });
    }

    /// <summary>
    /// 무조건 구매만 가능
    /// </summary>
    public void UpdateShopDoubleCoinItem()
    {
        if (IsBuyDoubleCoin.Value)
            return;

        Param param = new Param();
        param.Add(BuyDoubleCoinColName, true);


        Backend.GameData.Update(ShopTableName, new Where(), param, bro =>
        {
            if (FailError(bro))
                return;

            Debug.Log($"무제한 2배 코인 구매");

            buyDoubleCoinRP.Value = true;
        });
    }

    // 첫결제 확인
    public void UpdatePayment(int price)
    {
        Param param = new Param();
        param.AddCalculation(IsBoughtColName, GameInfoOperator.addition, price);

        Backend.GameData.UpdateWithCalculation(ShopTableName, new Where(), param, bro =>
        {
            if (FailError(bro))
                return;

            paymentRP.Value += price;
            Debug.Log($"결제 금액 갱신 : {paymentRP.Value}");
        });
    }

    #endregion 상점


    #region 광고

    private void GetAdsData()
    {
        if (isReset)
            return;

        loadCount++;

        Backend.GameData.GetMyData(AdsTableName, new Where(), bro =>
        {
            if (FailError(bro))
                return;

            if (bro.GetReturnValuetoJSON()["rows"].Count <= 0)
            {
                InsertAdsItem();
                return;
            }

            var rows = bro.FlattenRows();

            var type = System.Enum.GetValues(typeof(EAds));
            Dictionary<EAds, int> adCounts = new(type.Length);

            foreach (EAds item in type)
            {
                adCounts.Add(item, int.Parse(rows[0][item.ToString()].ToString()));
            }

            Debug.Log("광고 InitializeAdCounts");
            _DataManager.InitializeAdCounts(adCounts);

            loadCount--;
        });
    }

    private void InsertAdsItem()
    {
        int length = System.Enum.GetValues(typeof(EAds)).Length;
        Dictionary<EAds, int> adCounts = new(length);
        Param param = new Param();

        for (int i = 0; i < length; i++)
        {
            param.Add(((EAds)i).ToString(), Values.MAX_Ad_Count);
            adCounts.Add((EAds)i, Values.MAX_Ad_Count);
        }

        Backend.GameData.Insert(AdsTableName, param, bro =>
        {
            if (FailError(bro))
                return;

            _DataManager.InitializeAdCounts(adCounts);

            loadCount--;
        });
    }

    public void UpdateAdsItem(Dictionary<string, int> datas)
    {
        Param param = new Param();
        foreach (var data in datas)
        {
            param.Add(data.Key, data.Value);
        }

        var bro = Backend.GameData.Update(AdsTableName, new Where(), param);
        if (FailError(bro))
            return;

        Debug.Log("Update All Ads");
    }

    public void UpdateAdsItem(EAds type, int value)
    {
        Param param = new Param();
        param.Add((type).ToString(), value);

        Backend.GameData.Update(AdsTableName, new Where(), param, bro =>
        {
            if (FailError(bro))
                return;

            Debug.Log("Update Ads");
        });
    }


    private void ResetAdsData()
    {
        loadCount++;

        var tableBro = Backend.GameData.GetMyData(AdsTableName, new Where());
        if (tableBro.GetReturnValuetoJSON()["rows"].Count <= 0)
        {
            InsertAdsItem();
            return;
        }

        int length = System.Enum.GetValues(typeof(EAds)).Length;
        Dictionary<EAds, int> adCounts = new(length);

        Param param = new Param();
        for (int i = 0; i < length; i++)
        {
            param.Add(((EAds)i).ToString(), Values.MAX_Ad_Count);
            adCounts.Add((EAds)i, Values.MAX_Ad_Count);
        }

        Backend.GameData.Update(AdsTableName, new Where(), param, bro =>
        {
            if (FailError(bro))
                return;

            _DataManager.InitializeAdCounts(adCounts);

            loadCount--;
        });
    }

    #endregion 광고


    #region 우편

    /// <summary>
    /// **개수 확인 필요
    /// </summary>
    public void GetMails()
    {
        ++loadCount;

        mailRC.Clear();

        var bro = Backend.UPost.GetPostList(PostType.Admin, 10);

        if (FailError(bro))
            return;

        JsonData json = bro.GetReturnValuetoJSON()["postList"];

        FMailInfo mail = new FMailInfo();
        int count = 0;

        foreach (JsonData rows in json)
        {
            count = rows["items"].Count;
            if (count <= 0)           // 아이템이 없는 메일 X
                continue;

            mail.title = rows["title"].ToString();
            mail.contents = rows["content"].ToString();

            mail.type = count > 1 ? EPostType.Package : EPostType.Normal;
            mail.inDate = rows["inDate"].ToString();
            mail.limit = System.DateTime.Parse(rows["expirationDate"].ToString()) - System.DateTime.Now;

            mail.getItems = new FGetItem[count];
            int idx = 0;
            foreach (JsonData row in rows["items"])
            {
                mail.getItems[idx].type = (EGameItem)int.Parse(row["item"]["itemID"].ToString());
                mail.getItems[idx].count = int.Parse(row["itemCount"].ToString());

                if(idx == 0)
                    mail.icon = _GameData.GameItemSpriteMap[mail.getItems[idx].type];

                idx++;
            }
            mailRC.Add(mail);
        }

        // Debug.Log($"서버 메일 수 : {mailRC.Count}");

        loadCount--;
    }

    public void ReceiveMail(FMailInfo mail)
    {
        var bro = Backend.UPost.ReceivePostItem(PostType.Admin, mail.inDate);
        if (!bro.IsSuccess())
        {
            Debug.LogWarning($"메일 못받음 : {bro}");
            return;
        }

        mailRC.Remove(mail);

        //JsonData postList = bro.GetReturnValuetoJSON()["postItems"];

        //EGameItem item;
        //int count;
        //foreach (JsonData post in postList)
        //{
        //    if (post.Count <= 0)
        //    {
        //        Debug.Log("아이템이 없는 우편");
        //        continue;
        //    }

        //    item = (EGameItem)System.Enum.Parse(typeof(EGameItem), post["item"]["name"].ToString());
        //    count = int.Parse(post["itemCount"].ToString());
        //}
    }

    #endregion


    #region 공지사항

    public List<KeyValuePair<string, string>> GetNews()
    {
        var bro = Backend.Notice.NoticeList(10);

        if (FailError(bro))
            return null;

        var result = new List<KeyValuePair<string, string>>();

        var rows = bro.FlattenRows();
        foreach (JsonData row in rows)
        {
            result.Add(new KeyValuePair<string, string>(row["title"].ToString(), row["content"].ToString()));
        }

        return result;
    }

    #endregion


    #region 설정

    public List<FGetItem> IsValidCoupon(string code)
    {
        var bro = Backend.Coupon.UseCoupon(code);
        if (FailError(bro))
            return null;

        var result = new List<FGetItem>();

        JsonData json = bro.GetReturnValuetoJSON();
        foreach (JsonData rows in json["itemObject"])
        {
            result.Add(new FGetItem(
                (EGameItem)int.Parse(rows["item"]["itemID"].ToString()),
                int.Parse(rows["itemCount"].ToString())
                ));

            // AddItem(id, value);      => SystemUI
        }

        return result;
    }

    #endregion


    #region Playerprefs

    public bool PrefsHasKey(string key)
    {
        Debug.Log($"플레이어 키 값 : {Nickname}_{key}");
        return PlayerPrefs.HasKey($"{Nickname}_{key}");
    }

    public void PrefsSetInt(string key, int value)
    {
        PlayerPrefs.SetInt($"{Nickname}_{key}", value);
    }

    public void PrefsSetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat($"{Nickname}_{key}", value);
    }

    public void PrefsSetString(string key, string value)
    {
        PlayerPrefs.SetString($"{Nickname}_{key}", value);
    }

    public int PrefsGetInt(string key)
    {
        return PlayerPrefs.GetInt($"{Nickname}_{key}");
    }

    public float PrefsGetFloat(string key)
    {
        return PlayerPrefs.GetFloat($"{Nickname}_{key}");
    }

    public string PrefsGetString(string key)
    {
        return PlayerPrefs.GetString($"{Nickname}_{key}");
    }


    #endregion
}
