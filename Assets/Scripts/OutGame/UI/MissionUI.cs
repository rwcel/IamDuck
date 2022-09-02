using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using UniRx;
using System.Linq;

public class MissionUI : PopupUI
{
    [SerializeField] Transform missionListParent;
    [SerializeField] Transform rewardListParent;

    private MissionElement[] missionElements;
    private MissionRewardElement[] rewardElements;

    [BoxGroup("������")]
    [SerializeField] Slider gaugeSlider;
    [BoxGroup("������")]
    [SerializeField] TextMeshProUGUI gaugeText;

    [SerializeField] Button recvButton;

    private static readonly int _Rewarded = -1;

    // private int curGauge;

    // ServerData -> items :
    // �ʱ�ȭ �۾�

    protected override void Awake()
    {
        base.Awake();

        if (_DataManager == null)
            _DataManager = DataManager.Instance;

        missionElements = new MissionElement[_DataManager.MissionDatas.Length];
        rewardElements = new MissionRewardElement[_DataManager.MissionRewardDatas.Length];
        for (int i = 0, length = missionElements.Length; i < length; i++)
        {
            int idx = i;
            missionElements[i] = missionListParent.GetChild(i).GetComponent<MissionElement>();
            missionElements[i].OnRecvMission = () =>
            {
                _DataManager.RecvMission(idx, _Rewarded);
                UpdateRewardGauge();
                CheckNotify();
                CheckAllMissions();
            };
        }
        for (int i = 0, length = rewardElements.Length; i < length; i++)
        {
            rewardElements[i] = rewardListParent.GetChild(i).GetComponent<MissionRewardElement>();
        }

        gaugeSlider.maxValue = Values.Mission_Reward_MaxValue;
    }

    // ���� 1ȸ sorting
    protected override void Start()
    {
        base.Start();

        recvButton.onClick.AddListener(RecvReward);

        SortingMissions();
    }

    protected override void UpdateData()
    {
        // idx
        UpdateRewardGauge();

        SortingMissions();
    }

    void UpdateRewardGauge()
    {
        int count = 0;
        for (int i = 0, length = missionElements.Length; i < length; i++)
        {
            if (missionElements[i].Data.currentValue == _Rewarded)
            {
                count += _DataManager.MissionDatas[i].gauge;
            }
        }

        for (int i = 0, length = rewardElements.Length; i < length; i++)
        {
            rewardElements[i].UpdateData(count, _DataManager.MissionReward.Value);
        }

        gaugeText.text = count.ToString();
        gaugeSlider.value = count;
    }

    /// <summary>
    /// **OnEnable �ڿ� ����Ǿ����
    /// </summary>
    private void SortingMissions()
    {
         var missions = missionElements
            .OrderBy(x => x.State)
            .ThenBy(x => x.Num).ToList();

        for (int i = 0, length = missions.Count; i < length; i++)
        {
            // Debug.Log($"{missions[i].Num} - {missions[i].State}");
            missions[i].transform.SetSiblingIndex(i);
        }
    }

    private void CheckNotify()
    {
        var result = missionElements
            .Where(x => x.State == EElementState.Clear);

        if (result == null || result.Count() == 0)
            _DataManager.SetNotify(EHomeNotify.Mission, false);
    }

    /// <summary>
    /// �� �ִ� �̼��� �ٸ��̼ǰ� ���谡 �ֱ⶧���� ���� �ʿ�
    /// </summary>
    private void CheckAllMissions()
    {
        foreach (var item in missionElements)
        {
            item.UpdateData();
        }
    }

    private void RecvReward()
    {
        int rewardIdx = _DataManager.MissionReward.Value;
        if (rewardIdx >= rewardElements.Length)
            return;

        AudioManager.Instance.PlaySFX(ESfx.Reward);
        rewardElements[rewardIdx].RecvReward();
        // SystemUI.Instance.OpenNoneTouch(Values.Local_Table_Mission, Values.Local_Entry_NextReward);
    }
}
