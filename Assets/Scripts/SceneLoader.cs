using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{

    public static void LoadNextSceneStatic()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
    public static void RestartSceneStatic()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
    public static void LoadSceneByName(string nextScene)
    {
        SceneManager.LoadScene(nextScene);
    }
    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    internal static void LoadGameOverSceneStatic()
    {
        SceneManager.LoadScene("GameOver");
    }
    internal static void MenuSceneStatic()
    {
        SceneManager.LoadScene("menu");
    }

    public static void LoadFirstSceneStatic()
    {
        SceneManager.LoadScene(0);
    }
}
