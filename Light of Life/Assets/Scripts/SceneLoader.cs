using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadHomeScene()
    {
        SceneManager.LoadScene("Home");
    }

    public void LoadTutorial1()
    {
        SceneManager.LoadScene("TutorialLevel1");
    }

    public void LoadTutorial2()
    {
        SceneManager.LoadScene("TutorialLevel2");
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level1");
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("Level2");
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene("Level");
    }
}
