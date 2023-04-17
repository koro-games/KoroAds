using UnityEngine;


namespace KoroGames.KoroAds
{
    public abstract class PreGameCondition : MonoBehaviour
    {
        public abstract void Init();

        public abstract bool IsDone();
    }
}
