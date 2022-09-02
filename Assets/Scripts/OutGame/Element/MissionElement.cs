using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.Localization;

public class MissionElement : MonoBehaviour
{

    [BoxGroup("������ �ʿ�")]
    [SerializeField] MissionData data;
    public MissionData Data => data;

    [BoxGroup("UI")]
    [SerializeField] TextMeshProUGUI nameText;
    [BoxGroup("UI")]
    [SerializeField] OnOffButton recvButton;

    [BoxGroup("UI")]
    [SerializeField] Slider slider;
    [BoxGroup("UI")]
    [SerializeField] TextMeshProUGUI barText;
    [BoxGroup("UI")]
    [SerializeField] GameObject rewardedObj;

    [BoxGroup("����")]
    [SerializeField] Image gaugeImage;
    [BoxGroup("����")]
    [SerializeField] TextMeshProUGUI gaugeCountText;

    [BoxGroup("����")]
    [SerializeField] Image rewardImage;
    [BoxGroup("����")]
    [SerializeField] TextMeshProUGUI rewardCountText;

    private EElementState state;
    public EElementState State => state;
    public int Num => data.num;
    private static readonly int _Rewarded = -1;

    public System.Action OnRecvMission;

    private LocalizedString localizedMission;


    private void Awake()
    {
        // nameText.text = data.nameText;

        slider.gameObject.SetActive(data.isBar);
        gaugeImage.sprite = GameData.Instance.GameItemSpriteMap[EGameItem.Gauge];
        gaugeCountText.text = $"{data.gauge}";
        rewardImage.sprite = GameData.Instance.GameItemSpriteMap[(EGameItem)data.rewardID];
        rewardCountText.text = data.rewardValue.ToString();

        localizedMission = new LocalizedString(tableReference: data.tableName, entryReference: data.keyName)
        {
            {Values.Local_Name_Mission, 
                new UnityEngine.Localization.SmartFormat.PersistentVariables.IntVariable{Value = data.missionValue}}
        };
    }

    //public void InitializeWithData(MissionData missionData)
    //{
    //    this.missionData = missionData;
    //}

    private void Start()
    {
        recvButton.ClickButton(RecvMission, 
                    () => SystemUI.Instance.OpenNoneTouch(Values.Local_Table_Mission, Values.Local_Entry_FailMission));
    }

    private void OnEnable()
    {
        // UpdateData(DataManager.Instance.Missions[data.num]);
        UpdateData();

        nameText.text = localizedMission.GetLocalizedString();
    }


    public void UpdateData()
    {
        if (data.currentValue < data.missionValue
            && data.currentValue != _Rewarded)
        {
            barText.text = $"{data.currentValue}/{data.missionValue}";
            slider.value = (float)data.currentValue / data.missionValue;

            ChangeState(EElementState.Progress);
        }
        else
        {
            barText.text = $"{data.missionValue}/{data.missionValue}";
            slider.value = 1f;
            ChangeState(data.currentValue == _Rewarded ? EElementState.Reward : EElementState.Clear);
        }
    }

    /// <summary>
    /// **ChangeState ���� Invoke : Notify �˻�
    /// </summary>
    void RecvMission()
    {
        if (!recvButton.enabled)
            return;

        List<FGetItem> getItmes = new List<FGetItem>();
        getItmes.Add(new FGetItem(EGameItem.Gauge, data.gauge));
        getItmes.Add(new FGetItem((EGameItem)data.rewardID, data.rewardValue));

        SystemUI.Instance.OpenReward(getItmes);

        ChangeState(EElementState.Reward);

        OnRecvMission?.Invoke();
    }

    // int value, bool isReward,
    public void UpdateData(int value, bool isReward)
    {
        // state���� Ȯ��. 

        // barText.text = value / missionValue;
    }

    private void ChangeState(EElementState state)
    {
        this.state = state;

        rewardedObj.SetActive(state == EElementState.Reward);
        recvButton.SetButton(state == EElementState.Clear);
    }
}
