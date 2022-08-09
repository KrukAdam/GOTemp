using System;
using System.Collections;
#if STEAM_BUILD
using Steamworks;
#elif GOG_BUILD
using Galaxy.Api;
using Helpers;
#endif
using UnityEngine;

[DefaultExecutionOrder(-50)]
public class Achievements : MonoBehaviour
{
#if GOG_BUILD

    /* Informs about the event of receiving user stats and achievements definitions
    Callbacks for method: 
    GalaxyInstance.Stats().RequestUserStatsAndAchievements() */
    private class UserStatsAndAchievementsRetrieveListener : GlobalUserStatsAndAchievementsRetrieveListener
    {
        public override void OnUserStatsAndAchievementsRetrieveSuccess(GalaxyID userID)
        {
            if (userID != Achievements.Instance.myGalaxyID)
            {
                return;
            }

            //Debug.Log("User " + userID + " stats and achievements retrieved");
            Achievements.Instance.PlatformOK = true;
            Achievements.Instance.PlatformDataLoaded();
        }

        public override void OnUserStatsAndAchievementsRetrieveFailure(GalaxyID userID, FailureReason failureReason)
        {
            //Debug.LogWarning("User " + userID + " stats and achievements could not be retrieved, for reason " + failureReason);
            Achievements.Instance.load = true;
        }
    }

    /* Informs about the event of storing changes made to the achievement or statiscis of a user
    Callback for method: 
    GalaxyInstance.Stats().StoreStatsAndAchievements(); */
    private class StatsAndAchievementsStoreListener : GlobalStatsAndAchievementsStoreListener
    {
        public override void OnUserStatsAndAchievementsStoreSuccess()
        {
            Achievements.Instance.RequestUserStatsAndAchievements();
        }

        public override void OnUserStatsAndAchievementsStoreFailure(FailureReason failureReason)
        {
            //Debug.LogWarning("OnUserStatsAndAchievementsStoreFailure: " + failureReason);
            Achievements.Instance.save = true;
        }
    }

    /* Informs about the event of unlocking an achievement
    Callback for methods:
    GalaxyInstance.Stats().SetAchievement(apiKey); */
    private class AchievementChangeListener : GlobalAchievementChangeListener
    {
        public override void OnAchievementUnlocked(string name)
        {
            Achievements.Instance.AchievementReached(name);
        }
    }

#endif

    public event Action<string> AchievementReached = delegate { };
    public event Action PlatformDataLoaded = delegate { };

    public static Achievements Instance
    {
        get;
        private set;
    }

    public bool PlatformOK
    {
        get;
        private set;
    }

    private bool load;
    private bool save;

#if STEAM_BUILD
    private ulong gameID;
    private Callback<UserStatsReceived_t> statsReceived;
    private Callback<UserStatsStored_t> statsStored;
    private Callback<UserAchievementStored_t> achievementStored;
#elif GOG_BUILD
    private GalaxyID myGalaxyID;
    private UserStatsAndAchievementsRetrieveListener achievementRetrieveListener;
    private StatsAndAchievementsStoreListener achievementStoreListener;
    private AchievementChangeListener achievementChangeListener;
#endif

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
#if STEAM_BUILD
        if (!SteamManager.Initialized)
        {
            gameObject.SetActive(false);
            return;
        }

        gameID = new CGameID(SteamUtils.GetAppID()).m_GameID;
        statsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
        statsStored = Callback<UserStatsStored_t>.Create(OnUserStatsStored);
        achievementStored = Callback<UserAchievementStored_t>.Create(OnAchievementStored);
#elif GOG_BUILD
        if (!GalaxyManager.Instance.GalaxyFullyInitialized)
        {
            gameObject.SetActive(false);
            return;
        }

        myGalaxyID = GalaxyManager.Instance.MyGalaxyID;
        GogListenersInit();
#endif

        RequestUserStatsAndAchievements();
    }

#if GOG_BUILD
    private void OnDisable()
    {
        GogListenersDispose();
    }
#endif

    private void Update()
    {
        if (load)
        {
#if STEAM_BUILD
            load = !SteamUserStats.RequestCurrentStats();
#elif GOG_BUILD
            // load is changed by the appropriate gog callbacks
            GalaxyInstance.Stats().RequestUserStatsAndAchievements();
            load = false;
#endif
        }

        if (save)
        {
#if STEAM_BUILD
            save = !SteamUserStats.StoreStats();
#elif GOG_BUILD
            // save is changed by the appropriate gog callbacks
            GalaxyInstance.Stats().StoreStatsAndAchievements();
            load = false;
#endif
        }
    }

    public bool UnlockAchievement(string ID)
    {
        if (!gameObject.activeSelf || !PlatformOK)
        {
            return false;
        }

        // If this achievement is already unlocked, I return
        if (CheckAchievement(ID))
        {
            return false;
        }

#if STEAM_BUILD
        SteamUserStats.SetAchievement(ID);
#elif GOG_BUILD
        GalaxyInstance.Stats().SetAchievement(ID);
#endif

        StoreStatsAndAchievements();

        return true;
    }

    public bool CheckAchievement(string ID)
    {
        if (!gameObject.activeSelf || !PlatformOK)
        {
            return false;
        }

#if STEAM_BUILD
        SteamUserStats.GetAchievement(ID, out bool achieved);
        return achieved;
#elif GOG_BUILD
        return GalaxyInstance.Stats().GetAchievement(ID);
#endif
        return false;
    }

    public void RequestUserStatsAndAchievements()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        load = true;
    }

    public void ForceSave(Action onFinish)
    {
        if (!gameObject.activeSelf)
        {
            onFinish?.Invoke();
            return;
        }

        StartCoroutine(SaveCoroutine(onFinish));
    }

    private void StoreStatsAndAchievements()
    {
        save = true;
    }

    private IEnumerator SaveCoroutine(Action action)
    {
        int count = 0;
        save = true;

        while (save)
        {
            if (++count > 1000)
            {
                Debug.LogError("Error saving achievements");
                break;
            }
            Update();
            yield return null;
        }

        action?.Invoke();
    }

#if GOG_BUILD
    private void GogListenersInit()
    {
        Listener.Create(ref achievementRetrieveListener);
        Listener.Create(ref achievementChangeListener);
        Listener.Create(ref achievementStoreListener);
    }

    private void GogListenersDispose()
    {
        Listener.Dispose(ref achievementStoreListener);
        Listener.Dispose(ref achievementRetrieveListener);
        Listener.Dispose(ref achievementChangeListener);
    }
#endif

#if STEAM_BUILD

    private void OnUserStatsReceived(UserStatsReceived_t steamData)
    {
        if (steamData.m_nGameID != gameID || steamData.m_eResult != EResult.k_EResultOK)
        {
            return;
        }

        PlatformOK = true;
        PlatformDataLoaded();
    }

    private void OnUserStatsStored(UserStatsStored_t steamData)
    {
        if (steamData.m_nGameID != gameID)
        {
            return;
        }

        switch (steamData.m_eResult)
        {
            case EResult.k_EResultOK:
                OnUserStatsReceived(new UserStatsReceived_t() { m_nGameID = gameID, m_eResult = EResult.k_EResultOK });
                break;
            case EResult.k_EResultInvalidParam:
                save = false;
                RequestUserStatsAndAchievements();
                break;
        }
    }

    private void OnAchievementStored(UserAchievementStored_t steamData)
    {
        if (steamData.m_nGameID != gameID)
        {
            return;
        }

        var reachedAchievementID = steamData.m_rgchAchievementName;
        AchievementReached(reachedAchievementID);
    }

#endif
}
