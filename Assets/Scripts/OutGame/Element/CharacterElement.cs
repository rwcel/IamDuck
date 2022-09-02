using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class CharacterElement : MonoBehaviour
{
    [SerializeField] CharacterData data;
    public CharacterData Data => data;

    [SerializeField] Toggle toggle;
    public Toggle Toggle => toggle;

    [SerializeField] Image iconImage;
    [SerializeField] GameObject notify;

    [SerializeField] GameObject lockObj;

    /// <summary>
    /// 추후 업데이트로 추가
    /// </summary>
    public bool UpdateReady => data.updateReady;

    private bool isLock;
    public bool IsLock => isLock;

    private bool canUnlock;
    public bool CanUnlock
    {
        get 
        {
            if (curValue < 0)
                return false;

            return canUnlock = curValue >= data.value; 
        }
        private set 
        {
            if (canUnlock != value)
            {
                OnChangeCanUnlock?.Invoke(value);
            }

            // Debug.Log(data.id + "=>" + value);

            canUnlock = value;
            if(isLock)          // 해제 상태에서는 변하지 않음
            {
                notify.SetActive(value);
            }
        }
    }

    private int curValue = -1;
    public int CurValue => curValue;

    DataManager _DataManager;

    private System.Action unlockAction;
    public System.Action<bool> OnChangeCanUnlock;


    public void InitializeWithData(int num, bool unlock)
    {
        if (_DataManager == null)
            _DataManager = DataManager.Instance;

        data = GameData.Instance.CharacterDatas[num];

        //iconImage.sprite = GameData.Instance.CharacterSprite(data.id);            // **이게 맞음
        iconImage.sprite = data.sprite;

        notify.SetActive(false);

        if (data.updateReady)
            return;


        SetLock(!unlock);
        if(!unlock)
        {
            UnlockCheckAndObserve();
            CanUnlock = curValue >= data.value;
        }
        else
        {
            CanUnlock = false;
        }
    }

    public void Unlock()
    {
        unlockAction?.Invoke();
        CanUnlock = false;
        SetLock(false);
    }

    void SetLock(bool isLock)
    {
        this.isLock = isLock;
        lockObj.SetActive(isLock);
    }

    private void UnlockCheckAndObserve()
    {
        switch (data.category)
        {
            case ECharacterCategory.None:
                curValue = 0;
                break;
            case ECharacterCategory.GamePlay:
                curValue = GamePlayCheck((EGameplay)data.type);
                break;
            case ECharacterCategory.InGameItem:
                // type : InGameItem
                // InGameItemCheck((EIngameItem)data.type, data.value);

                int typeBit = 0;
                int length = System.Enum.GetValues(typeof(EInGameItemBit)).Length;
                while (typeBit < length)
                {
                    if ((1 << typeBit & data.type) != 0)
                    {
                        if (curValue == -1)
                            curValue = 0;

                        curValue += _DataManager.IngameItems[typeBit];
                    }
                    typeBit++;
                }
                
                break;
            case ECharacterCategory.UseGoods:
                // type : EGoods
                unlockAction = () => _DataManager.UseGoods((EGoods)data.type, data.value);
                GoodsObserve((EGoods)data.type, data.value);
                break;
            case ECharacterCategory.HaveGoods:
                // type : EGoods
                GoodsObserve((EGoods)data.type, data.value);
                break;
            case ECharacterCategory.Attendance:
                // value만 사용
                AttendanceObserve(data.value);
                // curValue = _DataManager.TodayAttendance;
                break;
            case ECharacterCategory.Payment:
                BackendManager.Instance.PaymentPrice
                    .Subscribe(x =>
                    {
                        curValue = x;
                        CanUnlock = (x >= data.value);
                    })
                    .AddTo(this.gameObject);
                break;
            case ECharacterCategory.Destroy:

                break;
        }
    }

    /// <summary>
    /// 인게임에서만 변경
    /// </summary>
    private int GamePlayCheck(EGameplay type)
    {
        switch (type)
        {
            case EGameplay.GamePlay:
                return 0;
            case EGameplay.ResultHeight:
                return _DataManager.Height.Value;
            case EGameplay.AccuHeight:
                return _DataManager.AccuHeight.Value;
        }

        return 0;
    }

    /// <summary>
    /// Use, Have는 사용시 확인
    /// </summary>
    private void GoodsObserve(EGoods type, int value)
    {
        switch (type)
        {
            case EGoods.Coin:
                _DataManager.Coin
                    .Subscribe(x =>
                    {
                        curValue = x;
                        CanUnlock = (x >= value);
                    })
                    .AddTo(this.gameObject);
                break;
            case EGoods.CashDia:
                _DataManager.CashDia
                    .Subscribe(x =>
                    {
                        curValue = x;
                        CanUnlock = (x >= value);
                    })
                    .AddTo(this.gameObject);
                break;
            case EGoods.FreeDia:
                _DataManager.FreeDia
                    .Subscribe(x =>
                    {
                        curValue = x;
                        CanUnlock = (x >= value);
                    })
                    .AddTo(this.gameObject);
                break;
            case EGoods.Dia:
                _DataManager.Dia
                    .Subscribe(x =>
                    {
                        curValue = x;
                        CanUnlock = (x >= value);
                    })
                    .AddTo(this.gameObject);
                break;
        }
    }

    /// <summary>
    /// 게임 초기에서만 변경
    /// </summary>
    private void AttendanceObserve(int value)
    {
        _DataManager.TodayAttendance
            .Subscribe(x => 
            {
                curValue = x;
                CanUnlock = (x >= value);
            })
            .AddTo(this.gameObject);

        //Debug.Log($"{_DataManager.TodayAttendance} >= {value}");
        //return _DataManager.TodayAttendance >= value;
    }


    /// <summary>
    /// 인게임에서만 변경
    /// </summary>
    //private bool InGameItemCheck(EIngameItem type, int value)
    //{
    //    _DataManager.IngameItemObservable
    //        .Subscribe()

    //     return _DataManager.IngameItems[(int)type] >= value;
    //}
}
