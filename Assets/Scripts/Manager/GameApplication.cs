using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameApplication : Singleton<GameApplication>
{
    [SerializeField] GameData gameData;

    private bool isTestMode;
    public bool IsTestMode => isTestMode;

    //private int version;

    private bool quitting = false;
    public bool Quitting => quitting;


    protected override void AwakeInstance()
    {
        isTestMode = gameData.isTestMode;

        //GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;

        //foreach (Transform child in transform)
        //{
        //    child.gameObject.SetActive(isTestMode);
        //}

        // 디바이스 로그 표시?
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
                Debug.unityLogger.logEnabled = isTestMode;
#endif
    }

    protected override void DestroyInstance() { }

    private void Start()
    {
#if UNITY_EDITOR
        Application.runInBackground = true;
#endif

        Caching.compressionEnabled = false;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        //var clickStream = this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.Escape));

        //clickStream
        //    .Buffer(clickStream.Throttle(System.TimeSpan.FromSeconds(1)))
        //    .Where(x => x.Count >= 2)
        //    .Subscribe(_ => QuitMessage());
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Escape))
    //    {
    //        QuitMessage();
    //    }
    //}

    public void LinkStore()
    {
        Application.OpenURL("market://details?id=com.ablegames.jumptab");
        // StartCoroutine(nameof(CoQuitGame));
    }

    public void Quit()
    {
        StartCoroutine(nameof(CoQuitGame));
    }

    IEnumerator CoQuitGame()
    {
        yield return new WaitForSeconds(0.4f);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void OnApplicationQuit()
    {
        quitting = true;
    }


#if UNITY_EDITOR

    [MenuItem("AbleX/Mode/Live")]
    private static void LiveMode()
    {
        GameData.Instance.isTestMode = false;
    }

    [MenuItem("AbleX/Mode/Test")]
    private static void TestMode()
    {
        GameData.Instance.isTestMode = true;
    }

    [MenuItem("AbleX/Store/Google")]
    private static void GoogleStore()
    {
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel30
            ;
        GameData.Instance.Store = EStore.Google;
    }

    [MenuItem("AbleX/Store/Onestore")]
    private static void OneStore()
    {
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel29;
        GameData.Instance.Store = EStore.Onestore;
    }

#endif
}
