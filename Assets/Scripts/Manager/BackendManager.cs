using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using static BackEnd.SendQueue;
using LitJson;
using UniRx;
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

public partial class BackendManager : Singleton<BackendManager>
{
    GameData _GameData;

    private string gamerID = "";                        // UUID
    public string UUID => gamerID;

    private bool isNoti;
    public bool IsNoti => isNoti;

    private readonly ReactiveProperty<string> nicknameRP = new StringReactiveProperty("");
    public ReadOnlyReactiveProperty<string> Nickname => nicknameRP.ToReadOnlyReactiveProperty();

    private static readonly int _Status_Maintenance = 401;
    private static readonly string _Msg_Maintenance = "bad serverStatus: maintenance";

    private static readonly int _Enum_Update = 2;

    private string language = "KO";


    protected override void AwakeInstance()
    {

    }

    protected override void DestroyInstance()
    {

    }

    private void Start()
    {
        //if (Application.internetReachability == NetworkReachability.NotReachable)
        //{   // 서버 접속 불가
        //    SystemUI.Instance.OpenOneButton
        //            (Values.Local_Entry_Maintainance_Title, Values.Local_Entry_Maintainance_Content, Values.Local_Entry_Confirm, GameApplication.Instance.Quit);
        //    return;
        //}

#if UNITY_ANDROID
        // Google
        var config = new PlayGamesClientConfiguration
            .Builder()
            .RequestServerAuthCode(false)
            .RequestEmail()                     // 이메일 권한
            .RequestIdToken()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;           // 디버그로그 확인
        PlayGamesPlatform.Activate();
#endif
#if !UNITY_EDITOR
        // FB
#endif

        var bro = Backend.Initialize(true);
        if (FailError(bro))
            return;

        if (!string.IsNullOrEmpty(Backend.Utils.GetGoogleHash()))
            Debug.Log("HashKey : " + Backend.Utils.GetGoogleHash());

        //var serverTime = Backend.Utils.GetServerTime();

        CheckApplicationVersion();

        _GameData = GameData.Instance;
        _DataManager = DataManager.Instance;
    }

    private void Update()
    {
        Dispatcher.Instance.InvokePending();
        if (Backend.IsInitialized)
        {
            try
            {
                Backend.AsyncPoll();
            }
            catch
            {
                SystemUI.Instance.OpenOneButton(
                    Values.Local_Entry_Message, Values.Local_Entry_ServerError, Values.Local_Entry_Confirm, GameApplication.Instance.Quit);
            }
        }
    }

    // Dispatcer에서 action 실행 (메인스레드)
    private void DispatcherAction(System.Action action)
    {
        if(action != null)
            Dispatcher.Instance.Invoke(action);
    }

    private void CheckApplicationVersion()
    {
        Backend.Utils.GetLatestVersion(versionBRO =>
        {
            if(versionBRO.IsMaintenanceError()
            || versionBRO.IsServerError())
            {
                // 서버 점검
                SystemUI.Instance.OpenOneButton(
                    Values.Local_Entry_Maintainance_Title, Values.Local_Entry_Maintainance_Content, 
                    Values.Local_Entry_Confirm, GameApplication.Instance.Quit);
                return;
            }
            if (versionBRO.IsSuccess())         // 에디터에서는 안됨
            {
                string latest = versionBRO.GetReturnValuetoJSON()["version"].ToString();
                Debug.Log("version info - current: " + Application.version + " latest: " + latest);

                if (!IsLatestVersion(Application.version, latest))
                {
                    Debug.Log($"버전 낮음 : {(int)versionBRO.GetReturnValuetoJSON()["type"] }");
                    // type = 1 : 선택, type = 2 : 강제
                    if ((int)versionBRO.GetReturnValuetoJSON()["type"] == _Enum_Update)
                    {
                        SystemUI.Instance.OpenOneButton(
                            Values.Local_Entry_NeedUpdate_Title, Values.Local_Entry_NeedUpdate_Content, Values.Local_Entry_Update, GameApplication.Instance.LinkStore);
                    }
                }
            }
        });
    }

    private bool RefreshTheBackendToken(int maxRepeatCount)
    {
        if (maxRepeatCount <= 0)
            return false;

        var callback = Backend.BMember.RefreshTheBackendToken();
        if (callback.IsSuccess())
            return true;

        if (callback.IsClientRequestFailError()) // 클라이언트의 일시적인 네트워크 끊김 시
        {
            return RefreshTheBackendToken(maxRepeatCount - 1);
        }
        else if (callback.IsServerError()) // 서버의 이상 발생 시
        {
            return RefreshTheBackendToken(maxRepeatCount - 1);
        }
        else if (callback.IsMaintenanceError()) // 서버 상태가 '점검'일 시
        {
            //점검 팝업창 + 로그인 화면으로 보내기
            return false;
        }
        else if (callback.IsTooManyRequestError()) // 단기간에 많은 요청을 보낼 경우 발생하는 403 Forbbiden 발생 시
        {
            //너무 많은 요청을 보내는 중 
            return false;
        }
        else
        {
            //재시도를 해도 액세스토큰 재발급이 불가능한 경우
            //커스텀 로그인 혹은 페데레이션 로그인을 통해 수동 로그인을 진행해야합니다.
            //중복 로그인일 경우 401 bad refreshToken 에러와 함께 발생할 수 있습니다.
            Debug.Log("게임 접속에 문제가 발생했습니다. 로그인 화면으로 돌아갑니다\n" + callback.ToString());
            return false;
        }
    }

    private bool IsLatestVersion(string gameVersion, string serverVersion)
    {
        int gameVer = 0, serverVer = 0;

        var split = gameVersion.Split('.');
        gameVer += int.Parse(split[0]) * 10000;
        gameVer += int.Parse(split[1]) * 100;
        gameVer += int.Parse(split[2]);

        split = serverVersion.Split('.');
        serverVer += int.Parse(split[0]) * 10000;
        serverVer += int.Parse(split[1]) * 100;
        serverVer += int.Parse(split[2]);

        return gameVer >= serverVer;
    }


    private bool FailError(BackendReturnObject bro, string text = null)
    {
        if (!bro.IsSuccess())
        {
            Debug.LogWarning($"{text} : {bro}");
            return true;
        }
        return false;
    }

}
