using UnityEngine;
using BackEnd;
using static BackEnd.SendQueue;
using LitJson;
using BackEnd.GlobalSupport;
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif


public partial class BackendManager
{
    private ELogin loginType;
    public ELogin LoginType => loginType;

    private bool isProgressLogin;


    private static readonly int _Status_SignUp = 201;
    private static readonly int _Status_Login = 200;
    private static readonly int _Status_DuplicateLogin = 409;

    private static readonly string _Key_Notify = "IsNoti";


    public void LoginWithTheBackendToken(System.Action successAction, System.Action failAction)
    {
        var bro = Backend.BMember.LoginWithTheBackendToken();
        if (bro.IsServerError())
        {
            Debug.LogWarning("���� ���� �Ҿ���");
        }

        if (bro.IsSuccess())
        {
            DispatcherAction(successAction);
            OnBackendAuthorized(failAction);
        }
        else
        {
            Debug.Log("�α��� ���� - " + bro.ToString());
            DispatcherAction(failAction);
        }
    }

    public bool CheckDuplicateNickname(string name)
    {
        BackendReturnObject bro = Backend.BMember.CheckNicknameDuplication(name);
        if (FailError(bro, "�α���"))
            return false;
        
        return true;
    }

    public void GoogleLogin(System.Action loginAction, System.Action signUpAction)
    {
        if (isProgressLogin)
            return;

        signUpAction += SignUp;

        isProgressLogin = true;
        loginType = ELogin.Google;

        if (Social.localUser.authenticated)
        {
            var bro = Backend.BMember.AuthorizeFederation(GetTokens(ELogin.Google), FederationType.Google, "gpgs");
            OnBackendAuthorized();
        }
        else
        {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    var bro = Backend.BMember.AuthorizeFederation(GetTokens(ELogin.Google), FederationType.Google, "gpgs");
                    if (bro.IsSuccess())
                    {
                        if (int.Parse(bro.GetStatusCode()) == _Status_SignUp)
                        {
                            // ȸ������
                            DispatcherAction(signUpAction);
                            isProgressLogin = false;
                        }
                        else if (int.Parse(bro.GetStatusCode()) == _Status_Login)
                        {
                            // �α���
                            DispatcherAction(loginAction);
                            OnBackendAuthorized(signUpAction);
                            Debug.Log("���� �α��� ����");
                        }
                    }
                }
                else
                {
                    Debug.LogError("�α��� ����");
                    isProgressLogin = false;
                }
            });
        }
    }

    public void FacebookLogin(System.Action loginAction, System.Action signUpAction)
    {
        if (isProgressLogin)
            return;

        isProgressLogin = true;
        loginType = ELogin.Facebook;
    }

    public void GuestLogin(System.Action loginAction, System.Action signUpAction)
    {
        if (isProgressLogin)
            return;

        signUpAction += SignUp;

        isProgressLogin = true;
        loginType = ELogin.Guest;
        var bro = Backend.BMember.GuestLogin();
        if (bro.IsSuccess())
        {
            if (int.Parse(bro.GetStatusCode()) == _Status_SignUp)
            {
                // ȸ������
                DispatcherAction(signUpAction);
                isProgressLogin = false;
            }
            else if (int.Parse(bro.GetStatusCode()) == _Status_Login)
            {
                // �α���
                DispatcherAction(loginAction);
                OnBackendAuthorized(signUpAction);
                Debug.Log("�Խ�Ʈ �α��� ����");
            }
        }
        else
        {
            // **�α� ����� �Ұ��� : �α����� �ؾ� �� �ڿ� InsertLog�� ����
            Debug.LogWarning("�Խ�Ʈ �α��� ���� : " + bro);

            // ���� ����
            Backend.BMember.DeleteGuestInfo();

            // SystemPopupUI.Instance.OpenOneButton(15, 114, 2, GameSceneManager.Instance.Restart);

            isProgressLogin = false;

        }
    }

    private string GetTokens(ELogin type)
    {
        if (type == ELogin.Google)
        {
            if (PlayGamesPlatform.Instance.localUser.authenticated)
            {
                Debug.Log("��ū ID : " + PlayGamesPlatform.Instance.GetIdToken());
                return PlayGamesPlatform.Instance.GetIdToken();
            }
            else
            {
                Debug.LogError("���ӵǾ����� �ʽ��ϴ�!");
                return null;
            }
        }
        //else if (type == ELogin.Facebook)
        //{
        //    var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
        //    //string facebookToken = aToken.TokenString;

        //    return aToken.TokenString;
        //}

        return null;
    }

    public void SignUp()
    {
        Backend.BMember.UpdateCountryCode(GetLanguageCountry(), countryBro =>
        {
            // Ȯ��?
            Debug.Log("��� ���� : " + GetLanguageCountry());
        });
    }

    public void LoginWithToken()
    {
        var bro = Backend.BMember.LoginWithTheBackendToken();
        if (bro.IsServerError())
        {
            Debug.LogWarning("���� ���� �Ҿ���");
        }

        if (FailError(bro, "�α���"))
            return;

        OnBackendAuthorized();
    }

    /// <summary>
    /// �Ϸ� �� ����
    /// </summary>
    public void CreateNickname(string nickname)
    {
        Backend.BMember.CreateNickname(nickname, bro =>
        {
            if (FailError(bro, "�г��� ����"))
                return;

            OnBackendAuthorized();
        });
    }

    public void UpdateNickname(string _newNickName, System.Action successAction)
    {
        var before = nicknameRP.Value;
        Backend.BMember.UpdateNickname(_newNickName, bro =>
        {
            if (FailError(bro, "�г��� ����"))
                return;

            nicknameRP.Value = _newNickName;

            DispatcherAction(successAction);
        });
    }

    /// <summary>
    /// ���� ���� ��������
    /// </summary>
    public void OnBackendAuthorized(System.Action failAction = null)
    {
        Backend.BMember.GetUserInfo(bro =>
        {
            if (FailError(bro, "�г��� ����"))
                return;

            JsonData Userdata = bro.GetReturnValuetoJSON()["row"];

            var subscriptionType = Userdata["subscriptionType"].ToString();
            if (subscriptionType == "google")
            {
                loginType = ELogin.Google;
            }
            else if (subscriptionType == "facebook")
            {
                loginType = ELogin.Facebook;
            }
            else if (subscriptionType == "customSignUp")
            {
                loginType = ELogin.Guest;
            }

            JsonData nicknameJson = Userdata["nickname"];
            if (nicknameJson != null)
            {
                nicknameRP.Value = nicknameJson.ToString();
                gamerID = Userdata["gamerId"].ToString();

                isProgressLogin = false;

                GameDataLoad();
            }
            else
            {
                Debug.Log("Not Nickname");

                failAction?.Invoke();
            }
        });
    }

    void GameDataLoad()
    {
        Debug.Log("���� ������ �ε��ϱ�");

        if (PrefsHasKey(_Key_Notify))
        {
            isNoti = PrefsGetInt(_Key_Notify) == 0 ? false : true;
            // Debug.Log($"�˸� : {isNoti}");
        }

        AudioManager.Instance.LoadData();


        GetChartLists();
        GetGameDatas();
    }

    public void SetPushNotification(bool value)
    {
        isNoti = value;
        PrefsSetInt(_Key_Notify, value ? 1 : 0);
        // Debug.Log($"�˸� : {isNoti}");

#if !UNITY_EDITOR
        BackendReturnObject bro;
        if(isNoti)
            bro = Backend.Android.PutDeviceToken();
        else
            bro = Backend.Android.DeleteDeviceToken();

        if (FailError(bro))
            return;

        Debug.Log($"Ǫ�� ���� : {value}");
#endif

        if (isNoti)
            SystemUI.Instance.OpenNoneTouch(Values.Local_Table_Setting, Values.Local_Entry_AllowPush);
    }

    public void GoogleFederation(System.Action duplicateAction)
    {
#if UNITY_EDITOR
        return;
#endif

        if (isProgressLogin)
            return;

        isProgressLogin = true;

        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                var bro = Backend.BMember.ChangeCustomToFederation(GetTokens(ELogin.Google), FederationType.Google);
                if (bro.IsSuccess())
                {
                    GamePopup.Instance.ClosePopup();
                    loginType = ELogin.Google;
                    isProgressLogin = false;
                }
                else
                {
                    if (int.Parse(bro.GetStatusCode()) == _Status_DuplicateLogin)
                    {
                        duplicateAction?.Invoke();
                        isProgressLogin = false;
                    }
                    else
                    {
                        Debug.Log("��ȯ���� : " + bro);
                        isProgressLogin = false;
                    }
                }
            }
            else
            {
                Debug.LogError("�α��� ����");
                // SystemPopupUI.Instance.OpenNoneTouch(50);
                isProgressLogin = false;
            }
        });
    }

    public void LogOut()
    {
        var bro = Backend.BMember.Logout();
        if (bro.IsSuccess())
        {
            Debug.Log("�α׾ƿ�");
            GameSceneManager.Instance.Restart();
        }
    }

    public void SignOut()
    {
        var bro = Backend.BMember.SignOut();
        if (bro.IsSuccess())
        {
            Debug.Log("Ż��Ϸ�");

            GameSceneManager.Instance.Restart();
        }
    }

    public CountryCode GetLanguageCountry()
    {
        var systemLanguage = Application.systemLanguage;
        switch (systemLanguage)
        {
            case SystemLanguage.Chinese:
                return CountryCode.China;
            case SystemLanguage.English:
                return CountryCode.UnitedKingdom;
            case SystemLanguage.French:
                return CountryCode.France;
            case SystemLanguage.German:
                return CountryCode.Germany;
            case SystemLanguage.Italian:
                return CountryCode.Italy;
            case SystemLanguage.Japanese:
                return CountryCode.Japan;
            case SystemLanguage.Korean:
                return CountryCode.SouthKorea;
            case SystemLanguage.Polish:
                return CountryCode.Poland;
            case SystemLanguage.Spanish:
                return CountryCode.Spain;
            case SystemLanguage.Thai:
                return CountryCode.Thailand;
            case SystemLanguage.Vietnamese:
                return CountryCode.VietNam;
            case SystemLanguage.Unknown:
                return CountryCode.UnitedStates;
            default:
                return CountryCode.UnitedStates;
        }
    }
}