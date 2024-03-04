using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetRestartCount : MonoBehaviour
{
    public void RestRestartCount()
    {
        PlayerPrefs.SetInt("RestartCount", 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
