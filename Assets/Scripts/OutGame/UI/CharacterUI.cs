using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using Sirenix.OdinInspector;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class CharacterUI : MonoBehaviour
{
    [BoxGroup("Bottom")]
    [SerializeField] Transform elementParent;
    protected List<CharacterElement> elements;

    [BoxGroup("Top")]
    [SerializeField] TextMeshProUGUI nameText;
    // [BoxGroup("Top")]
    // [SerializeField] GameObject descObj;                // 해금조건 obj : 무조건 보여주기
    [BoxGroup("Top")]
    [SerializeField] TextMeshProUGUI descText;

    [BoxGroup("Middle")]
    [SerializeField] Slider valueSlider;
    [BoxGroup("Middle")]
    [SerializeField] TextMeshProUGUI sliderText;

    [BoxGroup("Middle")]
    [SerializeField] Button leftButton;
    [BoxGroup("Middle")]
    [SerializeField] Button rightButton;

    [BoxGroup("스크롤")]
    [SerializeField] RectTransform contentRectTr;
    [BoxGroup("스크롤")]
    [SerializeField] GridLayoutGroup layoutGroup;

    [BoxGroup("Bottom")]
    [SerializeField] OnOffButton unlockButton;
    [BoxGroup("Bottom")]
    [SerializeField] OnOffButton equipButton;

    public System.Action<int> OnEquip;

    private readonly ReactiveProperty<int> notifyRP = new IntReactiveProperty(0);
    public ReadOnlyReactiveProperty<int> Notify => notifyRP.ToReadOnlyReactiveProperty();

    // **연출 오브젝트?

    DataManager _DataManager;
    GameData _GameData;

    private int selectIdx = -1;
    private int equipIdx = 0;

    private int elementCount;
    private int rowCount;

    private static readonly float _Character_PerY = 150f;


    //protected void Awake()
    public void OnAwake(int value)
    {
        equipIdx = value;

        if (_DataManager == null)
            _DataManager = DataManager.Instance;
        if (_GameData == null)
            _GameData = GameData.Instance;

        elementCount = elementParent.childCount;
        elements = new List<CharacterElement>(elementCount);

        for (int i = 0, length = elementCount; i < length; i++)
        {
            int idx = i;
            var element = elementParent.GetChild(i).GetComponent<CharacterElement>();
            elements.Add(element);
            element.Toggle.onValueChanged.AddListener((value) =>
            {
                if (value == true)
                {
                    if(element.UpdateReady)
                    {
                        SystemUI.Instance.OpenNoneTouch(Values.Local_Table_Character, Values.Local_Entry_Update);
                        elements[equipIdx].Toggle.isOn = true;
                    }
                    else
                    {
                        SelectCharacter(idx);
                    }
                }
            });
        }

        elements[elementCount - 1].Toggle.isOn = false;

        GetRowCount();
    }

    private void OnEnable()
    {
        // *0번을 안끄면 다른 게 안켜짐
        elements[0].Toggle.isOn = false;

        if (equipIdx != -1)
        {
            //SelectCharacter(equipIdx);
            elements[equipIdx].Toggle.isOn = false;
            elements[equipIdx].Toggle.isOn = true;
        }
        else
        {
            elements[0].Toggle.isOn = true;
        }
    }

    /// <summary>
    /// **게임 종료 에러
    /// </summary>
    private void OnDisable()
    {
        if(selectIdx != equipIdx)
        {   // 되돌리기
            elements[selectIdx].Toggle.isOn = false;
            elements[equipIdx].Toggle.isOn = true;
        }

        selectIdx = -1;
    }

    void GetRowCount()
    {
        float firstY = elements[0].transform.position.y;
        
        foreach (var element in elements)
        {
            if (element.transform.position.y != firstY)
                break;

            rowCount++;
        }
    }

    protected void Start()
    {
        leftButton.onClick.AddListener(() => ChangeCharacter(-1));
        rightButton.onClick.AddListener(() => ChangeCharacter(1));
        unlockButton.ClickButton(Unlock, () => SystemUI.Instance.OpenNoneTouch(Values.Local_Table_Character, Values.Local_Entry_FailUnlock));
        equipButton.ClickButton(Equip, () => SystemUI.Instance.OpenNoneTouch(Values.Local_Table_Character, Values.Local_Entry_AlreadyEquip));
    }

    public void InitializeElements(Transform modelParent)
    {
        int idx = 0;
        foreach (var element in elements)
        {
            element.InitializeWithData(idx, _DataManager.UnlockCharacters[idx]);
            if (element.UpdateReady)
            {
                idx++;
                continue;
            }

            var modelObj = modelParent.GetChild(idx).gameObject;
            element.Toggle.onValueChanged.AddListener((value)
                                            => modelObj.SetActive(value));
            modelObj.SetActive(idx == equipIdx);

            if (element.IsLock)
            {
                element.OnChangeCanUnlock += (value) => notifyRP.Value += value ? 1 : -1;
                if(element.CanUnlock)
                {
                    notifyRP.Value++;
                }
            }
            idx++;
        }

        Debug.Log($"new character : {notifyRP.Value}");
    }

    private void SelectCharacter(int idx  = -1)
    {
        var element = elements[idx];
        if (selectIdx == -1 || idx != selectIdx)
        {
            // 조건달기
            var localizedItemValue = new LocalizedString(Values.Local_Table_Character, element.Data.unlockEntry)
            {
                {Values.Local_Name_Unlock,
                    new UnityEngine.Localization.SmartFormat.PersistentVariables.IntVariable{Value = element.Data.value}}
            };

            nameText.text = LocalizationSettings.StringDatabase.GetLocalizedString(Values.Local_Table_Character, element.Data.nameEntry);
            descText.text = localizedItemValue.GetLocalizedString();
        }

        SetLockObjects(element.IsLock);

        unlockButton.SetButton(element.CanUnlock);
        unlockButton.gameObject.SetActive(element.IsLock);
        equipButton.SetButton((idx != equipIdx) && !element.IsLock);

        valueSlider.value = (float)element.CurValue / element.Data.value;
        sliderText.text = valueSlider.value >= 1f ? $"{element.Data.value}/{element.Data.value}"
                                                             : $"{element.CurValue}/{element.Data.value}";

        MoveScroll(idx);

        selectIdx = idx;

        AudioManager.Instance.PlaySFX(ESfx.Touch);
    }

    void MoveScroll(int idx)
    {
        int row = (idx / rowCount);
        contentRectTr.anchoredPosition = (row == 0) ? Vector2.zero 
                                                    : Vector2.up * (row * 2 * _Character_PerY - _Character_PerY);

        // Debug.Log(Vector2.up * (row * 2 * _Character_PerY - _Character_PerY));
    }

    private void ChangeCharacter(int value)
    {
        AudioManager.Instance.PlaySFX(ESfx.Touch);

        int result = selectIdx + value;
        if (result >= elementCount)
        {
            result = 0;
        }
        else if(result < 0)
        {
            result = elementCount - 1;
        }

        elements[selectIdx].Toggle.isOn = false;
        elements[result].Toggle.isOn = true;
    }

    private void SetLockObjects(bool isLock)
    {
        // descObj.SetActive(isLock);
        valueSlider.gameObject.SetActive(isLock);
    }

    private void Unlock()
    {
        // 해금 조건 만족?
        unlockButton.gameObject.SetActive(false);
        equipButton.SetButton(true);

        SetLockObjects(false);

        // 연출? + 서버
        elements[selectIdx].Unlock();
        _DataManager.UnlockCharacter(selectIdx);

        UnlockUI.SetCharacterData(selectIdx);
        GamePopup.Instance.OpenUnlock();
    }

    /// <summary>
    /// 장착 시 홈으로 이동하고 데이터 서버에 저장
    /// </summary>
    private void Equip()
    {
        // 서버 프로필로 저장 : 나갔을 때?
        equipIdx = selectIdx;

        OnEquip?.Invoke(equipIdx);
        // OnDisable();
    }


    private void UnlockElement(CollectionReplaceEvent<int> element)
    {
        // elements[element.Index].UpdateCount(element.NewValue);
        // UpdateSlots(elements[element.Index]);
    }
}
