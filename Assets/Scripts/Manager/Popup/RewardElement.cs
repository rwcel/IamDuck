using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class RewardElement : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI countText;
    [SerializeField] AnimationCurve animationCurve;
    [SerializeField] AnimEvent animEvent;
    public AnimEvent AnimEvent => animEvent;

    // -70 -> 0f
    Sequence sequence;

    public void InitializeWithData(FGetItem item)
    {
        icon.sprite = GameData.Instance.GameItemSpriteMap[item.type];
        countText.text = item.count.ToString();
    }

    //public void SetSequence(Sequence sequence)
    //{
    //    this.sequence = sequence;
    //}

    //public void PlayTween()
    //{
    //    sequence.Play();
    //}
}
