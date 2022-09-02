using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdsRewarded : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    string unitId = null;

    private Action OnReward = null;
    private Action OnFail = null;


    public void LoadAd(string unitId)
    {
        this.unitId = unitId;
        Advertisement.Load(unitId, this);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log($"������ ���� �ε� ���� : {placementId}");
    }

    public void ShowAd(Action onReward, Action onFail)
    {
        OnReward = onReward;
        OnFail = onFail;

        Advertisement.Show(unitId, this);

//#if UNITY_EDITOR
//        OnReward?.Invoke();
//        OnClose?.Invoke();
//#else
//        Advertisement.Show(unitId, this);
//#endif
    }

    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(unitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("���� ���� �Ϸ�");
            OnReward?.Invoke();
        }
        else
        {
            Debug.Log("���� ������ �Ⱥ�?");
            OnFail?.Invoke();
        }
        Advertisement.Load(unitId, this);
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"������ ���� �ε� ���� {adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"������ ���� ���� ���� {adUnitId}: {error.ToString()} - {message}");
        OnFail?.Invoke();
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }
}
