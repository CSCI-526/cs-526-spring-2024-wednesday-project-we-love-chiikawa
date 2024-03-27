using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int restartCount = 0;

    public void Start()
    {
        restartCount = PlayerPrefs.GetInt("RestartCount", 0);
        Time.timeScale = 1f;
    }

    public void RestartLevel()
    {
        restartCount++;
        PlayerPrefs.SetInt("RestartCount", restartCount);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }
    public void ResetLevel()
    {
        PlayerPrefs.SetInt("RestartCount", 0);
        PlayerPrefs.SetInt("EnergyUsedCount", 0);
        PlayerPrefs.SetInt("DeathCount", 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
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
