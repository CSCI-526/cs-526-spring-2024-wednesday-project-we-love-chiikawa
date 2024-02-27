using UnityEngine;
using UnityEngine.UI;

public class Battery : MonoBehaviour
{
    public GameObject player;
    public Image batteryUI;
    public Sprite[] batterySprites;

    // Update is called once per frame
    void Update()
    {
        batteryUI.sprite = batterySprites[player.GetComponent<BatteryController>().batteryLevel];
    }
}
