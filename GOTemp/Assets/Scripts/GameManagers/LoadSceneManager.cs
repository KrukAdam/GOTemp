using UnityEngine.SceneManagement;

public static class LoadSceneManager
{
    public static void ResetCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
