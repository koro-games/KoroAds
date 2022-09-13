namespace KoroGames.KoroAds
{
    public interface IAdBanner
    {
        IAdAnalytic Analytic { get; set; }
        bool IsOpen { get; }
        void ShowBanner();
        void HideBanner();
    }
}