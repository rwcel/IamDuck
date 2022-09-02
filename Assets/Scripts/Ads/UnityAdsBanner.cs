using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdsBanner : MonoBehaviour
{
    [SerializeField] BannerPosition bannerPosition = BannerPosition.TOP_CENTER;

    string unitId = null; 


    public void LoadBanner(string unitId)
    {
        this.unitId = unitId;
        Debug.Log(unitId);
        Advertisement.Banner.SetPosition(bannerPosition);

        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        Advertisement.Banner.Load(unitId, options);
    }

    void OnBannerLoaded() => Debug.Log("Banner loaded");
    void OnBannerError(string message) => Debug.Log($"Banner Error: {message}");

    /// <summary>
    /// 광고 구독중이면 실행 안함
    /// </summary>
    public void ShowBannerAd()
    {
        BannerOptions options = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            hideCallback = OnBannerHidden,
            showCallback = OnBannerShown
        };

        Advertisement.Banner.Show(unitId, options);
    }

    public void HideBannerAd()
    {
        Advertisement.Banner.Hide();
    }

    void OnBannerClicked() { }
    void OnBannerShown() { }
    void OnBannerHidden() { }
}