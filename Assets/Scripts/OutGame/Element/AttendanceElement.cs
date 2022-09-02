using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttendanceElement : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI dayText;
    [SerializeField] protected Image icon;
    [SerializeField] protected TextMeshProUGUI countText;
    [SerializeField] protected GameObject rewardedObj;
    [SerializeField] protected GameObject todayObj;

    protected Animator anim;
    protected AnimEvent animEvent;
    public AnimEvent AnimEvent => animEvent;

    private static readonly int _Anim_Reward = Animator.StringToHash("Reward");


    private void Awake()
    {
        anim = GetComponent<Animator>();
        animEvent = GetComponent<AnimEvent>();
    }

    /// <summary>
    /// data.day는 1일부터 시작
    /// </summary>
    /// <param name="today">0부터 시작해서 1 더해준 값을 전송</param>
    public virtual void InitializeWithData(FDailyCheck data, int today, int count)
    {
        if (animEvent == null)
            animEvent = GetComponent<AnimEvent>();

        dayText.text = $"{data.day}";
        icon.sprite = GameData.Instance.GameItemSpriteMap[(EGameItem)data.id];
        countText.text = $"{data.value}";
        rewardedObj.SetActive(data.day < today);
        todayObj.SetActive(data.day == today);
    }

    public void PlayReward()
    {
        // animator 작동
        rewardedObj.SetActive(true);
        anim.SetTrigger(_Anim_Reward);
    }
}
