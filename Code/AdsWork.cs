using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AdsWork : MonoBehaviour
{
    [SerializeField] private GameObject _loadingScreen;
    private IAdAdapter _adAdapter;
    private IAdInterstitial _adInterstitial;
    private IAdRewarded _adRewarded;
    private IAdBanner _adBanner;
    private IAdAnalytic _adAnalytic;
    private AdRequest _endLevelAd;

    public bool UseInterstitial;
    public bool UseRewarded;
    public bool UseBanner;
    public bool DebugNotAd;

    public bool NoAdOffer { get => PlayerPrefs.GetInt("NoAdOffer") == 1; set => PlayerPrefs.SetInt("NoAdOffer", value ? 1 : 0); }

    public static AdsWork Manager { get; private set; }

    public AdRequest CurrentAd
    {
        get => currentAd;
        set
        {
            if (currentAd == null || value == null)
                currentAd = value;
            else
                throw new Exception("Current ad request not null");
        }
    }
    private AdRequest currentAd;

    private void Awake()
    {

        if (PlayerPrefs.GetInt("NoAds") == 1)
        {
            OnActiveNoAD();
        }

        if (Manager != this && Manager != null)
        {
            Destroy(gameObject);
            return;
        }
        Manager = this;
        DontDestroyOnLoad(gameObject);

        _endLevelAd = new AdRequest("end_level_interstitial") { OnDisplay = () => _loadingScreen.SetActive(false) };
        _adAdapter = GetComponent<IAdAdapter>();
        _adAdapter.Init();

        _adInterstitial = GetComponent<IAdInterstitial>();
        _adRewarded = GetComponent<IAdRewarded>();
        _adBanner = GetComponent<IAdBanner>();
        _adAnalytic = GetComponent<IAdAnalytic>();
    }

    public void CallEndLevelAd(Action OnClose)
    {
        _endLevelAd.OnClose = OnClose;
        _endLevelAd.OnClose += () => CurrentAd = null;
        _endLevelAd.OnClose += _endLevelAd.OnClose = null;
        _endLevelAd.OnNotWaited = _endLevelAd.OnClose;
        CallInterstitial(_endLevelAd);
    }

    public void CallInterstitial(AdRequest request)
    {
        if (CurrentAd != null) return;

        CurrentAd = request;
        request.OnClose += () => CurrentAd = null;

        if (!UseInterstitial || NoAdOffer || !_adInterstitial.TryCallInterstitial(request))
        {
            request.OnClose.Invoke();
            CurrentAd = null;
            return;
        }

        if (false)
        {
            _adAnalytic.VideoAdsAvailable(AdType.interstitial, request.PlacementName, AdResult.not_available);
            _loadingScreen.SetActive(true);
            _adInterstitial.OnAdLoad = () =>
            {
                _adAnalytic.VideoAdsAvailable(AdType.interstitial, request.PlacementName, AdResult.waited);
                _adInterstitial.TryCallInterstitial(request);
                _loadingScreen.SetActive(false);
            };
            return;
        }
        _adAnalytic.VideoAdsAvailable(AdType.interstitial, request.PlacementName, AdResult.success);

    }

    public bool IsInterstitialLoaded() => _adAdapter.IsInterstitialLoaded() && !DebugNotAd;

    public void CallReward(AdRequest request)
    {

        if (CurrentAd != null) return;

        CurrentAd = request;
        if (!UseRewarded)
        {
            request.OnClose?.Invoke();
            request.OnReward?.Invoke();
            CurrentAd = null;
            return;
        }
        request.OnClose += () => CurrentAd = null;


        if (!_adRewarded.TryCallRewarded(request))
        {
            request.OnClose?.Invoke();
            CurrentAd = null;
            return;
        }
        _adAnalytic.VideoAdsAvailable(AdType.rewarded, request.PlacementName, AdResult.success);
    }

    public bool IsRewardLoaded() => _adAdapter.IsRewardedLoaded() && !DebugNotAd;

    public void CallCloseAdsLoad()
    {
        if (CurrentAd != null && CurrentAd.OnNotWaited != null) CurrentAd.OnNotWaited();

        _adInterstitial.OnAdLoad = null;
        _adRewarded.OnAdLoad = null;
        _loadingScreen.SetActive(false);
        CurrentAd = null;
    }

    public void SetBannerStatus(bool status)
    {
        if (status && UseBanner && !NoAdOffer)
        {
            if (!_adBanner.IsOpen)
                _adBanner.ShowBanner();
        }
        else
        {
            if (_adBanner.IsOpen)
                _adBanner.HideBanner();
        }
    }

    public void CallDebug() => _adAdapter.CallDebug();

#if DEBUG_MODE
    private void OnGUI()
    {
        GUI.color = Color.magenta;
        if (GUI.Button(new Rect(Screen.width / 2 - Screen.width * 0.2f, 300, Screen.width * 0.4f, Screen.width * 0.2f), "AD Debug"))
        {
            CallDebug();
        }
    }
#endif

    public void OnActiveNoAD()
    {
        UseBanner = false;
        UseInterstitial = false;
    }
}
