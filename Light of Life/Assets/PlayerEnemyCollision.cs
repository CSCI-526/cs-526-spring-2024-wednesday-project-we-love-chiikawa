using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class PlayerEnemyCollision : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.name.Contains("Spike"))
        {
            
            SceneManager.LoadScene("GameOverScene"); 
        }
    }
}
