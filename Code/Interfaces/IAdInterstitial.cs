namespace KoroGames.KoroAds
{
    public interface IAdInterstitial
    {

        IAdAnalytic Analytic { get; set; }
        System.Action OnAdLoad { get; set; }
        bool IsLoadAd();
        bool TryCallInterstitial(AdRequest adRequest);
    }
}