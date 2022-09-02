using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] Transform elementParent;
    protected List<UpgradeElement> elements;

    [BoxGroup("이름")]
    [SerializeField] TextMeshProUGUI nameText;
    [BoxGroup("이름")]
    [SerializeField] TextMeshProUGUI descText;

    [BoxGroup("슬라이더")]
    [SerializeField] Slider slider;
    [BoxGroup("슬라이더")]
    [SerializeField] TextMeshProUGUI valueText;
    [BoxGroup("버튼")]
    [SerializeField] TextMeshProUGUI upText;
    [BoxGroup("버튼")]
    [SerializeField] TextMeshProUGUI costText;
    [BoxGroup("버튼")]
    [SerializeField] OnOffButton upgradeButton;
    [BoxGroup("버튼")]
    [SerializeField] Button maxButton;

    [SerializeField] ParticleSystem particle;

    DataManager _DataManager;
    GameData _GameData;
    protected UpgradeElement curElement;


    protected void Awake()
    {
        if (_DataManager == null)
            _DataManager = DataManager.Instance;
        if (_GameData == null)
            _GameData = GameData.Instance;

        elements = new List<UpgradeElement>(elementParent.childCount);
        foreach (Transform itemTr in elementParent)
        {
            var element = itemTr.GetComponent<UpgradeElement>();
            elements.Add(element);
            element.Toggle.onValueChanged.AddListener((value) =>
            {
                if (value == true) 
                    UpdateBottomUI(element);
            });
        }

        // Debug.Log("elements Count : " + elements.Count);

        InitializeElements();
    }

    private void OnEnable()
    {
        elements[0].Toggle.isOn = false;
        elements[0].Toggle.isOn = true;
    }

    private void OnDisable()
    {
        if (curElement == null)
            return;

        curElement.Toggle.isOn = false;
        curElement = null;
    }


    protected void Start()
    {
        _DataManager.UpgradeObservable
            .Subscribe(UpdateUpgrade)
            .AddTo(this.gameObject);

        upgradeButton.ClickButton(UpgradeItem, 
                    () => SystemUI.Instance.OpenNoneTouch(Values.Local_Table_Manage, Values.Local_Entry_NeedMoney));
        maxButton.onClick.AddListener(() => SystemUI.Instance.OpenNoneTouch(Values.Local_Table_Manage, Values.Local_Entry_MaxUpgrade));
    }

    private void InitializeElements()
    {
        for (int i = 0, length = elements.Count; i < length; i++)
        {
            elements[i].InitializeWithData(_DataManager.UpgradeLevels[i]);
        }
    }

    // 처음에 element data가 설정되어있지 않음
    private void UpdateBottomUI(UpgradeElement element)
    {
        if(curElement == null || element != curElement)
        {
            nameText.text = LocalizationSettings.StringDatabase.GetLocalizedString(element.Data.tableName, element.Data.nameEntry);
            upText.text = element.Data.upgradeValue.ToString();

            AudioManager.Instance.PlaySFX(ESfx.Touch);
        }

        var localizedItemValue = new LocalizedString(element.Data.tableName, element.Data.valueEntry)
            {
                {Values.Local_Name_Upgrade,
                    new UnityEngine.Localization.SmartFormat.PersistentVariables.StringVariable{Value = element.UpgradeText}}
            };
        descText.text = localizedItemValue.GetLocalizedString();

        // descText.text = string.Format(element.Data.descName, element.UpgradeText);
        valueText.text = $"{element.Level} / {element.Data.maxLevel}";
        slider.value = element.SliderValue;
        costText.text = $"{element.Cost}";

        upgradeButton.SetButton(_DataManager.Coin.Value >= element.Cost);

        upgradeButton.gameObject.SetActive(slider.value < 1f);
        maxButton.gameObject.SetActive(slider.value >= 1f);

        curElement = element;
    }

    private void UpgradeItem()
    {
        if(_DataManager.BuyUpgrade(curElement.Data, curElement.Cost))
        {
            // particle
            AudioManager.Instance.PlaySFX(ESfx.Upgrade);
            particle.Play();
        }
        else
        {
            // AudioManager.Instance.PlaySFX(ESfx.Disable);
            // cost 확인?
            Debug.Log("돈 부족 or MaxLevel");
        }
            // UpdateUpgradeBottom(curUpgradeElement);
    }

    private void UpdateUpgrade(CollectionReplaceEvent<int> element)
    {
        elements[element.Index].UpdateData(element.NewValue);
        UpdateBottomUI(elements[element.Index]);
    }
}