using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdsInterstitial : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    string unitId;

    public void LoadAd(string unitId)
    {
        this.unitId = unitId;
        Advertisement.Load(unitId, this);
    }

    public void ShowAd()
    {
        Advertisement.Show(unitId, this);
    }

    // Implement Load Listener and Show Listener interface methods: 
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        // Optionally execute code if the Ad Unit successfully loads content.
        Debug.LogFormat("OnUnityAdsAdLoaded: {0}", adUnitId);
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState) { }
}
