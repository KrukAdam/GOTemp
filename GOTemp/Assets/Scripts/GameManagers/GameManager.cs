using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance
    { 
        get; 
        private set;
    }
    public InputManager InputManager
    {
        get;
        private set; 
    }

    private void Awake()
    {
        #region Instance
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        Assert.IsNull(Instance);
        Instance = this;
        DontDestroyOnLoad(gameObject);
        #endregion

        #region Init Managers
        InputManager = new InputManager();
        InputManager.Init();
        InputManager.Enable();
        #endregion
    }
}
