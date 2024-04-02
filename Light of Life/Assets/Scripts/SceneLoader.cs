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

    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level1");
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("Level2");
    }
}
