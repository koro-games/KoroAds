using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace KoroGames.KoroAds
{
    public class AdsWork : MonoBehaviour
    {
        [SerializeField] private GameObject _loadingScreen;
        private IAdAdapter _adAdapter;
        private IAdInterstitial _adInterstitial;
        private IAdRewarded _adRewarded;
        private IAdBanner _adBanner;
        private IAdAnalytic _adAnalytic;
        private AdRequest _endLevelAd;

        [Header("AD")]
        public bool UseInterstitial;
        public bool UseRewarded;
        public bool UseBanner;

        [Tooltip("Load inter if reward not loaded and vice versa"), Header("Setting")] public bool AllowCrossAd;
        [Tooltip("Alllow load ad with loading screen, or skip ads")] public bool AllowInterstitialLongLoad;
        [Tooltip("Alllow load ad with loading screen, or skip ads")] public bool AllowRewadedLongLoad;

#if DEBUG_MODE
        [Tooltip("Debug use no ads")] public bool DebugNotAd;
        [Tooltip("Disable load ad inter")] public bool Debug_InterstitialNotLoad;
        [Tooltip("Disable load ad reward")] public bool Debug_RewardNotLoad;
#else
        public bool DebugNotAd => false;
        public bool Debug_InterstitialNotLoad => false;
        public bool Debug_RewardNotLoad => false;
#endif

        public bool IsNoAd => KoroGames.KoroAds.Products.NoAdProduct.NoAdsStatus || DebugNotAd;
        public bool IsInterstitialLoaded() => (_adAdapter.IsInterstitialLoaded() || (AllowCrossAd && _adAdapter.IsRewardedLoaded())) && !DebugNotAd;
        public bool IsRewardLoaded() => (_adAdapter.IsRewardedLoaded() || (AllowCrossAd && _adAdapter.IsInterstitialLoaded())) && !DebugNotAd;


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



        public void CallInterstitial(AdRequest request)
        {
            if (CurrentAd != null) CurrentAd = null;

            CurrentAd = request;
            Debug.Log(CurrentAd == null ? "None AD" : CurrentAd.PlacementName);
            request.OnClose += () => CurrentAd = null;

            if (!UseInterstitial || IsNoAd)
            {
                request.OnClose.Invoke();
                CurrentAd = null;
                return;
            }

            if (Debug_InterstitialNotLoad || !_adInterstitial.TryCallInterstitial(request))
            {
                if (_adAdapter.IsRewardedLoaded() && !Debug_RewardNotLoad && AllowCrossAd)
                {
                    if (_adRewarded.TryCallRewarded(request))
                    {
                        return;
                    }
                }

                if (AllowInterstitialLongLoad)
                {
                    _adRewarded.OnAdLoad += () => CallInterstitial(request);
                    _adRewarded.OnAdLoad += () => _loadingScreen.SetActive(false);
                    _loadingScreen.SetActive(true);
                    return;
                }

                request.OnClose?.Invoke();
                CurrentAd = null;
                return;
            }

            _adAnalytic.VideoAdsAvailable(AdType.interstitial, request.PlacementName, AdResult.success);
        }

        public void CallReward(AdRequest request)
        {
            if (CurrentAd != null) CurrentAd = null;

            CurrentAd = request;
            Debug.Log(CurrentAd == null ? "None AD" : CurrentAd.PlacementName);
            request.OnClose += () => CurrentAd = null;

            if (!UseRewarded)
            {
                request.OnClose?.Invoke();
                request.OnReward?.Invoke();
                return;
            }

            if (Debug_RewardNotLoad || !_adRewarded.TryCallRewarded(request))
            {
                if (_adAdapter.IsInterstitialLoaded() && !Debug_InterstitialNotLoad && AllowCrossAd)
                {
                    request.OnClose += () => request.OnReward.Invoke();
                    if (_adInterstitial.TryCallInterstitial(request))
                    {
                        return;
                    }
                }

                if (AllowRewadedLongLoad)
                {
                    _adRewarded.OnAdLoad += () => CallReward(request);
                    _adRewarded.OnAdLoad += () => _loadingScreen.SetActive(false);
                    _loadingScreen.SetActive(true);
                    return;
                }

                request.OnClose?.Invoke();
                CurrentAd = null;
                return;
            }
            _adAnalytic.VideoAdsAvailable(AdType.rewarded, request.PlacementName, AdResult.success);
        }

        public void SetBannerStatus(bool status)
        {
            if (status && UseBanner && !IsNoAd)
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

        public void CloseLoading()
        {
            _loadingScreen.SetActive(false);
            CurrentAd?.OnNotWaited?.Invoke();
            CurrentAd = null;
            _adInterstitial.OnAdLoad = null;
            _adRewarded.OnAdLoad = null;
        }


#if DEBUG_MODE
    private void OnGUI()
    {
        GUI.color = Color.magenta;
        if (GUI.Button(new Rect(Screen.width / 2 - Screen.width * 0.2f, 300, Screen.width * 0.4f, Screen.width * 0.2f), "AD Debug"))
        {
            CallDebug();
        }
    }

    public void CallDebug() => _adAdapter.CallDebug();
#endif

        public void OnActiveNoAD()
        {
            UseBanner = false;
            UseInterstitial = false;
        }
    }
}