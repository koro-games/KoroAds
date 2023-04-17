using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using KoroGames.KoroAds;


namespace KoroGames.KoroAds
{
    public class MoveToMenu : MonoBehaviour
    {
        [SerializeField] private PreGameCondition[] _gameCondition;
        [SerializeField] private GameLoader _gameLoader;

        private bool ConditionDone
        {
            get
            {
                foreach (var condition in _gameCondition)
                {
                    if (condition.IsDone())
                        return true;
                }
                return false;
            }
        }

        void Awake()
        {
            StartCoroutine(LoadMenu());
            foreach (var preGameCondition in _gameCondition)
            {
                preGameCondition.Init();
            }
        }

        private IEnumerator LoadMenu()
        {
            yield return new WaitWhile(() => !ConditionDone || AdsWork.Manager == null);
            _gameLoader.LoadGame();
        }
    }
}
