using UnityEngine;
using GoogleMobileAds.Api;

public class AdMobAdsScript : MonoBehaviour
{
    private string appId = "ca-app-pub-6537113978825755~8376155834";
    private string bannerId = "ca-app-pub-6537113978825755/4944177289";

    BannerView bannerAd;

    private void Start()
    {
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.Initialize(initStatus =>
        {
            print("Ads Initialized!");
        });
        LoadBannerAd();
    }

    public void LoadBannerAd()
    {
        // Create a banner
        CreateBannerView();
        // listen to banner events
        ListenToBannerEvents();
        // load banner
        if (bannerAd == null) CreateBannerView();

        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");
        print("Loading banner Ad!!");
        bannerAd.LoadAd(adRequest);
    }

    void CreateBannerView()
    {
        if (bannerAd != null) DestroyBannerAd();
        bannerAd = new BannerView(bannerId, AdSize.Banner, AdPosition.Top);
    }

    void ListenToBannerEvents()
    {
        // Raised when an ad is loaded into the banner view.
        bannerAd.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + bannerAd.GetResponseInfo());
        };
        // Raised when an ad fails to load into the banner view.
        bannerAd.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                + error);
        };
        // Raised when the ad is estimated to have earned money.
        bannerAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log("Banner view paid {0} {1}." +
                adValue.Value +
                adValue.CurrencyCode);
        };
        // Raised when an impression is recorded for an ad.
        bannerAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        bannerAd.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        bannerAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        bannerAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }

    public void DestroyBannerAd()
    {
        if (bannerAd != null)
        {
            print("Destroying banner Ad");
            bannerAd.Destroy();
            bannerAd = null;
        }
    }
}
