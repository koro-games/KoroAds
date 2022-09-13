namespace KoroGames.KoroAds
{
    public interface IAdAdapter
    {
        void Init();
        void CallDebug();
        bool IsInterstitialLoaded();
        bool IsRewardedLoaded();
    }
}