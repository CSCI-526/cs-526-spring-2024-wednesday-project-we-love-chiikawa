using UnityEngine;
using UnityEngine.UI;

public class Battery : MonoBehaviour
{
    public GameObject flashlight;
    public Image batteryUI;
    public Sprite[] batterySprites;
    private int currentBatteryLevel; 
    void Start()
    {
        currentBatteryLevel = flashlight.GetComponent<FlashLightRe>().batteryLevel;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player presses the "F" key
        if (Input.GetKeyDown(KeyCode.F) && !flashlight.GetComponent<FlashLightRe>().fixedRoads)
        {
            // Check if battery level is not 0%
            if (currentBatteryLevel > 0)
            {
                // Decrease the battery level and update the UI
                currentBatteryLevel--;
                batteryUI.sprite = batterySprites[currentBatteryLevel];
            }
            else
            {
                // Battery is at 0%, prevent flashlight operation
                Debug.Log("Battery is empty. Flashlight cannot be operated.");
            }
        }
    }
}
