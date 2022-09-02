using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// AnimEvent 안쓰고 자체로 처리
/// </summary>
public abstract class PopupUI : MonoBehaviour
{
    [BoxGroup("공통")]
    [SerializeField] protected Button closeButton;
    [BoxGroup("공통")]
    [SerializeField] protected ScrollRect scrollRect;
    [BoxGroup("공통")]
    [SerializeField] protected bool closeAnim = true;

    //protected GameManager _GameManager;
    protected GamePopup _GamePopup;
    protected DataManager _DataManager;

    protected Animator anim;

    protected static readonly string _Anim_Close = "Close";

    System.Action OnClose;


    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();

        if (_GamePopup == null)
        {
            _GamePopup = GamePopup.Instance;
        }
        if (_DataManager == null)
        {
            _DataManager = DataManager.Instance;
        }
    }

    protected virtual void Start()
    {
        if(closeButton != null)
            closeButton.onClick.AddListener(_GamePopup.ClosePopup);
    }

    protected void OnEnable()
    {
        UpdateData();

        if (scrollRect != null)
            StartCoroutine(nameof(CoClearScroll));
    }

    IEnumerator CoClearScroll()
    {
        yield return Values.DelayFrame;
        scrollRect.verticalNormalizedPosition = 1f;
    }

    protected abstract void UpdateData();

    public void PlayCloseAnim(System.Action closeAction)
    {
        OnClose = closeAction;
        if (anim == null || !closeAnim)
        {
            Close();
        }
        else
        {
            anim.SetTrigger(_Anim_Close);
        }
    }

    public void Close()
    {
        OnClose?.Invoke();
        gameObject.SetActive(false);
    }
}