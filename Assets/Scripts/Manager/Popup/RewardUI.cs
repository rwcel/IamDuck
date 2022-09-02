using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class RewardUI : MonoBehaviour
{
    private enum EState
    {
        Open,
        AddItems,
        Touch,
        Close,
    }

    [SerializeField] Button closeButton;
    [SerializeField] RewardElement[] elements;

    private Animator anim;

    private int count;
    private bool canClose;

    public static readonly int _Anim_Touch = Animator.StringToHash("Touch");
    public static readonly int _Anim_Close = Animator.StringToHash("Close");

    DataManager _DataManager;


    private void Awake()
    {
        _DataManager = DataManager.Instance;

        anim = GetComponent<Animator>();

        SetElementTweens();
    }

    private void SetElementTweens()
    {
        for (int i = 0; i < elements.Length; i++)
        {
            int idx = i+1;
            elements[i].AnimEvent.SetAnimEvent(() => OpenItem(idx));
            //elements[i].SetSequence(DOTween.Sequence(
            //        transform.DORotate(Vector3.zero, 0.15f)
            //    .SetEase(Ease.Linear)
            //    .OnComplete(() => elements[i + 1].PlayTween())
            //    ));
        }
    }

    private void Start()
    {
        closeButton.onClick.AddListener(OnConfirm);
    }


    /// <summary>
    /// 여러개 받도록
    /// </summary>
    public void OpenWithData(List<FGetItem> getItems, System.Action onReward)
    {
        if(_DataManager == null)
            _DataManager = DataManager.Instance;

        count = getItems.Count;
        for (int i = 0, length = getItems.Count; i < length; i++)
        {
            elements[i].InitializeWithData(getItems[i]);
            _DataManager.AddItem(getItems[i]);
        }

        // 전체 끄기
        for (int i = 0, length = elements.Length; i < length; i++)
        {
            elements[i].gameObject.SetActive(false);
        }

        gameObject.SetActive(true);
        ChangeState(EState.Open);

        onReward?.Invoke();
    }

    private void ChangeState(EState state)
    {
        switch (state)
        {
            case EState.Open:
                // 따로 없음
                canClose = false;
                AudioManager.Instance.PlaySFX(ESfx.Reward);
                break;
            case EState.AddItems:
                // Element Play Animation
                elements[0].gameObject.SetActive(true);
                //for (int i = 0; i < count; i++)
                //{   // 순서대로
                //    elements[i].gameObject.SetActive(true);
                //}
                // end -> Touch On
                break;
            case EState.Touch:
                anim.SetTrigger(_Anim_Touch);
                canClose = true;
                break;
            case EState.Close:
                anim.SetTrigger(_Anim_Close);
                break;
        }
    }

    public void OpenItem(int idx)
    {
        if (idx >= count)
        {
            ChangeState(EState.Touch);
            return;
        }

        elements[idx].gameObject.SetActive(true);
    }

    void OnConfirm()
    {
        if (!canClose)
            return;

        //AudioManager.Instance.PlaySFX(ESFX.Touch);

        ChangeState(EState.Close);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
