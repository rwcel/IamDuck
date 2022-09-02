using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class MailElement : MonoBehaviour
{
    private FMailInfo data;

    [BoxGroup("제목")]
    [SerializeField] TextMeshProUGUI nameText;
    [BoxGroup("제목")]
    [SerializeField] TextMeshProUGUI contentsText;

    [BoxGroup("데이터")]
    [SerializeField] Image icon;
    [BoxGroup("데이터")]
    [SerializeField] TextMeshProUGUI countText;

    [BoxGroup("받기")]
    [SerializeField] TextMeshProUGUI limitText;
    [BoxGroup("받기")]
    [SerializeField] Button recvButton;

    public System.Action OnRecv;


    private void Start()
    {
        recvButton.onClick.AddListener(RecvItem);
    }

    public void InitializeWithData(FMailInfo mailData)
    {
        data = mailData;

        SetUI();
    }

    private void SetUI()
    {
        nameText.text = data.title;
        contentsText.text = data.contents;
        icon.sprite = data.icon;
        countText.text = data.itemValue.ToString();
        countText.gameObject.SetActive(data.getItems.Length <= 1);

        // {limit}d {limit}일
        //var localizedString = new LocalizedString(Values.Local_Table_Post, Values.Local_Entry_Day)
        //{
        //    {Values.Local_Name_Limit, new IntVariable { Value = data.limit.MailRemainTime() } }
        //};
        //limitText.text = localizedString.GetLocalizedString();
        limitText.text = data.limit.MailRemainTime();
    }

    private void RecvItem()
    {
        List<FGetItem> getItems = new();
        foreach (var item in data.getItems)
        {
            getItems.Add(item);
        }

        SystemUI.Instance.OpenReward(getItems);

        // DataManager.Instance.AddItem(data.type, data.value);

        OnRecv?.Invoke();

        gameObject.SetActive(false);
    }
}
