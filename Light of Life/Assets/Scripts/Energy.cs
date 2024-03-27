using UnityEngine;

public class Energy : MonoBehaviour
{
    public GameObject player;
    public GameObject parent;

    // This method is called when another collider enters the trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider belongs to the player (you might need to adjust the tag or layer)
        if (other.CompareTag("Player"))
        {
            player.GetComponent<BatteryController>().batteryLevel++;
            player.GetComponent<BatteryController>().batteryLevel = Mathf.Clamp(player.GetComponent<BatteryController>().batteryLevel, 0, 5);
            Destroy(parent);
        }
    }
}
