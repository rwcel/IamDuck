using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdsManager : Singleton<UnityAdsManager>, IUnityAdsInitializationListener
{
    [SerializeField] UnityAdsBanner bannerAd;
    [SerializeField] UnityAdsInterstitial interstitialAD;
    [SerializeField] UnityAdsRewarded coinReward;
    [SerializeField] UnityAdsRewarded[] useItemRewards;

    string _androidGameId = "4838265";
    string _iOSGameId = "4838264";

    [SerializeField] bool _testMode = false;
    private string _gameId;

    private string banner_unitID = "Banner_Android";
    private string interstitial_unitID = "Interstitial_Android";
    private string reward_dailyShop_unitID = "Reward_DailyShop_Android";
    private string[] reward_useItems_unitID = {
        "Reward_StartBoost1_Android",
        "Reward_StartBoost2_Android",
        "Reward_BonusCoin_Android",
        "Reward_Continue_Android",
        "Reward_IgnoreBomb_Android",
        "Reward_ObstacleAppear_Android"
    } ;

    private bool isRemoveAds;


    protected override void AwakeInstance()
    {
        InitializeAds();
    }

    protected override void DestroyInstance()
    {
    }

    public void InitializeAds()
    {
        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOSGameId
            : _androidGameId;
        Advertisement.Initialize(_gameId, _testMode, this);

        LoadAds();
    }

    public void OnInitializationComplete()
    {
        Debug.Log("유니티 광고 활성화");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"유니티 광고 비활성화 {error} - {message}");
    }

    public void LoadAds()
    {
        bannerAd.LoadBanner(banner_unitID);
        interstitialAD.LoadAd(interstitial_unitID);
        coinReward.LoadAd(reward_dailyShop_unitID);
        for (int i = 0, length = useItemRewards.Length; i < length; i++)
        {
            useItemRewards[i].LoadAd(reward_useItems_unitID[i]);
        }
    }

    public void SwitchAds(bool isRemove)
    {
        isRemoveAds = isRemove;
        Debug.Log($"광고 제거 여부 : {isRemoveAds}");

        if (isRemove)
        {
            bannerAd.HideBannerAd();
        }
        else
        {
            bannerAd.ShowBannerAd();
        }
    }

    public void ShowInterstitialAD()
    {
        // Firebase.Analytics.FirebaseAnalytics.LogEvent("Interstitial_Ad");
        if (isRemoveAds)
            return;

        interstitialAD.ShowAd();
    }

    public void ShowRewardAD(System.Action OnSuccess, System.Action OnFail, EAds type)
    {
        //Debug.Log($"리워드 광고 : {adType}");

        if (isRemoveAds)
        {
            OnSuccess?.Invoke();
            // Closed?.Invoke();

            return;
        }
        //Firebase.Analytics.FirebaseAnalytics.LogEvent("Reward_Ad", "Ad_Name", adType.ToString());

        switch (type)
        {
            case EAds.StartBoost1:
            case EAds.StartBoost2:
            case EAds.BonusCoin:
            case EAds.Continue:
            case EAds.IgnoreBomb:
            case EAds.ObstacleAppear:
                useItemRewards[(int)type].ShowAd(OnSuccess, OnFail);
                break;
            case EAds.Coin:
                coinReward.ShowAd(OnSuccess, OnFail);
                break;
        }
    }
}
