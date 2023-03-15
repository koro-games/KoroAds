using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using KoroGames.KoroAds;


namespace KoroGames.KoroAds
{
    public class MoveToMenu : MonoBehaviour
    {
            [SerializeField] private TermsAndATT _terms;

            private bool _termsAccept;

            void Awake()
        {
                StartCoroutine(LoadMenu());
                _terms.EventOnTermsAccepted += () => _termsAccept = true;
        }

        private IEnumerator LoadMenu()
        {
            yield return new WaitWhile(() => !_termsAccept || AdsWork.Manager == null);
            SceneManager.LoadScene("Menu");
        }
    }
}
