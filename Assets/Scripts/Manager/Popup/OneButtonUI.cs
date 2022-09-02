using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class OneButtonUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI contentsText;
    [SerializeField] TextMeshProUGUI buttonText;
    [SerializeField] Button button;

    private Animator anim;

    System.Action buttonAction;


    private void Awake()
    {
        anim = GetComponent<Animator>();

        button.onClick.AddListener(OnConfirm);
    }

    public void OpenWithData(string _titleEntry, string _contentEntry, string _buttonEntry, System.Action _buttonAction = null)
    {
        var database = LocalizationSettings.StringDatabase;
        titleText.text = database.GetLocalizedString(Values.Local_Table_Common, _titleEntry);
        contentsText.text = database.GetLocalizedString(Values.Local_Table_Common, _contentEntry);
        buttonText.text = database.GetLocalizedString(Values.Local_Table_Common, _buttonEntry);

        buttonAction = _buttonAction;

        gameObject.SetActive(true);
    }

    public void OpenWithData(string _titleEntry, string _contentTable, string _contentEntry, string _buttonEntry, System.Action _buttonAction = null)
    {
        var database = LocalizationSettings.StringDatabase;
        titleText.text = database.GetLocalizedString(Values.Local_Table_Common, _titleEntry);
        contentsText.text = database.GetLocalizedString(_contentTable, _contentEntry);
        buttonText.text = database.GetLocalizedString(Values.Local_Table_Common, _buttonEntry);

        buttonAction = _buttonAction;

        gameObject.SetActive(true);
    }

    void OnConfirm()
    {
        // AudioManager.Instance.PlaySFX(ESFX.Touch);

        buttonAction?.Invoke();

        anim.SetTrigger(Values._Anim_Close);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
