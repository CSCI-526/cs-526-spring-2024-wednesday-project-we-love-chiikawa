using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Left : MonoBehaviour
{
    /*
* 每个游戏对象都有一个 Transform 组件，它负责保存和管理游戏对象的空间属性。
* 而 GameObject 则是游戏对象的类，代表了场景中的实体，它可以具有多个组件，其中一个就是 Transform。
因此，当你声明 FlashLight 为 public Transform 时，它实际上是指向了一个游戏对象的位置、旋转和比例等信息，而不是游戏对象本身。
* */
    public Transform FlashLight;
    [HideInInspector]
    public int batteryLevel = 5;
    private GameObject Road1;
    private GameObject Road1_1;
    private GameObject Road2;
    private GameObject Road2_1;
    private GameObject Road3;
    private GameObject Road3_1;
    public SpriteRenderer roadRenderer;
    private bool isColliding = false;//if is colliding, road stop growing
    private float lastSafeDistance = 0f; // 记录上一次安全的（未碰撞的）距离。作用是让射线碰到障碍物后，还能缩短继续改变长度
    private int ObstacleType = 0; //障碍物类型
    private int CollidedRoad = 0;
    [HideInInspector]
    public bool fixedRoads = false;

    void Start()
    {
        //右边的路
        Road1 = new GameObject("Road1");
        InitializeRoad(Road1);
        BoxCollider2D Road1collider = Road1.AddComponent<BoxCollider2D>();
        Road1collider.size = new Vector2(1, 0.05f); // 初始大小，稍后会根据道路长度更新

        ////左边的路
        Road2 = new GameObject("Road2");
        InitializeRoad(Road2);
        BoxCollider2D Road2collider = Road2.AddComponent<BoxCollider2D>();
        Road2collider.size = new Vector2(0.05f, 0.05f);

        //中间的路（不需要collider）
        Road3 = new GameObject("Road3");
        InitializeRoad(Road3);

    }


    void InitializeRoad(GameObject Road)
    {
        // 创建Road，并添加Collider和组件
        roadRenderer = Road.AddComponent<SpriteRenderer>();
        Sprite defaultSprite = Resources.Load<Sprite>("Square");
        roadRenderer.sprite = defaultSprite;
        roadRenderer.color = Color.red;
        roadRenderer.sortingOrder = 1; //让道路渲染在障碍物的上面，不会被挡住
        //方向
        Road.transform.up = FlashLight.up;
    }


    void Update()
    {
        //按F键切换是否固定道路
        if (Input.GetKeyDown(KeyCode.F) && batteryLevel > 0)
        {
            fixedRoads = !fixedRoads;

            if (fixedRoads == true)
            {
                batteryLevel--;
                Debug.Log("Current battery level: " + batteryLevel);
            }
        }

        if (!fixedRoads)
        {
            //如果撞击到的是Road1（Road2的还没写）
            bool road1Collided = ExtendRoad1(Road1, 1) == 1;

            //1没撞到
            if (!road1Collided)
            {
                //大家都伸长
                ExtendRoad2(Road2, 2);
                ExtendRoad1(Road3, 3);
                Debug.Log("Road1没撞到");

                //看看2撞到了没
                bool road2Collided = ExtendRoad2(Road2, 2) == 2;

                //2撞到了
                if (road2Collided)
                {
                    Debug.Log("Road2撞到了");
                    //根据Road2_1更新另外两条Road的位置和长度
                    ReR1R3();
                }
            }

            //1撞到了
            else
            {
                Debug.Log("R1撞到了");
                //根据Road1_1更新另外两条Road的位置和长度
                ReR2R3();
            }
        }
    }
    void ReR2R3()
    {
        Debug.Log("ReR2R3");
        Road2.transform.localScale = new Vector2(Road1.transform.localScale.x - 1.5f, 0.05f); //减掉一截长度
        Road2.transform.position = Road1.transform.position + FlashLight.up * 1.0f; //更新位置
        Road2.transform.position -= Road2.transform.right * 0.75f;
        BoxCollider2D Road2collider = Road2.GetComponent<BoxCollider2D>();
        SpriteRenderer sprite2Renderer = Road2.GetComponent<SpriteRenderer>();
        //R2 collider的size和offset
        if (sprite2Renderer != null && Road2collider != null)
        {
            Road2collider.size = sprite2Renderer.size;
            Road2collider.offset = sprite2Renderer.bounds.center - Road2.transform.position;
        }

        Road3.transform.localScale = new Vector2(Road1.transform.localScale.x - 0.5f, 0.05f); //减掉一截长度
        Road3.transform.position = Road1.transform.position + FlashLight.up * 0.5f; //更新位置
        Road3.transform.position -= Road3.transform.right * 1.0f;

        //根据Road1_1更新另外两条Road的位置和长度
        if (Road1_1 != null)
        {
            Road2_1 = Instantiate(Road2);
            float roadRotation2 = Road2.transform.eulerAngles.z; //旋转R2_1
            Road2_1.transform.rotation = Quaternion.Euler(0, 0, roadRotation2 + 120f);
            Road2_1.transform.localScale = new Vector2(Road1_1.transform.localScale.x - 1.0f, 0.05f);
            Road2_1.transform.position = Road1_1.transform.position + Road1_1.transform.up * 1.0f;
            Road2_1.transform.position += Road2_1.transform.right * 1.0f;  //
            BoxCollider2D Road2_1collider = Road2_1.GetComponent<BoxCollider2D>();
            SpriteRenderer sprite2_1Renderer = Road2_1.GetComponent<SpriteRenderer>();
            //R2_1 collider的size和offset
            if (sprite2_1Renderer != null && Road2_1collider != null)
            {
                Road2_1collider.size = sprite2_1Renderer.size;
                Road2_1collider.offset = sprite2_1Renderer.bounds.center - Road2_1.transform.position;
            }

            Road3_1 = Instantiate(Road3);
            float roadRotation3 = Road3.transform.eulerAngles.z; //旋转R3_1
            Road3_1.transform.rotation = Quaternion.Euler(0, 0, roadRotation3 + 120f);
            Road3_1.transform.localScale = new Vector2(Road1_1.transform.localScale.x - 0.5f, 0.05f); //减掉一截长度
            Road3_1.transform.position = Road1_1.transform.position + Road1_1.transform.up * 0.5f; //更新位置
            Road3_1.transform.position += Road3_1.transform.right * 0.25f;
        }
    }

    void ReR1R3()
    {
        Debug.Log("ReR1R3");
        Road1.transform.localScale = new Vector2(Road2.transform.localScale.x - 1.5f, 0.05f); //减一截长度
        Road1.transform.position = Road2.transform.position - FlashLight.up * 1.0f; //更新位置
        Road1.transform.position -= Road2.transform.right * 0.75f;
        BoxCollider2D Road1collider = Road1.GetComponent<BoxCollider2D>();
        SpriteRenderer sprite1Renderer = Road1.GetComponent<SpriteRenderer>();
        //R1 collider的size和offset
        if (sprite1Renderer != null && Road1collider != null)
        {
            Road1collider.size = sprite1Renderer.size;
            Road1collider.offset = sprite1Renderer.bounds.center - Road1.transform.position;
        }

        Road3.transform.localScale = new Vector2(Road2.transform.localScale.x - 0.5f, 0.05f); //减掉一截长度
        Road3.transform.position = Road2.transform.position - FlashLight.up * 0.5f; //更新位置
        Road3.transform.position += Road3.transform.right * 0.25f;

        if (Road2_1 != null)
        {


            //根据Road2_1更新另外两条Road的位置和长度
            Road1_1 = Instantiate(Road1);
            float roadRotation1 = Road1.transform.eulerAngles.z; //旋转R1_1
            Road1_1.transform.rotation = Quaternion.Euler(0, 0, roadRotation1 + 60f);
            Road1_1.transform.localScale = new Vector2(Road2_1.transform.localScale.x - 1.0f, 0.05f);
            Road1_1.transform.position = Road2_1.transform.position + Road2_1.transform.up * 0.75f;
            Road1_1.transform.position -= Road1_1.transform.right * 1.25f;  //
            BoxCollider2D Road1_1collider = Road1_1.GetComponent<BoxCollider2D>();
            SpriteRenderer sprite1_1Renderer = Road1_1.GetComponent<SpriteRenderer>();
            //R2_1 collider的size和offset
            if (sprite1_1Renderer != null && Road1_1collider != null)
            {
                Road1_1collider.size = sprite1_1Renderer.size;
                Road1_1collider.offset = sprite1_1Renderer.bounds.center - Road1_1.transform.position;
            }

            Road3_1 = Instantiate(Road3);
            float roadRotation3 = Road3.transform.eulerAngles.z; //旋转R3_1
            Road3_1.transform.rotation = Quaternion.Euler(0, 0, roadRotation3 + 60f);
            Road3_1.transform.localScale = new Vector2(Road2_1.transform.localScale.x - 0.5f, 0.05f); //减掉一截长度
            Road3_1.transform.position = Road2_1.transform.position + Road2_1.transform.up * 0.5f; //更新位置
            Road3_1.transform.position += Road3_1.transform.right * 0.25f;
        }
    }


    //随着鼠标位置伸长road，碰撞后反射/折射

    int ExtendRoad1(GameObject Road, int RoadNo)
    {
        //必须加这段，不然反射光线不会消失嘟
        Destroy(Road2_1);
        Destroy(Road3_1);
        Destroy(Road1_1);
        //获取鼠标位置 计算Road的位置和长度
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Flashlight右面的法向量，并计算单位向量
        Vector2 flashlightRight = new Vector2(FlashLight.right.x, FlashLight.right.y);
        Vector2 FR_normalized = flashlightRight.normalized;
        //获取Flashlight的Vector2类型
        Vector2 flashlightPosition = FlashLight.position;
        //Flashlight到Mouse的向量FM
        Vector2 FM = flashlightPosition - mousePosition;
        // 计算FM在Fu上的投影的大小
        float distance = Mathf.Abs(Vector2.Dot(FM, FR_normalized));

        //未碰撞，随鼠标移动
        if (!isColliding || distance < lastSafeDistance)
        {
            //计算投影点N的坐标
            Vector2 N = flashlightPosition + FR_normalized * distance;
            Vector2 midPoint = (N + flashlightPosition) / 2f;

            //更新Road尺寸
            Road.transform.localScale = new Vector2(distance, 0.05f);

            //更新Road坐标和Collider尺寸
            if (RoadNo == 1)
            {
                Road.transform.position = new Vector2(midPoint.x, midPoint.y);
                Road.transform.position -= FlashLight.up * 0.5f;
                BoxCollider2D Roadcollider = Road.GetComponent<BoxCollider2D>();
                SpriteRenderer spriteRenderer = Road.GetComponent<SpriteRenderer>();
                //collider的size和offset
                if (spriteRenderer != null && Roadcollider != null)
                {
                    Roadcollider.size = spriteRenderer.size;
                    Roadcollider.offset = spriteRenderer.bounds.center - Road.transform.position;
                }
            }
            else
            {
                Road.transform.position = new Vector2(midPoint.x, midPoint.y);
            }
            // 更新上次安全距离
            lastSafeDistance = distance;
        }

        //检测碰撞
        //RaycastHit2D hit = Physics2D.Raycast(flashlightPosition-(Vector2)FlashLight.transform.up*0.5f, FR_normalized, distance);
        //RaycastHit2D hit = Physics2D.Raycast(flashlightPosition, FR_normalized, distance);
        RaycastHit2D hit;
        Vector2 offset = (Vector2)FlashLight.transform.up * 0.4f; // 偏移量
        Vector2 adjustedPosition = flashlightPosition - offset; // 应用偏移量
        hit = Physics2D.Raycast(adjustedPosition, FR_normalized, distance);
        Debug.DrawLine(adjustedPosition, adjustedPosition + FR_normalized * distance, Color.green, duration: 20.0f);


        //如果碰撞了
        if (hit.collider != null)
        {
            isColliding = true;
            Vector2 hitPoint = hit.point + (Vector2)FlashLight.transform.up * 0.4f;
            //如果碰撞的是metal，转120度
            //后序可以根据介质修改角度
            if (hit.collider.gameObject.name.ToLower().Contains("metal"))
            {
                Road1_1 = Instantiate(Road);
                //设置长度为5.0f
                Road1_1.transform.localScale = new Vector2(5.0f, 0.05f);
                BoxCollider2D Roadcollider = Road.GetComponent<BoxCollider2D>();
                Roadcollider.size = new Vector2(5.0f, 0.05f);
                SpriteRenderer spriteRenderer = Road.GetComponent<SpriteRenderer>();
                //collider的size和offset
                if (spriteRenderer != null && Roadcollider != null)
                {
                    Roadcollider.size = spriteRenderer.size;
                    Roadcollider.offset = spriteRenderer.bounds.center - Road.transform.position;
                }
                //position大致为撞击点
                Road1_1.transform.position = hitPoint;
                float roadRotation = Road.transform.eulerAngles.z;
                // 将 Road1_1 的旋转角度设置为 Road 的旋转角度逆时针转 120度
                Road1_1.transform.rotation = Quaternion.Euler(0, 0, roadRotation + 120f); //这个角度是相对x轴而言的角度
                Road1_1.transform.position += Road1_1.transform.right * 2.0f;//得这样调，本来是2.5的，但是会有个洞
                Road1_1.transform.position += Road1_1.transform.up * 0.05f;
            }

            return RoadNo; //返回碰撞的Road编号
        }
        // 如果没有碰撞，重置isColliding
        else
        {
            isColliding = false;
            Destroy(Road1_1);
            if (Road1_1 == null)
                Debug.Log("Road1_1妹有");
        }
        return 0; //返回0表示没有碰撞
    }
    int ExtendRoad2(GameObject Road, int RoadNo)
    {
        Destroy(Road2_1);
        Destroy(Road3_1);
        Destroy(Road1_1);

        //获取鼠标位置 计算鼠标到手电筒的距离
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 flashlightRight = new Vector2(FlashLight.right.x, FlashLight.right.y);
        Vector2 FR_normalized = flashlightRight.normalized;
        Vector2 flashlightPosition = FlashLight.position;
        Vector2 FM = flashlightPosition - mousePosition;
        float distance = Mathf.Abs(Vector2.Dot(FM, FR_normalized));

        //未碰撞，随鼠标移动
        if (!isColliding || distance < lastSafeDistance)
        {
            //计算投影点N的坐标
            Vector2 N = flashlightPosition + FR_normalized * distance;
            Vector2 midPoint = (N + flashlightPosition) / 2f;

            //更新Road尺寸
            Road.transform.localScale = new Vector2(distance, 0.05f);

            //更新Road坐标和Collider尺寸
            Road.transform.position = new Vector2(midPoint.x, midPoint.y);
            Road.transform.position += FlashLight.up * 0.5f;
            BoxCollider2D Roadcollider = Road.GetComponent<BoxCollider2D>();
            SpriteRenderer spriteRenderer = Road.GetComponent<SpriteRenderer>();
            //collider的size和offset
            if (spriteRenderer != null && Roadcollider != null)
            {
                Roadcollider.size = spriteRenderer.size;
                Roadcollider.offset = spriteRenderer.bounds.center - Road.transform.position;
            }
            // 更新上次安全距离
            lastSafeDistance = distance;
        }

        //检测碰撞
        RaycastHit2D hit2;
        Vector2 offset = (Vector2)FlashLight.transform.up * 0.4f; // 偏移量
        Vector2 adjustedPosition = flashlightPosition + offset; // 应用偏移量

        hit2 = Physics2D.Raycast(adjustedPosition, FR_normalized, distance);

        Debug.DrawLine(adjustedPosition, adjustedPosition + FR_normalized * distance, Color.green, duration: 20.0f);
        //如果碰撞了
        if (hit2.collider != null)
        {
            isColliding = true;
            Vector2 hit2Point = hit2.point + (Vector2)FlashLight.transform.up * 0.4f;

            if (hit2.collider.gameObject.name.ToLower().Contains("metal"))
            {
                Road2_1 = Instantiate(Road);
                //设置长度为5.0f
                Road2_1.transform.localScale = new Vector2(5.0f, 0.05f);
                BoxCollider2D Roadcollider = Road.GetComponent<BoxCollider2D>();
                Roadcollider.size = new Vector2(5.0f, 0.05f);
                SpriteRenderer spriteRenderer = Road.GetComponent<SpriteRenderer>();
                //collider的size和offset
                if (spriteRenderer != null && Roadcollider != null)
                {
                    Roadcollider.size = spriteRenderer.size;
                    Roadcollider.offset = spriteRenderer.bounds.center - Road.transform.position;
                }
                //position大致为撞击点
                Road2_1.transform.position = hit2Point;
                float roadRotation = Road.transform.eulerAngles.z;
                // 将 Road2_1 的旋转角度设置为 Road 的旋转角度逆时针60度
                Road2_1.transform.rotation = Quaternion.Euler(0, 0, roadRotation + 60f); //这个角度是相对x轴而言的角度
                Road2_1.transform.position -= Road2_1.transform.up * 0.1f;
                Road2_1.transform.position -= Road2_1.transform.right * 2.8f;//得这样调，本来是2.5的，但是会有个洞


            }
            Debug.Log("11111");
            return RoadNo; //返回碰撞的Road编号
        }
        // 如果没有碰撞，重置isColliding
        else
        {
            isColliding = false;
            Destroy(Road2_1);

        }
        return 0; //返回0表示没有碰撞
    }
}
