#if GOG_BUILD
using Galaxy.Api;
using Helpers;
#endif
using UnityEngine;

public class GalaxyManager : MonoBehaviour
{
#if GOG_BUILD

    #region Variables

    // Project specific data (ClientID and ClientSecret can be obtained from devportal.gog.com's game page)
    // ClientID and ClientSecret are merged into initParams for later use with Init(InitParams initParams) method
    private readonly string clientID = "50225266424144145";
    private readonly string clientSecret = "45955f1104f99b625a5733fa1848479b43d63bdb98f0929e37c9affaf900e99f";

    // Declaration of GalaxyManager static Instance so that it is easily available from anywhere in the code
    public static GalaxyManager Instance;
    private static GalaxyID myGalaxyID;
    public GalaxyID MyGalaxyID { get { return myGalaxyID; } }
    // Galaxy initiation result
    private bool galaxyFullyInitialized;
    public bool GalaxyFullyInitialized { get { return galaxyFullyInitialized; } }

    //  Authentication listener
    public AuthenticationListener authListener;
    public GogServicesConnectionStateListener gogServicesConnectionStateListener;

    #endregion

    #region Behaviors

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(this);
        }
    }

    void OnEnable()
    {
        Init();
        ListenersInit();
        SignInGalaxy();
    }

    void Update()
    {
        /* Makes the GalaxyPeer process data. 
        NOTE: This is required for any listener to work, and has little overhead. */
        GalaxyInstance.ProcessData();
    }

    void OnDisable()
    {
        ListenersDispose();
    }

    void OnApplicationQuit()
    {
        /* Shuts down the working instance of GalaxyPeer. 
        NOTE: Shutdown should be the last method called, and all listeners should be closed before that. */
        GalaxyInstance.Shutdown(true);
        Instance = null;
        Destroy(this);
    }

    #endregion

    #region Listeners methods

    private void ListenersInit()
    {
        Listener.Create(ref authListener);
        Listener.Create(ref gogServicesConnectionStateListener);
    }

    private void ListenersDispose()
    {
        Listener.Dispose(ref authListener);
        Listener.Dispose(ref gogServicesConnectionStateListener);
    }

    #endregion

    #region Methods

    /* Initializes GalaxyPeer instance
    NOTE: Even if Init will throw exepction Apps interface will still be available. 
    If you want to use Apps interface in your game make sure that shutdown is NOT called when Init throws an exception.*/
    private void Init()
    {
        InitParams initParams = new InitParams(clientID, clientSecret);
        Debug.Log("Initializing GalaxyPeer instance...");
        try
        {
            GalaxyInstance.Init(initParams);
            galaxyFullyInitialized = true;
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Init failed for reason " + e);
            galaxyFullyInitialized = false;
        }
    }

    /* Signs the current user in to Galaxy services
    NOTE: This call is asynchronus. Sign in result is received by AuthListener. */
    public void SignInGalaxy()
    {
        Debug.Log("Signing user in using Galaxy client...");
        try
        {
            GalaxyInstance.User().SignInGalaxy();
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("SignInGalaxy failed for reason " + e);
        }
    }

    public void SignInCredentials(string username, string password)
    {
        Debug.Log("Signing user in using credentials...");
        try
        {
            GalaxyInstance.User().SignInCredentials(username, password);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("SignInCredentials failed for reason " + e);
        }
    }

    /* Signs the current user out from Galaxy services */
    public void SignOut()
    {
        Debug.Log("Singing user out...");
        try
        {
            GalaxyInstance.User().SignOut();
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("SignOut failed for reason " + e);
        }
    }

    /* Checks current user singed in status
    NOTE: Signed in means that the user is signed in to GOG Galaxy client (he does not have to have working internet connection). */
    public bool IsSignedIn(bool silent = false)
    {
        bool signedIn = false;
        if (!silent) Debug.Log("Checking SignedIn status...");
        try
        {
            signedIn = GalaxyInstance.User().SignedIn();
            if (!silent) Debug.Log("User SignedIn: " + signedIn);
        }
        catch (GalaxyInstance.Error e)
        {
            if (!silent) Debug.LogWarning("Could not check user signed in status for reason " + e);
        }
        return signedIn;

    }

    /* Checks if user is logged on
    NOTE: Logged on means that the user is signed in to GOG Galaxy client and he does have working internet connection */
    public bool IsLoggedOn(bool silent = false)
    {
        bool isLoggedOn = false;

        if (!silent) Debug.Log("Checking LoggedOn status...");

        try
        {
            isLoggedOn = GalaxyInstance.User().IsLoggedOn();
            if (!silent) Debug.Log("User logged on: " + isLoggedOn);
        }
        catch (GalaxyInstance.Error e)
        {
            if (!silent) Debug.LogWarning("Could not check user logged on status for reason " + e);
        }

        return isLoggedOn;

    }

    // Checks if DLC specified by productID is installed on users machine
    public bool IsDlcInstalled(ulong productID)
    {
        bool isDlcInstalled = false;

        Debug.Log("Checking is DLC " + productID + " installed");

        try
        {
            isDlcInstalled = GalaxyInstance.Apps().IsDlcInstalled(productID);
            Debug.Log("DLC " + productID + " installed " + isDlcInstalled);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.LogWarning("Could not check is DLC " + productID + " installed for reason " + e);
        }

        return isDlcInstalled;

    }

    public string GetCurrentGameLanguage ()
    {
        string gameLanguage = null;
        Debug.Log("Checking current game language");
        try
        {
            gameLanguage = GalaxyInstance.Apps().GetCurrentGameLanguage();
            Debug.Log("Current game language is " + gameLanguage);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.Log("Could not check current game language for reason " + e);
        }
        return gameLanguage;
    }

    public void ShowOverlayWithWebPage (string url)
    {
        Debug.Log("Opening overlay with web page " + url);
        try
        {
            GalaxyInstance.Utils().ShowOverlayWithWebPage(url);
            Debug.Log("Opened overlay with web page " + url);
        }
        catch (GalaxyInstance.Error e)
        {
            Debug.Log("Could not open overlay with web page " + url + " for reason " + e);
        }
    }

    #endregion

    #region Listeners

    /* Informs about the event of authenticating user
    Callback for method:
    SignInGalaxy() */
    public class AuthenticationListener : GlobalAuthListener
    {
        public override void OnAuthSuccess()
        {
            myGalaxyID = GalaxyInstance.User().GetGalaxyID();

            Debug.Log("Successfully signed in as user: " + myGalaxyID);

        }

        public override void OnAuthFailure(FailureReason failureReason)
        {
            Debug.LogWarning("Failed to sign in for reason " + failureReason);
        }

        public override void OnAuthLost()
        {
            Debug.LogWarning("Authorization lost");
        }

    }

    public class GogServicesConnectionStateListener : GlobalGogServicesConnectionStateListener
    {
        public override void OnConnectionStateChange(GogServicesConnectionState connected)
        {
            Debug.Log("Connection state to GOG services changed to " + connected);
        }
    }

    #endregion

#endif
}

