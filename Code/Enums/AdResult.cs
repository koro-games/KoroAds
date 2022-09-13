using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KoroGames.KoroAds
{
    public enum AdResult
    {
        empty,
        success,
        not_available,
        waited,
        start,
        watched,
        clicked,
        canceled
    }
}
