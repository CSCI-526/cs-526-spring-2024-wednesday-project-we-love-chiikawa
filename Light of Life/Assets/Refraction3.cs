using UnityEngine;

public class Refraction3 : MonoBehaviour
{
    public GameObject player;
    public Transform FlashLight;
    public float flashlightLength = 1.0f;
    public float roadVerticalOffset = -0.5f;
    private GameObject Road1;
    private GameObject Road1_1;
    private GameObject Road2;
    private GameObject Road2_1;
    public bool isCollidingWithPlayer;
    public LayerMask obstacleLayer;
    public SpriteRenderer roadRenderer;

    private bool roadsAreFixed = false;
    private GameObject lastHitGlass = null;
    private bool roadsFixed = false;
    public float expandSpeed = 10.0f;

    void Start()
    {
        Road1 = new GameObject("Road1");
        InitializeRoad(Road1);
        Road1.layer = LayerMask.NameToLayer("Ground");

        Road2 = new GameObject("Road2");
        InitializeRoad(Road2);
        Road2.layer = LayerMask.NameToLayer("Ground");

        //roadsAreFixed = false;
    }

    void InitializeRoad(GameObject Road)
    {    
        Road.transform.localScale = new Vector2(0, 0);
        roadRenderer = Road.AddComponent<SpriteRenderer>();
        Sprite defaultSprite = Resources.Load<Sprite>("Square");
        roadRenderer.sprite = defaultSprite;
        roadRenderer.color = Color.yellow;
        
        Road.transform.position = FlashLight.position;     
        Road.transform.up = FlashLight.up;

        BoxCollider2D roadCollider = Road.AddComponent<BoxCollider2D>();
        roadCollider.size = new Vector2(1, 0.1f);
        roadCollider.enabled = false;
        
    }

void Update()
{
    if (isCollidingWithPlayer)
    {
        if (Input.GetKeyDown(KeyCode.F) && player.GetComponent<BatteryController>().batteryLevel > 0)
        {
            roadsAreFixed = !roadsAreFixed;
            ShowRoad(true);
            FixRoadsInPlace();

            if (roadsAreFixed && player.GetComponent<BatteryController>().batteryLevel > 0)
            {
                player.GetComponent<BatteryController>().batteryLevel--;
            }
        
        }
    }

}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isCollidingWithPlayer = true;
            ShowRoad(true);
            if(!roadsAreFixed){
                StartExtendingRoad1();
                StartExtendingRoad2();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isCollidingWithPlayer = false;
            if(!roadsAreFixed){

                ShowRoad(false);
            }
            else{
                ShowRoad(true);
            
            }
        }
        if(!roadsAreFixed && lastHitGlass != null)
    {
        SetGlassRigidbodyType(lastHitGlass, RigidbodyType2D.Kinematic);
    }
    if(roadsAreFixed && lastHitGlass != null)
    {
        SetGlassRigidbodyType(lastHitGlass, RigidbodyType2D.Dynamic);
    }
    }



    void StartExtendingRoad1()
    {

        StartCoroutine(ExtendRoad1(Road1, FlashLight.right.normalized, flashlightLength));
    }

    void StartExtendingRoad2()
    {
        StartCoroutine(ExtendRoad2(Road2, FlashLight.right.normalized, -flashlightLength));
    }

    System.Collections.IEnumerator ExtendRoad1(GameObject road, Vector2 direction, float verticalOffset)
    {
        float length = 0;
        bool hitWall = false;
        Vector2 roadBasePosition = (Vector2)FlashLight.position + (Vector2)FlashLight.up * flashlightLength;

        float road1RotationNow = Road1.transform.eulerAngles.z;
        bool isPointingLeftNow = road1RotationNow > 90 && road1RotationNow < 270;

        if (isPointingLeftNow){
            roadBasePosition.y += 0.5f;
            roadBasePosition.x += 0.2f;
        
        }
        else
        {
            roadBasePosition.y += -0.5f;
            roadBasePosition.x += 0.1f;
        }

        while (!hitWall && isCollidingWithPlayer)
        {
            RaycastHit2D hit = Physics2D.Raycast(roadBasePosition + (direction * length), direction, 0.1f, obstacleLayer);
            if (hit.collider != null)
            {
                hitWall = true;
                UpdateRoad1(Road1, Road1_1);
                lastHitGlass = hit.collider.gameObject;
            }
            else
            {
                length += expandSpeed * Time.deltaTime; // Using expandSpeed to control the extension rate
                road.transform.localScale = new Vector2(length, 0.1f);
                road.transform.position = roadBasePosition + (direction * (length / 2));

                yield return new WaitForSeconds(0.01f / expandSpeed); // Faster extension as expandSpeed increases
            }
        }
    }

    System.Collections.IEnumerator ExtendRoad2(GameObject road, Vector2 direction, float verticalOffset)
    {
        
        float length = 0;
        bool hitWall = false;
        Vector2 roadBasePosition = (Vector2)FlashLight.position - (Vector2)FlashLight.up * flashlightLength;

        float road1RotationNow = Road1.transform.eulerAngles.z;
        bool isPointingLeftNow = road1RotationNow > 90 && road1RotationNow < 270;

        if (isPointingLeftNow){
            roadBasePosition.y -= 0.5f;
            roadBasePosition.x -= 0.2f;
        }
        else
        {
            roadBasePosition.y -= -0.5f;
            roadBasePosition.x -= 0.2f;
        }

        while (!hitWall && isCollidingWithPlayer)
        {
            RaycastHit2D hit = Physics2D.Raycast(roadBasePosition + (direction * length), direction, 0.1f, obstacleLayer);
            //hits
            if (hit.collider != null)
            {
                hitWall = true;
                lastHitGlass = hit.collider.gameObject;
                UpdateRoad2(Road2, Road2_1);
            }
            else
            {
                length += expandSpeed * Time.deltaTime; // Using expandSpeed to control the extension rate
                road.transform.localScale = new Vector2(length, 0.1f);
                road.transform.position = roadBasePosition + (direction * (length / 2));

                yield return new WaitForSeconds(0.01f / expandSpeed); // Faster extension as expandSpeed increases
            }
        }
    }

    void UpdateRoad1(GameObject Road, GameObject NewRoad)
    {
        Vector2 road1Position = Road1.transform.position;
        Vector2 road1Scale = Road1.transform.localScale;
        float road1Rotation = Road1.transform.eulerAngles.z;
        float road1_1Roation = 0;
        Vector2 roadDirection = new Vector2(Mathf.Cos(road1Rotation * Mathf.Deg2Rad), Mathf.Sin(road1Rotation * Mathf.Deg2Rad));
        bool isPointingLeft = road1Rotation > 90 && road1Rotation < 270;

        Vector2 roadEndPoint = road1Position + roadDirection * (road1Scale.x * 0.5f);

        if (isPointingLeft){
            roadDirection = roadDirection;
        }
        
        Road1_1 = Instantiate(Road);

        BoxCollider2D road1_1Collider = Road1_1.GetComponent<BoxCollider2D>();
        road1_1Collider.enabled = false;

        Road1_1.transform.localScale = new Vector2(3.0f, 0.05f); 
        Road1_1.transform.position = roadEndPoint; 
        Road1_1.transform.rotation = Quaternion.Euler(0, 0, road1_1Roation);
        Vector2 adjustment = new Vector2(Road1_1.transform.localScale.x * 0.5f, 0);
        adjustment = isPointingLeft ? -adjustment : adjustment;
        Road1_1.transform.position = (Vector2) Road1_1.transform.position + adjustment;

        //(lastHitGlass, RigidbodyType2D.Dynamic);

    }

       void UpdateRoad2(GameObject Road, GameObject NewRoad)
    {
        Vector2 road2Position = Road2.transform.position;
        Vector2 road2Scale = Road2.transform.localScale;
        float road2Rotation = Road2.transform.eulerAngles.z;
        float road2_1Roation = 0;


        Vector2 roadDirection2 = new Vector2(Mathf.Cos(road2Rotation * Mathf.Deg2Rad), Mathf.Sin(road2Rotation * Mathf.Deg2Rad));

        bool isPointingLeft2 = road2Rotation > 90 && road2Rotation < 270;
        if (isPointingLeft2)
        {
            
            roadDirection2 = roadDirection2;
        }

        Vector2 roadEndPoint2 = road2Position + roadDirection2 * (road2Scale.x * 0.5f);

        Road2_1 = Instantiate(Road2);

        BoxCollider2D road2_1Collider = Road2_1.GetComponent<BoxCollider2D>();
        road2_1Collider.enabled = false;

        Road2_1.transform.localScale = new Vector2(3.0f, 0.05f); 
        Road2_1.transform.position = roadEndPoint2; 
        Road2_1.transform.rotation = Quaternion.Euler(0, 0, road2_1Roation);

        Vector2 adjustment2 = new Vector2(Road2_1.transform.localScale.x * 0.5f, 0);
        adjustment2 = isPointingLeft2 ? -adjustment2 : adjustment2;
        Road2_1.transform.position = (Vector2) Road2_1.transform.position + adjustment2;

        //SetGlassRigidbodyType(lastHitGlass, RigidbodyType2D.Dynamic);
    }



