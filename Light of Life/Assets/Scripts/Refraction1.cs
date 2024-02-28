using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;

public class RefractionEdge : MonoBehaviour
{   
    public Transform FlashLight;
    private GameObject Road1;
    private GameObject Road1_1;
    private GameObject Road2;
    private GameObject Road2_1;
    //private GameObject Road2_2;
    public SpriteRenderer roadRenderer;
    private bool isColliding = false;
    private float lastSafeDistance = 0f;
    private int ObstacleType = 0;

    void Start()
    {
        Road1 = new GameObject("Road1");
        InitializeRoad(Road1, 1);
        BoxCollider2D Road1collider = Road1.AddComponent<BoxCollider2D>();
        Road1collider.size = new Vector2(1, 0.05f);

        Road2 = new GameObject("Road2");
        InitializeRoad(Road2, 2);
        BoxCollider2D Road2collider = Road2.AddComponent<BoxCollider2D>();        
        Road2collider.size = new Vector2(1, 0.05f);  
    }

    void InitializeRoad(GameObject Road, int dir)
    {       
        roadRenderer = Road.AddComponent<SpriteRenderer>();
        Sprite defaultSprite = Resources.Load<Sprite>("Square");
        roadRenderer.sprite = defaultSprite;
        roadRenderer.color = Color.red;
        roadRenderer.sortingOrder = 1; 
        
        Road.transform.position = FlashLight.position;
        switch (dir)
        {
            case 1:
                Road.transform.localScale = new Vector2(5.0f, 0.05f);
                Road.transform.position -= FlashLight.up * 0.5f;
                break;
            case 2:
                Road.transform.localScale = new Vector2(5.0f, 0.05f);
                Road.transform.position += FlashLight.up * 0.5f;
                Road.transform.position += FlashLight.right * 2.0f;
                break;
        }      
        Road.transform.up = FlashLight.up;
    }

    void Update()
    {

        UpdateRoad(Road1, Road1_1);
        //UpdateRoad(Road2, Road2_1);
    }

    void UpdateRoad(GameObject Road, GameObject NewRoad)
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 flashlightRight = new Vector2(FlashLight.right.x, FlashLight.right.y);
        Vector2 FR_normalized = flashlightRight.normalized;
        Vector2 flashlightPosition = FlashLight.position;
        Vector2 FM = flashlightPosition - mousePosition;
        float distance = Mathf.Abs(Vector2.Dot(FM, FR_normalized));

        if (!isColliding || distance < lastSafeDistance)
        {
            Vector2 N = flashlightPosition + FR_normalized * distance;
            Vector2 midPoint = (N + flashlightPosition) / 2f;

            Road.transform.position = new Vector2(midPoint.x, midPoint.y);
            Road.transform.position -= FlashLight.up * 0.5f;
            Road.transform.localScale = new Vector2(distance, 0.05f);
            lastSafeDistance = distance;

            //need to fix collider
            // BoxCollider2D Roadcollider = Road.GetComponent<BoxCollider2D>();
            // Roadcollider.size = new Vector2(distance, 0.05f);
        }

        RaycastHit2D hit = Physics2D.Raycast(flashlightPosition, FR_normalized, distance);

        if (hit.collider != null && !isColliding)
        {
            isColliding = true;

            //obstacleType = 2
            if (hit.collider.gameObject.name.ToLower().Contains("glass"))
            {
                ObstacleType = 2;
            }
            Vector2 hitPoint = hit.point;
            if (ObstacleType == 2)
            {
                // Get Road1's position, scale, and rotation
                Vector2 road1Position = Road1.transform.position;
                Vector2 road1Scale = Road1.transform.localScale;
                float road1Rotation = Road1.transform.eulerAngles.z;
                float road1_1Roation = 0;

                // Calculate the direction in which Road1 is pointing based on its rotation
                Vector2 roadDirection = new Vector2(Mathf.Cos(road1Rotation * Mathf.Deg2Rad), Mathf.Sin(road1Rotation * Mathf.Deg2Rad));

                bool isPointingLeft = road1Rotation > 90 && road1Rotation < 270;
                if (isPointingLeft)
                {
                    roadDirection = -roadDirection;
                }

                // Since Road1 is centered on its pivot, you need to move half its length to get to the tip, assuming it's horizontal
                Vector2 roadEndPoint = road1Position + roadDirection * (road1Scale.x * 0.5f);

                // Instantiate Road1_1 and set its properties
                Road1_1 = Instantiate(Road);
                Road1_1.transform.localScale = new Vector2(5.0f, 0.05f); // Set the scale as needed
                Road1_1.transform.position = roadEndPoint; // Initially set position to Road1's endpoint
                Road1_1.transform.rotation = Quaternion.Euler(0, 0, road1_1Roation);

                // Adjust Road1_1's position to account for its pivot and length
                Vector2 adjustment = new Vector2(Road1_1.transform.localScale.x * 0.5f, 0);
                adjustment = isPointingLeft ? -adjustment : adjustment;
                Road1_1.transform.position = (Vector2) Road1_1.transform.position + adjustment;


                //for road2_1
                Vector2 road2Position = Road2.transform.position;
                Vector2 road2Scale = Road2.transform.localScale;
                float road2Rotation = Road2.transform.eulerAngles.z;
                float road2_1Roation = 0;


                Vector2 roadDirection2 = new Vector2(Mathf.Cos(road2Rotation * Mathf.Deg2Rad), Mathf.Sin(road2Rotation * Mathf.Deg2Rad));

                bool isPointingLeft2 = road2Rotation > 90 && road2Rotation < 270;
                if (isPointingLeft2)
                {
                    roadDirection2 = -roadDirection2;
                }

                Vector2 roadEndPoint2 = road2Position + roadDirection2 * (road2Scale.x * 0.5f);

                Road2_1 = Instantiate(Road2);
                Road2_1.transform.localScale = new Vector2(5.0f, 0.05f); 
                Road2_1.transform.position = roadEndPoint2; 
                Road2_1.transform.rotation = Quaternion.Euler(0, 0, road2_1Roation);

                Vector2 adjustment2 = new Vector2(Road2_1.transform.localScale.x * 0.5f, 0);
                adjustment2 = isPointingLeft2 ? -adjustment2 : adjustment2;
                Road2_1.transform.position = (Vector2) Road2_1.transform.position + adjustment;

            }
            ObstacleType = 0;
        }
        else if (hit.collider == null)
        {
            Destroy(Road1_1);
            Destroy(Road2_1);
            isColliding = false;
            lastSafeDistance = Mathf.Infinity;
        }
    }
}