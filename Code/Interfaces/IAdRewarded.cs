namespace KoroGames.KoroAds
{
    public interface IAdRewarded
    {
        IAdAnalytic Analytic { get; set; }
        System.Action OnAdLoad { get; set; }
        bool IsLoadAd();
        bool TryCallRewarded(AdRequest adRequest);
    }
}