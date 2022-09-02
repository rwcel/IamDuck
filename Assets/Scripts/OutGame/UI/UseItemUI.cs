using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using DG.Tweening;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using static Values;

public class UseItemUI : MonoBehaviour
{
    [SerializeField] Transform elementParent;
    protected List<UseItemElement> elements;

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI descText;

    [SerializeField] TextMeshProUGUI costText;

    [SerializeField] GameObject dropdownObj;
    [SerializeField] Transform arrowTr;

    [SerializeField] Button getButton;
    [SerializeField] OnOffButton buyButton;
    [SerializeField] Button adButton;
    [SerializeField] OnOffButton equipButton;

    // [SerializeField] ParticleSystem particle;
    [SerializeField] TextMeshProUGUI adCountText;
    [SerializeField] GameObject cooldownObj;
    [SerializeField] TextMeshProUGUI cooldownText;

    DataManager _DataManager;
    GameData _GameData;
    protected UseItemElement curElement;

    string localizedEquip;
    string localizedUnEquip;

    protected void Awake()
    {
        if (_DataManager == null)
            _DataManager = DataManager.Instance;
        if (_GameData == null)
            _GameData = GameData.Instance;

        elements = new List<UseItemElement>(elementParent.childCount);
        foreach (Transform itemTr in elementParent)
        {
            var element = itemTr.GetComponent<UseItemElement>();
            elements.Add(element);
            element.Toggle.onValueChanged.AddListener((value) =>
            {
                if (value == true)
                    UpdateBottomUI(element);
            });
        }

        localizedEquip = LocalizationSettings.StringDatabase.GetLocalizedString(Local_Table_Manage, Local_Entry_Equip);
        localizedUnEquip = LocalizationSettings.StringDatabase.GetLocalizedString(Local_Table_Manage, Local_Entry_UnEquip);

        InitializeElements();
    }

    private void OnEnable()
    {
        elements[0].Toggle.isOn = false;
        elements[0].Toggle.isOn = true;

        localizedEquip = LocalizationSettings.StringDatabase.GetLocalizedString(Local_Table_Manage, Local_Entry_Equip);
        localizedUnEquip = LocalizationSettings.StringDatabase.GetLocalizedString(Local_Table_Manage, Local_Entry_UnEquip);
    }

    private void OnDisable()
    {
        if(curElement != null)
        {
            curElement.Toggle.isOn = false;
            curElement = null;
        }

        _DataManager.OnAdTimer -= UpdateCooldown;
    }

    protected void Start()
    {
        _DataManager.UseItemObservable
            .Subscribe(UpdateUseItem)
            .AddTo(this.gameObject);
        _DataManager.ItemAdCountObservable
            .Subscribe(UpdateItemAdCount)
            .AddTo(this.gameObject);

        SystemUI systemUI = SystemUI.Instance;

        getButton.onClick.AddListener(() => SwitchDropdown(!dropdownObj.activeSelf));
        buyButton.ClickButton(BuyItem, () => systemUI.OpenNoneTouch(Local_Table_Manage, Local_Entry_NeedMoney));
        adButton.onClick.AddListener(AdItem);
        equipButton.ClickButton(Equip, () => systemUI.OpenNoneTouch(Local_Table_Manage, Local_Entry_NotHave));
    }

    private void InitializeElements()
    {
        int idx = 0;
        foreach (var element in elements)
        {
            element.InitializeWithData(_DataManager.UseItems[idx], 0);
            element.OnEquip += (value) => _DataManager.SetUseItemTypeValue(element.Data.type, value);
            idx++;
        }
    }

