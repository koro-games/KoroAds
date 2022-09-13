namespace KoroGames.KoroAds
{
    public interface IAdAnalytic
    {
        bool ConnectionStatus { get; }

        void VideoAdsAvailable(AdType adType, string placement, AdResult result);
        void VideoAdsStarted(AdType adType, string placement, AdResult result);
        void VideoAdsWatch(AdType adType, string placement, AdResult result, AdRevenue revenueData);
    }
}