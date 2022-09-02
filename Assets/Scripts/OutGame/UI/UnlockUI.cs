using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.UI;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UniRx;
using UnityEngine.Localization.Settings;

public class UnlockUI : PopupUI
{
    [SerializeField] Image background;
    [SerializeField] TextMeshProUGUI descText;
    [SerializeField] Transform modelParent;

    //private CharacterData data;             // 해금 오리 정보
    private GameObject modelObj;

    private static CharacterData data;

    //protected override void Awake()
    //{
    //    base.Awake();

    //    // 자동실행..?
    //    DataManager.Instance.CharacterObservable
    //        .Subscribe(SetCharacterData);
    //}

    //private void SetCharacterData(CollectionReplaceEvent<bool> element)
    //{
    //    data = GameData.Instance.CharacterDatas[element.Index];
    //    // OpenUI
    //}

    public static void SetCharacterData(int idx)
    {
        data = GameData.Instance.CharacterDatas[idx];
    }

    protected override void UpdateData()
    {
        AudioManager.Instance.PlaySFX(ESfx.Unlock);

        Debug.Assert(data != null, "해금 캐릭터 정보가 없습니다");
        // data = ;
        modelObj = Instantiate(data.lobbyObj, modelParent);

        var localizedString = new LocalizedString(Values.Local_Table_Character, Values.Local_Entry_CharDesc)
        {
            {Values.Local_Name_CharDesc, new StringVariable 
                        { Value = LocalizationSettings.StringDatabase.GetLocalizedString(Values.Local_Table_Character, data.nameEntry) } }
        };

        descText.text = localizedString.GetLocalizedString();

        background.color = data.themeColor;
    }

    private void OnDisable()
    {
        modelObj.SetActive(false);
        data = null;
    }


}
