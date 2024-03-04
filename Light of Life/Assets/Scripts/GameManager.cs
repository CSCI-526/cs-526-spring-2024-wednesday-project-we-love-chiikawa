using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int restartCount = 0;

    public void Start()
    {
        restartCount = PlayerPrefs.GetInt("RestartCount", 0);
    }

    public void RestartLevel()
    {
        restartCount++;
        PlayerPrefs.SetInt("RestartCount", restartCount);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void ResetRestartCount()
    {
        PlayerPrefs.SetInt("RestartCount", 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f; // Pause the time
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // Resume the time
    }
}
