using UnityEngine;
using UnityEngine.UI;


public class NoticeUI : PopupUI
{
    [SerializeField] Toggle newsToggle;
    [SerializeField] GameObject newsObj;

    [SerializeField] Toggle forumToggle;
    [SerializeField] GameObject forumObj;

    protected override void Awake()
    {
        base.Awake();

        newsToggle.onValueChanged.AddListener((value) => newsObj.SetActive(value));
        forumToggle.onValueChanged.AddListener((value) => forumObj.SetActive(value));

    }

    private void OnDisable()
    {
        forumToggle.isOn = false;
    }

    protected override void UpdateData()
    {
        _DataManager.IsLogin = true;

        newsToggle.isOn = false;
        newsToggle.isOn = true;
    }
}
