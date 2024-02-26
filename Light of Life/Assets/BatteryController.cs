using UnityEngine;
using UnityEngine.UI;

public class BatteryController : MonoBehaviour
{
    public Image batteryUI;
    public Sprite[] batterySprites; // Array holding the battery sprites in order from full to empty
    private int currentBatteryLevel = 5; // Assuming the initial battery level is full (battery_1)

    // Update is called once per frame
    void Update()
    {
        // Check if the player presses the "F" key
        if (Input.GetKeyDown(KeyCode.F))
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
