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
        //{   // ���� ���� �Ұ�
        //    SystemUI.Instance.OpenOneButton
        //            (Values.Local_Entry_Maintainance_Title, Values.Local_Entry_Maintainance_Content, Values.Local_Entry_Confirm, GameApplication.Instance.Quit);
        //    return;
        //}

#if UNITY_ANDROID
        // Google
        var config = new PlayGamesClientConfiguration
            .Builder()
            .RequestServerAuthCode(false)
            .RequestEmail()                     // �̸��� ����
            .RequestIdToken()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;           // ����׷α� Ȯ��
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

    // Dispatcer���� action ���� (���ν�����)
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
                // ���� ����
                SystemUI.Instance.OpenOneButton(
                    Values.Local_Entry_Maintainance_Title, Values.Local_Entry_Maintainance_Content, 
                    Values.Local_Entry_Confirm, GameApplication.Instance.Quit);
                return;
            }
            if (versionBRO.IsSuccess())         // �����Ϳ����� �ȵ�
            {
                string latest = versionBRO.GetReturnValuetoJSON()["version"].ToString();
                Debug.Log("version info - current: " + Application.version + " latest: " + latest);

                if (!IsLatestVersion(Application.version, latest))
                {
                    Debug.Log($"���� ���� : {(int)versionBRO.GetReturnValuetoJSON()["type"] }");
                    // type = 1 : ����, type = 2 : ����
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

        if (callback.IsClientRequestFailError()) // Ŭ���̾�Ʈ�� �Ͻ����� ��Ʈ��ũ ���� ��
        {
            return RefreshTheBackendToken(maxRepeatCount - 1);
        }
        else if (callback.IsServerError()) // ������ �̻� �߻� ��
        {
            return RefreshTheBackendToken(maxRepeatCount - 1);
        }
        else if (callback.IsMaintenanceError()) // ���� ���°� '����'�� ��
        {
            //���� �˾�â + �α��� ȭ������ ������
            return false;
        }
        else if (callback.IsTooManyRequestError()) // �ܱⰣ�� ���� ��û�� ���� ��� �߻��ϴ� 403 Forbbiden �߻� ��
        {
            //�ʹ� ���� ��û�� ������ �� 
            return false;
        }
        else
        {
            //��õ��� �ص� �׼�����ū ��߱��� �Ұ����� ���
            //Ŀ���� �α��� Ȥ�� �䵥���̼� �α����� ���� ���� �α����� �����ؾ��մϴ�.
            //�ߺ� �α����� ��� 401 bad refreshToken ������ �Բ� �߻��� �� �ֽ��ϴ�.
            Debug.Log("���� ���ӿ� ������ �߻��߽��ϴ�. �α��� ȭ������ ���ư��ϴ�\n" + callback.ToString());
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
