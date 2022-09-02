using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public partial class DataManager
{
    private System.IDisposable[] disposables;
    private readonly ReactiveDictionary<EAds, int> adTimerRD = new ReactiveDictionary<EAds, int>();
    public System.IObservable<DictionaryReplaceEvent<EAds, int>> adTimerObservable => adTimerRD.ObserveReplace();
    public Dictionary<EAds, int> AdTimers => adTimerRD.ToDictionary(pair => pair.Key, pair => pair.Value);
    public System.Action<EAds, int> OnAdTimer;

    private readonly ReactiveDictionary<EAds, int> adCountRD = new ReactiveDictionary<EAds, int>();
    public System.IObservable<DictionaryReplaceEvent<EAds, int>> adCountObservable => adCountRD.ObserveReplace();
    public Dictionary<EAds, int> AdCounts => adCountRD.ToDictionary(pair => pair.Key, pair => pair.Value);

    private System.IDisposable interstitialDisposable;
    private System.IDisposable timerDisposable;
    public int interstitialTime = 0;

    private static readonly string _Key_DateOfExit = "DateOfExit";
    private static readonly string _Key_Interstitial = "InterstitialTime";
    private static readonly int adMaxDelay = 180;
    private static readonly int interstitialDelay = 80;

    List<Coroutine> coroutines;

    public void InitializeAdCounts(Dictionary<EAds, int> items)
    {
        GameSceneManager.Instance.RestartAction += ClearAd;

        // coroutines = new List<Coroutine>();
        disposables = new System.IDisposable[System.Enum.GetValues(typeof(EAds)).Length];
        adCountRD.Clear();
        adTimerRD.Clear();
        if (timerDisposable != null)
            timerDisposable.Dispose();
        
        OnAdTimer = null;

        foreach (var item in items)
        {
            adCountRD.Add(item.Key, item.Value);
        }

        CheckAdsTimer();

        timerDisposable = adTimerObservable
            .Subscribe(item => OnAdTimer?.Invoke(item.Key, item.NewValue))
            .AddTo(this.gameObject);

        adCountObservable
            .Subscribe(item => _Server.UpdateAdsItem(item.Key, item.NewValue))
            .AddTo(this.gameObject);

    }

    void ClearAd()
    {
        Debug.Log($"���� �ʱ�ȭ");

        if (timerDisposable != null)
        {
            Debug.Log("Dispose : time");
            timerDisposable.Dispose();
        }

        if(disposables != null)
        {
            for (int i = 0, length = disposables.Length; i < length; i++)
            {
                if (disposables[i] != null)
                {
                    Debug.Log($"Dispose : {((EAds)i).ToString()}");
                    disposables[i].Dispose();
                }
                else
                {
                    Debug.Log($"Dispose is null : {((EAds)i).ToString()}");
                }
            }
        }
        
        if(coroutines != null)
        {
            for (int i = 0, length  = coroutines.Count; i < length; i++)
            {
                if (coroutines[i] != null)
                    StopCoroutine(coroutines[i]);
            }
        }

        OnApplicationPause(true);

        GameSceneManager.Instance.RestartAction -= ClearAd;
    }

    /// <summary>
    /// ������ȯ �� Ȯ�� �ʿ�
    /// </summary>
    void CheckAdsTimer()
    {
        // �ʱ�
        if (!_Server.PrefsHasKey(_Key_DateOfExit))
        {
            _Server.PrefsSetString(_Key_DateOfExit, System.DateTime.Now.ToString());
            foreach (EAds type in System.Enum.GetValues(typeof(EAds)))
            {
                adTimerRD.Add(type, 0);
            }
        }
        else
        {
            var dateOfExit = System.DateTime.Parse(_Server.PrefsGetString(_Key_DateOfExit));
            double totalSeconds = System.DateTime.Now.Subtract(dateOfExit).TotalSeconds;

            _Server.PrefsSetString(_Key_DateOfExit, System.DateTime.Now.ToString());
            foreach (EAds type in System.Enum.GetValues(typeof(EAds)))
            {
                Debug.Log($"���� {type} : {_Server.PrefsGetInt(type.ToString())} - {(int)totalSeconds}");
                var value = Mathf.Clamp(_Server.PrefsGetInt(type.ToString()) - (int)totalSeconds, 0, adMaxDelay);
                adTimerRD.Add(type, value);
                if(value > 0)
                {
                    StartCooldown(type, value);
                }
                else if (disposables[(int)type] != null)
                {
                    Debug.Log("Dispose");
                    disposables[(int)type].Dispose();
                }
            }

            var adTime = Mathf.Clamp(_Server.PrefsGetInt(_Key_Interstitial) - (int)totalSeconds, 0, interstitialDelay);
            StartCooldown(adTime);
        }
    }

    public void SetAdCountAndTime(EAds type)
    {
        // ī��Ʈ 
        adCountRD[type]--;

        // �ð���
        StartCooldown(type, adMaxDelay);
    }

    public void StartCooldown(EAds type, int value)
    {
        adTimerRD[type] = value;
        if (value <= 0)
            return;

        if (disposables[(int)type] != null)
        {
            Debug.Log("Dispose - cooldown");
            disposables[(int)type].Dispose();
        }

        disposables[(int)type] =
            Observable.FromCoroutine<int>(observer => CoTimerObserver(observer, value))
                .Subscribe(value => adTimerRD[type] = value)
                .AddTo(gameObject);

        //var coroutine = StartCoroutine(CoAdTimer(type, value));
        //coroutines.Add(coroutine);
    }

    //
    public void StartCooldown(int value = -1)
    {
        if (interstitialDisposable != null)
        {
            interstitialDisposable.Dispose();
        }
        if (value < 0)
            value = interstitialDelay;

        Debug.Log($"���鱤�� �ð� : {value}");

        interstitialDisposable =
            Observable.FromCoroutine<int>(observer => CoTimerObserver(observer, value))
                .Subscribe(value => interstitialTime = value)
                .AddTo(gameObject);
    }

    public bool AdUseItem(EAds type, System.Action onSuccess)
    {
        if (adCountRD[type] <= 0)
            return false;

        onSuccess += () =>
        {
            useItemRC[(int)type]++;
            SetAdCountAndTime(type);
        };

        // Reward���� 
        UnityAdsManager.Instance.ShowRewardAD(onSuccess, null, type);

        return true;
    }


    IEnumerator CoTimerObserver(System.IObserver<int> observer, int delay)
    {
        while (delay > 0)
        {
            Debug.Log($"{delay}");
            observer.OnNext(delay--);
            yield return Values.Delay1;
        }
        observer.OnNext(0);
        observer.OnCompleted();
    }


    IEnumerator CoAdTimer(EAds type, int value)
    {
        var result = value;
        while(result > 0)
        {
            yield return Values.DelayReal1;
            adTimerRD[type] = --result;
        }
        adTimerRD[type] = 0;
    }


    private void OnApplicationQuit()
    {
        OnApplicationPause(true);
    }

#if UNITY_EDITOR
    private void OnApplicationFocus(bool focus)
    {
        OnApplicationPause(!focus);
    }
#endif

    /// <summary>
    /// �� üũ �ʿ� : Outgame, InGame �ʿ�����
    /// </summary>
    /// <param name="pause"></param>
    private void OnApplicationPause(bool pause)
    {
        Debug.Log($"�������� ���� : {pause}");
        if(pause)
        {
            _Server.PrefsSetString(_Key_DateOfExit, System.DateTime.Now.ToString());
            _Server.PrefsSetInt(_Key_Interstitial, interstitialTime);
            foreach (var adTimer in adTimerRD)
            {
                _Server.PrefsSetInt(adTimer.Key.ToString(), adTimer.Value);
            }

            PlayerPrefs.Save();
        }
        else
        {
            if (!_Server.PrefsHasKey(_Key_DateOfExit) || adTimerRD.Count <= 0)
                return;

            var dateOfExit = System.DateTime.Parse(_Server.PrefsGetString(_Key_DateOfExit));
            double totalSeconds = System.DateTime.Now.Subtract(dateOfExit).TotalSeconds;
            if (totalSeconds > adMaxDelay)
                totalSeconds = adMaxDelay;              // double -> int ����ȯ ���� ���� ��� ������

            Debug.Log(totalSeconds);

            foreach (EAds type in System.Enum.GetValues(typeof(EAds)))
            {
                if(adTimerRD[type] > 0)
                {
                    StartCooldown(type, Mathf.Clamp(adTimerRD[type] - (int)totalSeconds, 0, adMaxDelay));
                }
            }
            StartCooldown(Mathf.Clamp(_Server.PrefsGetInt(_Key_Interstitial) - (int)totalSeconds, 0, interstitialDelay));
        }
    }
}
