using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;

public class ReflectionEdge : MonoBehaviour
{   
    public Transform FlashLight;
    private GameObject Road1;
    private GameObject Road1_1;
    private GameObject Road2;
    private GameObject Road2_1;
    private GameObject Road2_2;
    public SpriteRenderer roadRenderer;
    private bool isColliding = false;
    private float lastSafeDistance = 0f;
    private int ObstacleType = 0;

    void Start()
    {
        //initial roads that player sees before reflection
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
            //for road 1
            case 1:
                Road.transform.localScale = new Vector2(5.0f, 0.05f);
                Road.transform.position -= FlashLight.up * 0.5f;
                break;
            //for road 2
            case 2:
                Road.transform.localScale = new Vector2(4.0f, 0.05f);
                Road.transform.position += FlashLight.up * 0.5f;
                Road.transform.position += FlashLight.right * 2.0f;
                break;
        }      
        Road.transform.up = FlashLight.up;
    }

    void Update()
    {
        UpdateRoad(Road1, Road1_1);
    }

    void UpdateRoad(GameObject Road, GameObject NewRoad)
    {
        //roads move by following mouse position
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
            BoxCollider2D Roadcollider = Road.GetComponent<BoxCollider2D>();
            Roadcollider.size = new Vector2(distance, 0.05f);
        }

        RaycastHit2D hit = Physics2D.Raycast(flashlightPosition, FR_normalized, distance);

        //has collision 
        if (hit.collider != null && !isColliding)
        {
            isColliding = true;

            //metal is obstacle type = 1 
            if (hit.collider.gameObject.name.ToLower().Contains("metal"))
            {
                ObstacleType = 1;
            }

            Vector2 hitPoint = hit.point;

            if (ObstacleType == 1)
            {
                //reflected road1_1
                Road1_1 = Instantiate(Road);
                Road1_1.transform.localScale = new Vector2(5.0f, 0.05f);
                Road1_1.transform.position = hitPoint;
                float roadRotation = Road.transform.eulerAngles.z;
                Road1_1.transform.rotation = Quaternion.Euler(0, 0, roadRotation + 120f); 
                Road1_1.transform.position += Road1_1.transform.right * 2.0f;
                Road1_1.transform.position += Road1_1.transform.up * 0.05f;

                //new road2_1
                Road2_1 = Instantiate(Road2);
                float lengthOfRoad1 = Road1.transform.localScale.x;
                float Road2_1_length = lengthOfRoad1 - 1.5f;     
                Road2_1.transform.localScale = new Vector2(Road2_1_length, Road2.transform.localScale.y);
                Vector2 flashlightTip = (Vector2)FlashLight.position + (Vector2)FlashLight.right * FlashLight.localScale.x * 0.5f;
                Road2_1.transform.position = flashlightTip;
                Road2_1.transform.position += Road2_1.transform.up * 0.55f;
                Road2_1.transform.right = FlashLight.right;
                Road2_1.transform.position = (Vector2)Road2_1.transform.position + (Vector2)Road2_1.transform.right * Road2_1.transform.localScale.x * 0.5f;

                //reflected road2_2
                float lengthOfRoad1_1 = Road1_1.transform.localScale.x;
                float Road2_2_length = lengthOfRoad1_1 * 0.6f;
                Road2_2 = Instantiate(Road2);
                Road2_2.transform.localScale = new Vector2(Road2_2_length, Road1_1.transform.localScale.y);
                Road2_2.transform.rotation = Road1_1.transform.rotation * Quaternion.Euler(0, 0, 180);
                Vector2 endOfRoad2_1 = (Vector2)Road2_1.transform.position + (Vector2)Road2_1.transform.right * Road2_1.transform.localScale.x * 0.5f; 
                Vector2 startOfRoad2_2 = endOfRoad2_1 - (Vector2)Road2_2.transform.right * Road2_2.transform.localScale.x * 0.5f;
                Road2_2.transform.position = startOfRoad2_2;

                Road2.SetActive(false);
            }
            ObstacleType = 0;
        }
        else if (hit.collider == null)
        {
            Destroy(Road1_1);
            Destroy(Road2_1);
            Destroy(Road2_2);
            isColliding = false;
            Road2.SetActive(true); 
            lastSafeDistance = Mathf.Infinity;
        }
    }
}