    private void UpdateBottomUI(UseItemElement element)
    {
        if (curElement == null || element != curElement)
        {   // �ٸ� ���

            var localizedItemName = new LocalizedString(element.Data.tableName, element.Data.nameEntry)
            {
                {Local_Name_UseItem,
                    new UnityEngine.Localization.SmartFormat.PersistentVariables.FloatVariable{Value = element.Data.value}}
            };

            var localizedItemValue = new LocalizedString(element.Data.tableName, element.Data.valueEntry)
            {
                {Local_Name_UseItem,
                    new UnityEngine.Localization.SmartFormat.PersistentVariables.FloatVariable{Value = element.Data.value}}
            };

            nameText.text = localizedItemName.GetLocalizedString();
            descText.text = localizedItemValue.GetLocalizedString();

            equipButton.gameObject.SetActive(!element.Data.autoEquip);
            equipButton.SetText(element.IsEquip ? localizedUnEquip : localizedEquip);         // �ݴ�

            adCountText.text = $"{_DataManager.AdCounts[element.Data.adType]} / {MAX_Ad_Count}";
            if (_DataManager.AdTimers[element.Data.adType] > 0)
            {
                cooldownObj.SetActive(true);
                cooldownText.text = _DataManager.AdTimers[element.Data.adType].ValueToTime();     // �ʱ� ������
            }
            else
            {
                cooldownObj.SetActive(false);
            }

            AudioManager.Instance.PlaySFX(ESfx.Touch);
        }

        costText.text = element.Data.cost.ToString();
        buyButton.SetButton(_DataManager.Coin.Value >= element.Data.cost);
        SwitchDropdown(false);
        equipButton.SetButton(element.Count > 0);
        //equipButton.interactable = _DataManager..UseItems[i]

        curElement = element;
    }

    private void SwitchDropdown(bool isActive)
    {
        // �߰��� �ٽ� Ŭ���ϴ°� ����?

        dropdownObj.SetActive(isActive);
        arrowTr.DOScaleY(isActive ? 1f : -1f, 0.2f);

        if(isActive)
        {
            // Debug.Log("Enable");
            _DataManager.OnAdTimer += UpdateCooldown;
            cooldownText.text = _DataManager.AdTimers[curElement.Data.adType].ValueToTime();
            buyButton.SetButton(_DataManager.Coin.Value >= curElement.Data.cost);          // ������ ���� �������� �� �� Ȯ��
        }
        else
        {
            // Debug.Log("Disable");
            _DataManager.OnAdTimer -= UpdateCooldown;
        }
    }

    private void UpdateCooldown(EAds type, int value)
    {
        Debug.Log($"UseItem : {type} - {value}");

        if (type != curElement.Data.adType)
            return;

        if (value == 0)
        {
            cooldownObj.SetActive(false);
            cooldownText.text = "";
        }
        else
        {
            cooldownText.text = value.ValueToTime();
        }
    }

    /// <summary>
    /// cost�� playerCoin ��
    /// ���� �߰�
    /// Data Save
    /// **������ �����ֱ� OnEvent + UpdateData
    /// </summary>
    private void BuyItem()
    {
        if (_DataManager.BuyUseItem(curElement.Data.type, curElement.Data.cost))
        {
            AudioManager.Instance.PlaySFX(ESfx.ItemBuy);
            curElement?.OnAdd();
        }
    }

    /// <summary>
    /// adCount�� maxAdCount ��
    /// ���� ����
    /// Data Save
    /// </summary>
    private void AdItem()
    {
        if (cooldownObj.activeSelf)
        {
            AudioManager.Instance.PlaySFX(ESfx.Disable);
            SystemUI.Instance.OpenNoneTouch(Values.Local_Table_Manage, Values.Local_Entry_NotAds);
            return;
        }

        AudioManager.Instance.PlaySFX(ESfx.Touch);
        _DataManager.AdUseItem(curElement.Data.adType, OnAdItem);
    }

    void OnAdItem()
    {
        // AdCount, AdTime
        curElement?.OnAdd();

        cooldownObj.SetActive(true);
        // UpdateData();
    }

    /// <summary>
    /// ���� or ���� : ������ �ݿ� �ʿ�
    /// </summary>
    private void Equip()
    {
        bool isEquip = !curElement.IsEquip;
        curElement.EquipItem(isEquip);
        equipButton.SetText(isEquip ? localizedUnEquip : localizedEquip);         // �ݴ�
    }


    private void UpdateUseItem(CollectionReplaceEvent<int> element)
    {
        elements[element.Index].UpdateCount(element.NewValue);
        UpdateBottomUI(elements[element.Index]);
    }
    private void UpdateItemAdCount(CollectionReplaceEvent<int> element)
    {
        elements[element.Index].UpdateAdCount(element.NewValue);
        UpdateBottomUI(elements[element.Index]);
    }

    //private void UpdateUpgrade(CollectionReplaceEvent<int> element)
    //{
    //    elements[element.Index].UpdateData(element.NewValue);
    //    UpdateUpgradeBottom(elements[element.Index]);
    //}
}
