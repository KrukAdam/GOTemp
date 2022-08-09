using UnityEngine;

//
// Must be placed in the first scene of the game
//
[DefaultExecutionOrder(-100)]
public class PlatformIntegrator : MonoBehaviour
{
    private void Awake()
    {

#if STEAM_BUILD
        var steamManager = GetComponentInChildren<SteamManager>(true);
        steamManager.gameObject.SetActive(true);
        steamManager.transform.SetParent(null);
#elif GOG_BUILD
        var galaxyManager = GetComponentInChildren<GalaxyManager>(true);
        galaxyManager.gameObject.SetActive(true);
        galaxyManager.transform.SetParent(null);
#endif

    }
}
