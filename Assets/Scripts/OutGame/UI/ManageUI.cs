using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ManageUI : MonoBehaviour
{
    private enum ETab
    {
        UseItem, 
        Upgrade,
    }

    [System.Serializable]
    public class Tab<T>
    {
        public Toggle toggle;
        public GameObject panel;            // 미사용 -> 인스펙터 처리
        public T uiScript;
        public GameObject notify;
        // public Transform parent;
    }

    [SerializeField] Tab<UseItemUI> useItemTab;
    public Tab<UseItemUI> UseItemTab => useItemTab;
    [SerializeField] Tab<UpgradeUI> upgradeTab;
    public Tab<UpgradeUI> UpgradeTab => upgradeTab;

    // DataManager _DataManager;

    private void Awake()
    {
        useItemTab.toggle.onValueChanged.AddListener((value) =>
        {
            if(value)
                SwitchTab(ETab.UseItem);
        });
        upgradeTab.toggle.onValueChanged.AddListener((value) =>
        {
            if (value)
                SwitchTab(ETab.Upgrade);
        });
    }

    //void SwitchTab(int idx)
    //{
    //    useItemTab.toggle.onValueChanged .AddListener(() => SwitchTab(ETab.UseItem));
    //    useItemTab.toggle.onValueChanged.AddListener((value) => useItemTab.panel.SetActive(value));
    //    upgradeTab.toggle.onValueChanged.AddListener((value) => upgradeTab.panel.SetActive(value));
    //}

    protected void OnEnable()
    {
        useItemTab.toggle.isOn = false;
        useItemTab.toggle.isOn = true;

        DataManager.Instance.IsManageNotify
            .Subscribe(upgradeTab.notify.SetActive)
            .AddTo(this.gameObject);
    }

    private void OnDisable()
    {
        upgradeTab.toggle.isOn = false;
    }

    //private void Start()
    //{
    //    useItemTab.toggle.onValueChanged .AddListener(() => SwitchTab(ETab.UseItem));
    //    upgradeTab.toggle.onClick.AddListener(() => SwitchTab(ETab.Upgrade));
    //    useItemTab.toggle.onValueChanged.AddListener((value) => useItemTab.panel.SetActive(value));
    //    upgradeTab.toggle.onValueChanged.AddListener((value) => upgradeTab.panel.SetActive(value));
    //}

    private void SwitchTab(ETab tab)
    {
        AudioManager.Instance.PlaySFX(ESfx.Touch);

        useItemTab.panel.SetActive(tab == ETab.UseItem);
        upgradeTab.panel.SetActive(tab == ETab.Upgrade);
    }
}
