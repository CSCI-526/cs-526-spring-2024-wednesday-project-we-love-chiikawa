using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEWReflection : MonoBehaviour
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
    private bool isColliding = false;//if is colliding, road stop growing
    private bool Colliding1 = false;
    private bool Colliding2 = false;
    public bool fixedRoads = false;
    public float maxChangePerFrame = 0.5f; // 每帧最大长度或位置变化

    void Start()
    {
        Road1 = new GameObject("Road1");
        InitializeRoad(Road1);
        Road1.layer = LayerMask.NameToLayer("Ground");

        Road2 = new GameObject("Road2");
        InitializeRoad(Road2);
        Road2.layer = LayerMask.NameToLayer("Ground");


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
            if (!roadsAreFixed)
            {
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
            if (!roadsAreFixed)
            {

                ShowRoad(false);
            }
            else
            {
                ShowRoad(true);

            }
        }
        if (!roadsAreFixed && lastHitGlass != null)
        {
            SetGlassRigidbodyType(lastHitGlass, RigidbodyType2D.Kinematic);
        }
        if (roadsAreFixed && lastHitGlass != null)
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

        if (isPointingLeftNow)
        {
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
                length += expandSpeed * Time.deltaTime;
                road.transform.localScale = new Vector2(length, 0.05f);
                road.transform.position = roadBasePosition + (direction * (length / 2));

                yield return new WaitForSeconds(0.01f / expandSpeed);
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

        if (isPointingLeftNow)
        {
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
                road.transform.localScale = new Vector2(length, 0.05f);
                road.transform.position = roadBasePosition + (direction * (length / 2));

                yield return new WaitForSeconds(0.01f / expandSpeed);
            }
        }
    }

    void UpdateRoad1(GameObject Road, GameObject NewRoad)
    {
        RaycastHit2D hit;
        Vector2 offset = (Vector2)FlashLight.transform.up * 0.4f; // 偏移量
        Vector2 adjustedPosition = (Vector2)FlashLight.position - offset; // 应用偏移量
        hit = Physics2D.Raycast(adjustedPosition, FlashLight.right, 0);

        //R1和R2均未碰撞
        if (Colliding2 != true && isColliding != true)
        {
            // 自动延长道路的长度
            Road.transform.localScale = new Vector2(Road.transform.localScale.x + expandSpeed * Time.deltaTime, 0.05f);
            Road.transform.position = new Vector2(FlashLight.transform.position.x, FlashLight.transform.position.y);
            Road.transform.position -= Road.transform.up * 0.5f;
            //由于Road的中心在pivot上，所以要沿着FlashLight.right的方向移动一半的长度
            Road.transform.position += Road.transform.right * Road.transform.localScale.x * 0.5f;
            BoxCollider2D collider = Road.GetComponent<BoxCollider2D>();
            SpriteRenderer spriteRenderer = Road.GetComponent<SpriteRenderer>();
            //collider的size和offset
            if (spriteRenderer != null && collider != null)
            {
                collider.size = spriteRenderer.size;
                collider.offset = spriteRenderer.bounds.center - Road.transform.position;
            }

            hit = Physics2D.Raycast(adjustedPosition, FlashLight.right, Road.transform.localScale.x);
            Debug.DrawLine(adjustedPosition, adjustedPosition + (Vector2)(FlashLight.right * Road.transform.localScale.x), Color.green, duration: 20.0f);

            isColliding = false;
            Colliding1 = false;
            Destroy(Road1_1);
        }

        //如果R2先撞上了，更新R1和R1_1
        if (Colliding2 == true)
        {
            Road.transform.localScale = new Vector2(Road2.transform.localScale.x - 1.8f, 0.05f);
            Road.transform.position = new Vector2(Road2.transform.position.x, Road2.transform.position.y);
            Road.transform.position -= Road.transform.up * 1.0f;
            Road.transform.position -= Road.transform.right * 0.9f;
            BoxCollider2D Road1collider = Road1.GetComponent<BoxCollider2D>();
            SpriteRenderer sprite1Renderer = Road1.GetComponent<SpriteRenderer>();
            if (sprite1Renderer != null && Road1collider != null)
            {
                Road1collider.size = sprite1Renderer.size;
                Road1collider.offset = sprite1Renderer.bounds.center - Road1.transform.position;
            }

            Vector2 roadDirection = new Vector2(Mathf.Cos(Road.transform.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(Road.transform.eulerAngles.z * Mathf.Deg2Rad));
            Vector2 roadEndPoint = (Vector2)Road.transform.position + roadDirection * (Road.transform.localScale.x * 0.5f);
            Road1_1 = Instantiate(Road);
            Road1_1.transform.localScale = new Vector2(3.0f, 0.05f);
            Road1_1.transform.position = roadEndPoint;
            Road1_1.transform.rotation = Quaternion.Euler(0, 0, Road.transform.eulerAngles.z - 120f);
            Road1_1.transform.position += Road1_1.transform.right * 1.5f;
            BoxCollider2D Road1_1collider = Road1_1.GetComponent<BoxCollider2D>();
            SpriteRenderer sprite1_1Renderer = Road1_1.GetComponent<SpriteRenderer>();
            if (sprite1_1Renderer != null && Road1_1collider != null)
            {
                Road1_1collider.size = sprite1_1Renderer.size;
                Road1_1collider.offset = sprite1_1Renderer.bounds.center - Road1_1.transform.position;
            }
            return;
        }

        //R1先撞上了,生成R1_1
        else if (hit.collider != null)
        {
            isColliding = true;
            Colliding1 = true;
            Destroy(Road1_1);
            Vector2 roadPosition = Road.transform.position;
            Vector2 roadScale = Road.transform.localScale;
            float roadRotation = Road.transform.eulerAngles.z;
            Vector2 roadDirection = new Vector2(Mathf.Cos(roadRotation * Mathf.Deg2Rad), Mathf.Sin(roadRotation * Mathf.Deg2Rad));
            // Since Road1 is centered on its pivot, you need to move half its length to get to the tip, assuming it's horizontal
            Vector2 roadEndPoint = roadPosition + roadDirection * (roadScale.x * 0.5f);

            // Instantiate Road1_1 and set its properties
            Road1_1 = Instantiate(Road);
            Road1_1.transform.localScale = new Vector2(3.0f, 0.05f);
            Road1_1.transform.position = roadEndPoint; // Initially set position to Road1's endpoint
            // 将 Road1_1 的旋转角度设置为 Road 的旋转角度逆时针转 120度
            Road1_1.transform.rotation = Quaternion.Euler(0, 0, roadRotation + 120f); //这个角度是相对x轴而言的角度
            Road1_1.transform.position += Road1_1.transform.right * 1.5f;
        }

    }

    void UpdateRoad2(GameObject Road, GameObject NewRoad)
    {
        RaycastHit2D hit;
        Vector2 offset = (Vector2)FlashLight.transform.up * 0.4f; // 偏移量
        Vector2 adjustedPosition = (Vector2)FlashLight.position + offset; // 应用偏移量
        hit = Physics2D.Raycast(adjustedPosition, FlashLight.right, 0);

        //R1和R2均未碰撞
        if (Colliding1 != true && isColliding != true)
        {
            // 自动延长道路的长度
            Road.transform.localScale = new Vector2(Road.transform.localScale.x + expandSpeed * Time.deltaTime, 0.05f);
            Road.transform.position = new Vector2(FlashLight.transform.position.x, FlashLight.transform.position.y);
            Road.transform.position += Road.transform.up * 0.5f;
            //由于Road的中心在pivot上，所以要沿着FlashLight.right的方向移动一半的长度
            Road.transform.position += Road.transform.right * Road.transform.localScale.x * 0.5f;
            BoxCollider2D collider = Road.GetComponent<BoxCollider2D>();
            SpriteRenderer spriteRenderer = Road.GetComponent<SpriteRenderer>();
            //collider的size和offset
            if (spriteRenderer != null && collider != null)
            {
                collider.size = spriteRenderer.size;
                collider.offset = spriteRenderer.bounds.center - Road.transform.position;
            }

            hit = Physics2D.Raycast(adjustedPosition, FlashLight.right, Road.transform.localScale.x);
            Debug.DrawLine(adjustedPosition, adjustedPosition + (Vector2)(FlashLight.right * Road.transform.localScale.x), Color.green, duration: 20.0f);

            isColliding = false;
            Colliding2 = false;
            Destroy(Road2_1);
        }


        //R1先撞上了，更新R2和R2_1
        if (Colliding1 == true)
        {
            //更新R2  
            Road.transform.localScale = new Vector2(Road1.transform.localScale.x - 1.8f, 0.05f);
            Road.transform.position = new Vector2(Road1.transform.position.x, Road1.transform.position.y);
            //R2在R1的上面
            Road.transform.position += Road.transform.up * 1.0f;
            Road.transform.position -= Road.transform.right * 0.9f;
            BoxCollider2D Road2collider = Road2.GetComponent<BoxCollider2D>();
            SpriteRenderer sprite2Renderer = Road2.GetComponent<SpriteRenderer>();
            if (sprite2Renderer != null && Road2collider != null)
            {
                Road2collider.size = sprite2Renderer.size;
                Road2collider.offset = sprite2Renderer.bounds.center - Road2.transform.position;
            }

            Vector2 roadDirection = new Vector2(Mathf.Cos(Road.transform.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(Road.transform.eulerAngles.z * Mathf.Deg2Rad));
            Vector2 roadEndPoint = (Vector2)Road.transform.position + roadDirection * (Road.transform.localScale.x * 0.5f);
            Road2_1 = Instantiate(Road);
            Road2_1.transform.localScale = new Vector2(3.0f, 0.05f);
            Road2_1.transform.position = roadEndPoint;
            Road2_1.transform.rotation = Quaternion.Euler(0, 0, Road.transform.eulerAngles.z + 120f); //这个角度是相对x轴而言的角度
            Road2_1.transform.position += Road2_1.transform.right * 1.5f;
            BoxCollider2D Road2_1collider = Road2_1.GetComponent<BoxCollider2D>();
            SpriteRenderer sprite2_1Renderer = Road2_1.GetComponent<SpriteRenderer>();
            if (sprite2_1Renderer != null && Road2_1collider != null)
            {
                Road2_1collider.size = sprite2_1Renderer.size;
                Road2_1collider.offset = sprite2_1Renderer.bounds.center - Road2_1.transform.position;
            }
            return;
        }

        //R2先撞了，生成R2_1
        else if (hit.collider != null)
        {
            isColliding = true;
            Colliding2 = true;
            Destroy(Road2_1);

            Vector2 roadPosition = Road.transform.position;
            Vector2 roadScale = Road.transform.localScale;
            float roadRotation = Road.transform.eulerAngles.z;
            Vector2 roadDirection = new Vector2(Mathf.Cos(roadRotation * Mathf.Deg2Rad), Mathf.Sin(roadRotation * Mathf.Deg2Rad));
            Vector2 roadEndPoint = roadPosition + roadDirection * (roadScale.x * 0.5f);

            // Instantiate Road1_1 and set its properties
            Road2_1 = Instantiate(Road);
            Road2_1.transform.localScale = new Vector2(3.0f, 0.05f);
            Road2_1.transform.position = roadEndPoint; // Initially set position to Road1's endpoint
            // 将 Road2_1 的旋转角度设置为 Road 的旋转角度顺时针转 120度
            Road2_1.transform.rotation = Quaternion.Euler(0, 0, roadRotation - 120f); //这个角度是相对x轴而言的角度
            Road2_1.transform.position += Road2_1.transform.right * 1.5f;

        }

    }



    void ShowRoad(bool visible)
    {
        if (Road1 != null)
        {
            Road1.SetActive(visible);
            if (Road1_1 != null && !visible)
            {
                Destroy(Road1_1);
                Road1_1 = null;
            }
        }

        if (Road2 != null)
        {
            Road2.SetActive(visible);
            if (Road2_1 != null && !visible)
            {
                Destroy(Road2_1);
                Road2_1 = null;
            }
        }
    }




    void FixRoadsInPlace()
    {
        // Stop extending the roads if they are currently being extended
        StopAllCoroutines();

        // Enable colliders for all 4 roads
        ToggleColliders(Road1, true);
        if (Road1_1 != null) ToggleColliders(Road1_1, true);
        ToggleColliders(Road2, true);
        if (Road2_1 != null) ToggleColliders(Road2_1, true);

        ShowRoad(true);
    }

    void ToggleColliders(GameObject road, bool state)
    {
        if (road != null)
        {
            BoxCollider2D collider = road.GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                collider.enabled = state;
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
