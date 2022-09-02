using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

// ** 알림 추가 필요
public class MissionRewardElement : MonoBehaviour
{
    private enum EState
    {
        Progress,
        Clear,
        Current,            // Clear했지만 value가 다르면 not Current
        Reward,
    }

    // **서버에서 갱신이 되는데 굳이 Init을 할 필요가?
    [SerializeField] MissionRewardData data;

    [SerializeField] Image image;           // 리워드
    [SerializeField] TextMeshProUGUI countText;
    [SerializeField] TextMeshProUGUI gaugeText;
    [SerializeField] GameObject rewardedObj;
    [SerializeField] GameObject notify;

    private EState curState;
    private Animator anim;

    private static readonly int _Anim_Reward = Animator.StringToHash("Reward");


    // 현재 데이터는?
    public void Awake()
    {
        anim = GetComponent<Animator>();

        image.sprite = GameData.Instance.GameItemSpriteMap[(EGameItem)data.id];
        countText.text = $"x{data.value}";
        gaugeText.text = data.gauge.ToString();

        // rect위치로 포지션 조절
        //var rect = GetComponent<RectTransform>();
        //rect.anchorMin = new Vector2(data.gauge / (float)Values.Mission_Reward_MaxValue, 1);
        //rect.anchorMax = new Vector2(data.gauge / (float)Values.Mission_Reward_MaxValue, 1);
        //rect.anchoredPosition = Vector2.zero;
    }

    private void Start()
    {
        DataManager.Instance.MissionReward
            .Subscribe(value =>
            {
                if (value == data.num && curState == EState.Clear)
                {
                    ChangeState(EState.Current);
                }
            }).AddTo(gameObject);


    }

    // int value, bool isReward,
    public void UpdateData(int value, int rewardIdx)
    {
        // state변경 확인. 
        if(value >= data.gauge)
        {
            ChangeState(rewardIdx > data.num ? EState.Reward
                                : (rewardIdx == data.num ? EState.Current : EState.Clear));
        } 
        else
        {
            ChangeState(EState.Progress);
        }
    }

    private void ChangeState(EState state)
    {
        rewardedObj.SetActive(state == EState.Reward);
        notify.SetActive(state == EState.Current);

        curState = state;
    }

    public bool RecvReward()
    {
        if (curState == EState.Progress || curState == EState.Reward)
        {
            Debug.Log("not enabled?");
            return false;
        }

        // data => rewarded : 서버
        // OnRecvMission?.Invoke();
        if(DataManager.Instance.RewardMission(data))
        {
            ChangeState(EState.Reward);
            anim.SetTrigger(_Anim_Reward);
            Debug.Log("상태변환");

            return true;
        }

        return false;
    }
}
