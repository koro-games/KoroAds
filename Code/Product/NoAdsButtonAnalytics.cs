using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace KoroGames.KoroAds.Products
{
    public class NoAdProduct : MonoBehaviour
    {
        public Button ADSButton;

        public IStoreController store;
        private void Start()
        {
            ADSButton.interactable = PlayerPrefs.GetInt("NoAds", 0) != 1;
            store = CodelessIAPStoreListener.Instance.StoreController;
            TryCallReceipt();
        }

        public void TryCallReceipt()
        {
            if (store != null && store.products.WithID("no_ads").hasReceipt)
            {
                Activate();
            }
        }


        public void SendNoAdPurchase(Product product)
        {
            PlayerPrefs.SetInt("NoAds", 1);
            var data = product.metadata;
            Activate();
        }


        public void Activate()
        {
            PlayerPrefs.SetInt("NoAds", 1);
            ADSButton.interactable = false;
            AdsWork.Manager.OnActiveNoAD();
        }
    }
}