void ShowRoad(bool visible)
{
    if (Road1 != null)
    {
        Road1.SetActive(visible);
        //Road1_1.SetActive(visible);
        // Destroy the extended road if the main road is being hidden
        if (Road1_1 != null && !visible)
        {
            Destroy(Road1_1);
            Road1_1 = null; // Clear the reference
        }
    }

    if (Road2 != null)
    {
        Road2.SetActive(visible);
        //Road2_1.SetActive(visible);
        // Destroy the extended road if the main road is being hidden
        if (Road2_1 != null && !visible)
        {
            Destroy(Road2_1);
            Road2_1 = null; // Clear the reference
        }
    }
}




void FixRoadsInPlace()
{
    // Stop extending the roads if they are currently being extended
    StopAllCoroutines(); 

    // Enable colliders for all 4 roads
    ToggleColliders(Road1, true);
    if (Road1_1 != null) ToggleColliders(Road1_1, true); // Check for null in case the road hasn't been initialized
    ToggleColliders(Road2, true);
    if (Road2_1 != null) ToggleColliders(Road2_1, true); // Check for null in case the road hasn't been initialized

    // Ensure the roads are visible
    ShowRoad(true);
}

void ToggleColliders(GameObject road, bool state)
{
    if (road != null) // Check if the road object is not null
    {
        BoxCollider2D collider = road.GetComponent<BoxCollider2D>();
        if (collider != null) // Check if the road has a collider component
        {
            collider.enabled = state; // Enable or disable the collider based on the 'state' parameter
        }
    }
}

    void SetGlassRigidbodyType(GameObject glass, RigidbodyType2D type)
    {
        Debug.Log("Inside setGlass. Glass is " + glass);
        Debug.Log("Inside setGlass. Glass rb is " + glass.GetComponent<Rigidbody2D>());
        Rigidbody2D rb = glass.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = type;
        }
    }

}

