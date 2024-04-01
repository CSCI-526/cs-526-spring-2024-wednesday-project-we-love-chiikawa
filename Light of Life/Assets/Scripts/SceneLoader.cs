using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadHomeScene()
    {
        SceneManager.LoadScene("Home");
    }

    public void LoadLevelScene()
    {
        SceneManager.LoadScene("TutorialLevel");
    }
}
