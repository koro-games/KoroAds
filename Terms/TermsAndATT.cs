using System.Collections;
using System.Collections.Generic;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;    
#endif
using UnityEngine;
using UnityEngine.Events;


namespace KoroGames.KoroAds
{

    public class TermsAndATT : PreGameCondition
    {

        private const string TermsAcceptedKey = "UserAcceptTerms";

        #region Fields
        public event UnityAction EventOnTermsAccepted;

        [SerializeField] protected UITermsPanel TermsPanelPrefab;
        [SerializeField] protected Transform PanelParentCanvas;

        private UITermsPanel PanelInstance;
        private bool _isDone;
        #endregion

        #region Public
        public override bool IsDone() => _isDone;

        public override void Init()
        {
            EventOnTermsAccepted += () => _isDone = true;
            BeginPlay();
        }

        public void BeginPlay()
        {
#if UNITY_IOS
            ShowATTIOSDialog();
#endif
            int TermsAcceptValue = PlayerPrefs.GetInt(TermsAcceptedKey, 0);
            bool bTermsAccepted = (TermsAcceptValue != 0);
            if (!bTermsAccepted)
            {
                ShowTermsPanel();
            }
            else
            {
                EventOnTermsAccepted?.Invoke();
            }
        }
        #endregion

        #region Helpers

        private void ShowATTIOSDialog()
        {
#if UNITY_IOS
            if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED) {
                ATTrackingStatusBinding.RequestAuthorizationTracking();
            }
#endif
        }

        private void ShowTermsPanel()
        {

            Transform PanelParent = PanelParentCanvas;
            if (!PanelParent)
            {
                Canvas AnyCanvas = FindObjectOfType<Canvas>();
                if (AnyCanvas)
                {
                    PanelParent = AnyCanvas.transform;
                }
            }

            if (PanelParent)
            {
                if (TermsPanelPrefab)
                {
                    PanelInstance = Instantiate(TermsPanelPrefab, PanelParent);
                    if (PanelInstance)
                    {
                        PanelInstance.EventOnAcceptClick += PanelInstanceOnEventOnAcceptClick;
                    }
                }
            }
            else
            {
                Debug.LogError($"MAXHelper: Unable to find proper canvas for Terms panel!", gameObject);
            }

        }

        private void PanelInstanceOnEventOnAcceptClick()
        {
            PlayerPrefs.SetInt(TermsAcceptedKey, 1);
            bool bHasConsent = true;
#if UNITY_IOS
            ATTrackingStatusBinding.AuthorizationTrackingStatus Status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
            bHasConsent = (Status == ATTrackingStatusBinding.AuthorizationTrackingStatus.AUTHORIZED);
            Debug.LogWarning($"STATUS: {Status}   Has consent: {bHasConsent}");
#endif

            var termsElements = GetComponents<ITermsElement>();

            foreach (var term in termsElements)
            {
                term.SetStatus(bHasConsent);
            }

            PanelInstance.EventOnAcceptClick -= PanelInstanceOnEventOnAcceptClick;
            EventOnTermsAccepted?.Invoke();
        }
        #endregion
    }

}
