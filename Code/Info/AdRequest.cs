using System;

namespace KoroGames.KoroAds
{
    public class AdRequest
    {
        public string PlacementName;
        public string Status;
        /// <summary>
        /// Call when player watched ad and close it
        /// </summary>
        public Action OnClose;
        /// <summary>
        /// Call right after show ad for player
        /// </summary>
        public Action OnDisplay;
        /// <summary>
        /// Call for rewarded ad if player watch all ad
        /// </summary>
        public Action OnReward;
        /// <summary>
        /// Call if plyer not wait and close ad loading screen
        /// </summary>
        public Action OnNotWaited;

        public AdResult result;

        public AdRequest(string placementName)
        {
            PlacementName = placementName;
        }
    }
}