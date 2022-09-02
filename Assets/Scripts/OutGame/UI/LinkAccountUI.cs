using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LinkAccountUI : PopupUI
{
    [SerializeField] GameObject[] federationObjs;
    [SerializeField] GameObject[] guestObjs;

    [SerializeField] Button googleButton;
    [SerializeField] Button facebookButton;
    [SerializeField] Button logOutButton;
    [SerializeField] Button signOutButton;


    BackendManager _Server;

    protected override void Start()
    {
        base.Start();

        if (_Server == null)
            _Server = BackendManager.Instance;

        googleButton.onClick.AddListener(() => _Server.GoogleFederation(DuplicateLogin));
        // facebookButton.onClick.AddListener(_Server.GoogleFederation);
        logOutButton.onClick.AddListener(LogOut);
        signOutButton.onClick.AddListener(SignOut);
    }

    protected override void UpdateData()
    {
        if (_Server == null)
            _Server = BackendManager.Instance;

        switch (_Server.LoginType)
        {
            case ELogin.Google:
            case ELogin.Facebook:
                foreach (var obj in federationObjs)
                {
                    obj.SetActive(true);
                }
                foreach (var obj in guestObjs)
                {
                    obj.SetActive(false);
                }
                break;
            case ELogin.Guest:
                foreach (var obj in federationObjs)
                {
                    obj.SetActive(false);
                }
                foreach (var obj in guestObjs)
                {
                    obj.SetActive(true);
                }
                break;
        }
    }

    private void LogOut()
    {
        SystemUI.Instance.OpenTwoButton(Values.Local_Entry_Message, Values.Local_Entry_Logout,
                                                    Values.Local_Entry_Confirm, Values.Local_Entry_Cancel,
            _Server.LogOut);
    }

    private void SignOut()
    {
        SystemUI.Instance.OpenTwoButton(Values.Local_Entry_Message, Values.Local_Entry_Signout,
                                                    Values.Local_Entry_Confirm, Values.Local_Entry_Cancel,
            _Server.LogOut);
    }

    private void DuplicateLogin()
    {
        SystemUI.Instance.OpenOneButton(Values.Local_Entry_Message,
                            Values.Local_Table_Setting, Values.Local_Entry_AccountExist, Values.Local_Entry_Confirm);
    }
}
