using UnityEngine;

public class Refraction2 : MonoBehaviour
{   
    public GameObject player;
    public Transform FlashLight;
    private GameObject Road1;
    private GameObject Road1_1;
    private GameObject Road2;
    private GameObject Road2_1;
    public SpriteRenderer roadRenderer;
    private bool isColliding1 = false;
    private bool isColliding2 = false;
    private bool roadsAreFixed = false;
    public bool isCollidingWithPlayer;
    private float lastSafeDistance = 0f;
    private int ObstacleType = 0;
    private Vector2 lastPosition;
    private GameObject lastHitGlass = null;

    void Start()
    {
        Road1 = new GameObject("Road1");
        InitializeRoad(Road1);
        Road1.layer = LayerMask.NameToLayer("Ground");


        Road2 = new GameObject("Road2");
        InitializeRoad(Road2);
        Road2.layer = LayerMask.NameToLayer("Ground");

        roadsAreFixed = false;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isCollidingWithPlayer = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isCollidingWithPlayer = false;
        }
    }

    void InitializeRoad(GameObject Road)
    {    
        Road.transform.localScale = new Vector2(0, 0);
        roadRenderer = Road.AddComponent<SpriteRenderer>();
        Sprite defaultSprite = Resources.Load<Sprite>("Square");
        roadRenderer.sprite = defaultSprite;
        roadRenderer.color = Color.yellow;
        roadRenderer.sortingOrder = 1; 
        
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
                ToggleColliders(roadsAreFixed); // Toggle colliders based on roadsAreFixed

                if (roadsAreFixed)
                {
                    player.GetComponent<BatteryController>().batteryLevel--;
                }
            }
            
            if (!roadsAreFixed)
            {
                UpdateRoad1(Road1, Road1_1);
                UpdateRoad2(Road2, Road2_1);
            }
        }
    }

    void ToggleColliders(bool state) 
    {
        // This method toggles the colliders for all roads
        Road1.GetComponent<BoxCollider2D>().enabled = state;
        Road2.GetComponent<BoxCollider2D>().enabled = state;

        if (Road1_1) Road1_1.GetComponent<BoxCollider2D>().enabled = state;
        if (Road2_1) Road2_1.GetComponent<BoxCollider2D>().enabled = state;
    }

    void UpdateRoad1(GameObject Road, GameObject NewRoad)
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastPosition = Vector2.Lerp(lastPosition, mousePosition, Time.deltaTime * 2.0f);
        Vector2 flashlightRight = new Vector2(FlashLight.right.x, FlashLight.right.y);
        Vector2 FR_normalized = flashlightRight.normalized;
        Vector2 flashlightPosition = FlashLight.position;
        Vector2 FM = flashlightPosition - lastPosition;
        float distance = Mathf.Abs(Vector2.Dot(FM, FR_normalized));

        if (!isColliding1 || distance < lastSafeDistance)
        {
            Vector2 N = flashlightPosition + FR_normalized * distance;
            Vector2 midPoint = (N + flashlightPosition) / 2f;
            Road.transform.position = new Vector2(midPoint.x, midPoint.y);
            Road.transform.position -= FlashLight.up * 0.5f;
            Road.transform.localScale = new Vector2(distance, 0.05f);
            lastSafeDistance = distance;

        }

        RaycastHit2D hit = Physics2D.Raycast(flashlightPosition, FR_normalized, distance);

        if (hit.collider != null && !isColliding1)
        {
            isColliding1 = true;

            if (hit.collider.gameObject.name.ToLower().Contains("glass")){
                ObstacleType = 2;
            }

            Vector2 hitPoint = hit.point;

            if (ObstacleType == 2){

                Vector2 road1Position = Road1.transform.position;
                Vector2 road1Scale = Road1.transform.localScale;
                float road1Rotation = Road1.transform.eulerAngles.z;
                float road1_1Roation = 0;
                Vector2 roadDirection = new Vector2(Mathf.Cos(road1Rotation * Mathf.Deg2Rad), Mathf.Sin(road1Rotation * Mathf.Deg2Rad));
                bool isPointingRight = road1Rotation > 90 && road1Rotation < 270;

                if (isPointingRight){
                    roadDirection = roadDirection;
                }

                Vector2 roadEndPoint = road1Position + roadDirection * (road1Scale.x * 0.5f);
                Road1_1 = Instantiate(Road);

                BoxCollider2D road1_1Collider = Road1_1.GetComponent<BoxCollider2D>();
                road1_1Collider.enabled = false;

                Road1_1.transform.localScale = new Vector2(5.0f, 0.05f); 
                Road1_1.transform.position = roadEndPoint; 
                Road1_1.transform.rotation = Quaternion.Euler(0, 0, road1_1Roation);
                Vector2 adjustment = new Vector2(Road1_1.transform.localScale.x * 0.5f, 0);
                adjustment = isPointingRight ? -adjustment : adjustment;
                Road1_1.transform.position = (Vector2) Road1_1.transform.position + adjustment;

                lastHitGlass = hit.collider.gameObject;
                SetGlassRigidbodyType(hit.collider.gameObject, RigidbodyType2D.Dynamic);
            }
            ObstacleType = 0;
        }
        else if (hit.collider == null){
            Destroy(Road1_1);
            isColliding1 = false;
            lastSafeDistance = Mathf.Infinity;
            if (lastHitGlass != null)
            {
                SetGlassRigidbodyType(lastHitGlass, RigidbodyType2D.Kinematic);
                lastHitGlass = null; 
            }
        }
    }

    void UpdateRoad2(GameObject Road, GameObject NewRoad)
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastPosition = Vector2.Lerp(lastPosition, mousePosition, Time.deltaTime * 2.0f);
        Vector2 flashlightRight = new Vector2(FlashLight.right.x, FlashLight.right.y);
        Vector2 FR_normalized = flashlightRight.normalized;
        Vector2 flashlightPosition = FlashLight.position;
        Vector2 FM = flashlightPosition - lastPosition;
        float distance = Mathf.Abs(Vector2.Dot(FM, FR_normalized));

        if (!isColliding2 || distance < lastSafeDistance){
            Vector2 N = flashlightPosition + FR_normalized * distance;
            Vector2 midPoint = (N + flashlightPosition) / 2f;
            Road.transform.position = new Vector2(midPoint.x, midPoint.y);
            Road.transform.position += FlashLight.up * 0.5f;
            Road.transform.localScale = new Vector2(distance, 0.05f);
            lastSafeDistance = distance;
        }

        RaycastHit2D hit = Physics2D.Raycast(flashlightPosition, FR_normalized, distance);

        if (hit.collider != null && !isColliding2)
        {
            isColliding2 = true;

            if (hit.collider.gameObject.name.ToLower().Contains("glass"))
            {
                ObstacleType = 2;
            }
            Vector2 hitPoint = hit.point;
            if (ObstacleType == 2)
            {
                Vector2 road2Position = Road2.transform.position;
                Vector2 road2Scale = Road2.transform.localScale;
                float road2Rotation = Road2.transform.eulerAngles.z;
                float road2_1Roation = 0;


                Vector2 roadDirection2 = new Vector2(Mathf.Cos(road2Rotation * Mathf.Deg2Rad), Mathf.Sin(road2Rotation * Mathf.Deg2Rad));

                bool isPointingRight2 = road2Rotation > 90 && road2Rotation < 270;
                if (isPointingRight2)
                {
                    roadDirection2 = roadDirection2;
                }

                Vector2 roadEndPoint2 = road2Position + roadDirection2 * (road2Scale.x * 0.5f);

                Road2_1 = Instantiate(Road2);

                BoxCollider2D road2_1Collider = Road2_1.GetComponent<BoxCollider2D>();
                road2_1Collider.enabled = false;

                Road2_1.transform.localScale = new Vector2(5.0f, 0.05f); 
                Road2_1.transform.position = roadEndPoint2; 
                Road2_1.transform.rotation = Quaternion.Euler(0, 0, road2_1Roation);

                Vector2 adjustment2 = new Vector2(Road2_1.transform.localScale.x * 0.5f, 0);
                adjustment2 = isPointingRight2 ? -adjustment2 : adjustment2;
                Road2_1.transform.position = (Vector2) Road2_1.transform.position + adjustment2;

                lastHitGlass = hit.collider.gameObject;
                SetGlassRigidbodyType(hit.collider.gameObject, RigidbodyType2D.Dynamic);

            }
            ObstacleType = 0;
        }
        else if (hit.collider == null)
        {
            Destroy(Road2_1);
            isColliding2 = false;
            lastSafeDistance = Mathf.Infinity;
            if (lastHitGlass != null)
            {
                SetGlassRigidbodyType(lastHitGlass, RigidbodyType2D.Kinematic);
                lastHitGlass = null; 
            }
        }
    }

    void SetGlassRigidbodyType(GameObject glass, RigidbodyType2D type)
    {
        Rigidbody2D rb = glass.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = type;
        }
    }
}


