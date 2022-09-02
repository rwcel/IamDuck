using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class TwoButtonUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI contentsText;
    [SerializeField] TextMeshProUGUI leftButtonText;
    [SerializeField] TextMeshProUGUI rightButtonText;
    [SerializeField] Button leftButton;
    [SerializeField] Button rightButton;

    private Animator anim;

    System.Action leftButtonAction;
    System.Action rightButtonAction;


    private void Awake()
    {
        anim = GetComponent<Animator>();

        leftButton.onClick.AddListener(() => OnConfirm(leftButtonAction));
        rightButton.onClick.AddListener(() => OnConfirm(rightButtonAction));
    }

    public void OpenWithData(string _titleEntry, string _contentEntry, string _leftBtnEntry, string _rightBtnEntry,
                                        System.Action _leftButtonAction = null, System.Action _rightButtonAction = null)
    {
        var database = LocalizationSettings.StringDatabase;
        titleText.text = database.GetLocalizedString(Values.Local_Table_Common, _titleEntry);
        contentsText.text = database.GetLocalizedString(Values.Local_Table_Common, _contentEntry);
        leftButtonText.text = database.GetLocalizedString(Values.Local_Table_Common, _leftBtnEntry);
        rightButtonText.text = database.GetLocalizedString(Values.Local_Table_Common, _rightBtnEntry);

        leftButtonAction = _leftButtonAction;
        rightButtonAction = _rightButtonAction;

        gameObject.SetActive(true);
    }

    // content 테이블만 다른 경우
    public void OpenWithData(string _titleEntry, string _contentTable, string _contentEntry, string _leftBtnEntry, string _rightBtnEntry,
                                    System.Action _leftButtonAction = null, System.Action _rightButtonAction = null)
    {
        var database = LocalizationSettings.StringDatabase;
        titleText.text = database.GetLocalizedString(Values.Local_Table_Common, _titleEntry);
        contentsText.text = database.GetLocalizedString(_contentTable, _contentEntry);
        leftButtonText.text = database.GetLocalizedString(Values.Local_Table_Common, _leftBtnEntry);
        rightButtonText.text = database.GetLocalizedString(Values.Local_Table_Common, _rightBtnEntry);

        leftButtonAction = _leftButtonAction;
        rightButtonAction = _rightButtonAction;

        gameObject.SetActive(true);
    }

    void OnConfirm(System.Action buttonAction)
    {
        //AudioManager.Instance.PlaySFX(ESFX.Touch);

        buttonAction?.Invoke();

        anim.SetTrigger(Values._Anim_Close);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
