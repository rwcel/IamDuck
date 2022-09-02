using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;


public class InGameManager : Singleton<InGameManager>
{
    [SerializeField] PlayerController playerController;
    [SerializeField] ParticleSystem reviveParticle;

    private readonly ReactiveProperty<int> scoreRP = new IntReactiveProperty(0);
    private readonly ReactiveProperty<int> heightRP = new IntReactiveProperty(0);
    private readonly ReactiveProperty<int> recordHeightRP = new IntReactiveProperty(0);
    private readonly ReactiveProperty<int> coinRP = new IntReactiveProperty(0);                 // 인게임에서 얻은 코인
    private readonly ReactiveProperty<bool> feverRP = new ReactiveProperty<bool>(false);

    public ReadOnlyReactiveProperty<int> Score => scoreRP.ToReadOnlyReactiveProperty();
    public ReadOnlyReactiveProperty<int> Height => heightRP.ToReadOnlyReactiveProperty();
    public ReadOnlyReactiveProperty<int> RecordHeight => recordHeightRP.ToReadOnlyReactiveProperty();
    public ReadOnlyReactiveProperty<int> Coin => coinRP.ToReadOnlyReactiveProperty();
    public ReadOnlyReactiveProperty<bool> IsFever => feverRP.ToReadOnlyReactiveProperty();      // * 지금은 사용하지 않음. UI 바꿔야할때 필요

    DataManager _DataManager;
    LevelData _LevelData;
    AudioManager _AudioManager;

    RecordData recordData;

    private int heightRewardIdx = 0;

    private float[] upgradeValues;
    public float UpgradeValue(EUpgradeType type) => upgradeValues[(int)type];

    private bool isRevived;

    private bool canTouchUI = true;
    public bool CanTouchUI => canTouchUI;

    public bool IsPause;


    protected override void AwakeInstance()
    {
        _DataManager = DataManager.Instance;
        _LevelData = LevelData.Instance;
        _AudioManager = AudioManager.Instance;

        recordData = new RecordData();

        SetUpgradeValues();
    }

    protected override void DestroyInstance()
    {
    }

    /// <summary>
    /// 업그레이드 값 캐싱하기
    /// </summary>
    void SetUpgradeValues()
    {
        upgradeValues = new float[_DataManager.UpgradeLevels.Count];
        for (int i = 0, length = upgradeValues.Length; i < length; i++)
        {
            upgradeValues[i] = _DataManager.UpgradeLevels[i] * _LevelData.UpgradeDatas[i].upgradeValue;
            // Debug.Log($"{_DataManager.UpgradeLevels[i]} * {_LevelData.UpgradeDatas[i].upgradeValue}");
        }
    }

    private void Start()
    {
        ClearData();

        BindEvents();

        SetRecordData();
    }

    void ClearData()
    {
        scoreRP.Value = 0;
        heightRP.Value = 0;
        coinRP.Value = 0;

        recordHeightRP.Value = _DataManager.Height.Value;
        // SetTimer(0);

        isRevived = false;
    }

    void BindEvents()
    {
        //Debug.Assert(playerController != null, "플레이어 컨트롤러 없음");
        if (playerController == null)
            return;

        playerController.OnFever += (value) => feverRP.Value = value;
        playerController.OnDead += Dead;
        playerController.OnFly += (value) => canTouchUI = !value;
        playerController.OnHit += () => canTouchUI = false;
        playerController.OnDead += () => canTouchUI = true;
        playerController.OnRevive += OnRevive;
    }

    void UnbindEvents()
    {
        //Debug.Assert(playerController != null, "플레이어 컨트롤러 없음");
        if (playerController == null)
            return;

        playerController.OnFever -= (value) => feverRP.Value = value;
        playerController.OnDead -= Dead;
        playerController.OnFly = (value) => canTouchUI = !value;
        playerController.OnHit -= () => canTouchUI = false;
        playerController.OnDead -= () => canTouchUI = true;
        playerController.OnRevive -= OnRevive;
    }

    void Dead()
    {
        //if (GameApplication.Instance.IsTestMode)
        //    isRevived = false;

        if (!isRevived && _DataManager.HaveUseItemType(EUseItemType.Continue)) 
        {
            GamePopup.Instance.OpenContinue();
            isRevived = true;
        }
        else
        {
            GamePopup.Instance.OpenGameOver();
        }
    }

    void OnRevive()
    {
        Debug.Log("OnRevive");
        reviveParticle.Play();
    }

    void SetRecordData()
    {
        Coin.Subscribe(value => recordData.coin = value)
            .AddTo(this.gameObject);
        Height.Subscribe(value =>
        {
            recordData.height = value;
            AddCoin(RewardHeight(value));
        }).AddTo(this.gameObject);
        recordData.items = new int[System.Enum.GetNames(typeof(EIngameItem)).Length];
    }

    /// <summary>
    /// PlayerController -> GameManager
    /// </summary>
    /// <param name="value"></param>
    public void CheckBestHeight(int value)
    {
        if (heightRP.Value >= value)
            return;

        // scoreRP.Value += _LevelData.CalcScore(heightRP.Value, value);
        heightRP.Value = value;

        RecordBestHeight(value);
    }

    private void RecordBestHeight(int value)
    {
        if(recordHeightRP.Value < value)
        {
            recordHeightRP.Value = value;
        }
        // 게임 종료 후 저장
    }

    public void AddCoin(int value = 1)
    {
        if(value > 0)
        {
            _AudioManager.PlaySFX(ESfx.Coin);
        }

        coinRP.Value += value;
    }

    public void AddItem(EIngameItem type, int value = 1)
    {
        _AudioManager.PlaySFX(ESfx.ItemAdd);

        recordData.items[(int)type] += value;
    }

    public void ChangePlayerState(EPlayerState state) => playerController.ChangeState(state);

    public void SaveData(int coin)
    {
        recordData.coin = coin;

        _DataManager.SaveRecord(recordData);
    }

    /// <summary>
    /// height 100,300,500,700,1000층 일때 보상
    /// </summary>
    /// <param name="height"></param>
    /// <returns></returns>
    private int RewardHeight(int height)
    {
        if (heightRewardIdx >= _LevelData.HeightRewards.Length)
            return 0;

        int goal = _LevelData.HeightRewards[heightRewardIdx];
        if (height == goal)
        {
            heightRewardIdx++;
            return goal;
        }

        return 0;
    }

    #region Time

    //private void SetTimer(float time)
    //{
    //    Observable.FromCoroutine<int>(observer => CoTimerObserver(observer, Values.BaseTime, time))
    //        .Subscribe(value => timeRP.Value -= value,
    //        () => ChangePlayerState(EPlayerState.Hit));
    //}

    ///// <summary>
    ///// **시간 변동하기 때문에 RP를 직접 넣어야함
    ///// </summary>
    //IEnumerator CoTimerObserver(System.IObserver<int> observer, int startTime, float reviveTime)
    //{
    //    timeRP.Value = startTime;
    //    yield return new WaitForSeconds(reviveTime);

    //    while (timeRP.Value > 0)
    //    {
    //        yield return Values.Delay1;
    //        observer.OnNext(1);
    //    }
    //    observer.OnNext(Values.BaseTime);
    //    observer.OnCompleted();
    //}

    //public void AddTime(int value = 5)
    //{
    //    // **코루틴 시간과 관계없이 증가하는 것이기 때문에 먹고서 바로 시간 감소 시 이상해 보임?
    //    timeRP.Value = Mathf.Clamp(timeRP.Value + value, 0, Values.BaseTime + 1);
    //}

    #endregion
}