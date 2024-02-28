using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCollision : MonoBehaviour
{
    public GameObject flashlight2; // Assign this in the inspector to the Flashlight2 GameObject

    void OnCollisionEnter2D(Collision2D collision)
    {

        flashlight2.SetActive(true);
    }
}



