using UnityEngine;

public abstract class BaseAchievement : MonoBehaviour
{
    [field: SerializeField]
    public string ID
    {
        get;
        private set;
    }

    public bool Finished
    {
        get;
        private set;
    } = false;

    private bool inited;

    private void Awake()
    {
        Achievements.Instance.PlatformDataLoaded += OnPlatformDataLoaded;

        if (Achievements.Instance.PlatformOK)
        {
            OnPlatformDataLoaded();
        }
    }

    private void OnDestroy()
    {
        if (inited)
        {
            Deinit();
            inited = false;
        }

        Achievements.Instance.PlatformDataLoaded -= OnPlatformDataLoaded;
    }

    public override string ToString()
    {
        return ID;
    }

    public void UnlockAchievement()
    {
        if (!inited)
        {
            Debug.LogWarning($"You are trying to unlock {ID} achievement before it is initialized");
            return;
        }

        if (Finished)
        {
            return;
        }

        if (!Achievements.Instance.UnlockAchievement(ID))
        {
            Debug.LogWarning($"Unable to unlock {ID} achievement");
            return;
        }
    }

    protected abstract void Init();
    protected abstract void Deinit();

    private void OnPlatformDataLoaded()
    {
        Finished = Achievements.Instance.CheckAchievement(ID);

        // If this achievement is already unlocked it should not be inited
        if (Finished)
        {
            if (inited)
            {
                Deinit();
                inited = false;
            }
        }
        else if (!inited)
        {
            Init();
            inited = true;
        }
    }
}

