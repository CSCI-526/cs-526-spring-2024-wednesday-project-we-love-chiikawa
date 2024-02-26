using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;

public class DynamicRectangle : MonoBehaviour
{   /*
     * 每个游戏对象都有一个 Transform 组件，它负责保存和管理游戏对象的空间属性。
     * 而 GameObject 则是游戏对象的类，代表了场景中的实体，它可以具有多个组件，其中一个就是 Transform。
        因此，当你声明 FlashLight 为 public Transform 时，它实际上是指向了一个游戏对象的位置、旋转和比例等信息，而不是游戏对象本身。
     * */
    public Transform FlashLight; // 物体A的Transform
    private GameObject Road1; // 物体B
    private GameObject Road1_1;
    private GameObject Road2; // 物体B
    private GameObject Road2_1;
    private GameObject Road2_2;
    public SpriteRenderer roadRenderer;
    private bool isColliding = false;//if is colliding, road stop growing
    private float lastSafeDistance = 0f; // 记录上一次安全的（未碰撞的）距离。作用是让射线碰到障碍物后，还能缩短继续改变长度
    private int ObstacleType=0; //障碍物类型


    void Start()
    {
        //右边的路
        Road1 = new GameObject("Road1");
        InitializeRoad(Road1,1);
        BoxCollider2D Road1collider = Road1.AddComponent<BoxCollider2D>();
        Road1collider.size = new Vector2(1, 0.05f); // 初始大小，稍后会根据道路长度更新

        //左边的路
        Road2 = new GameObject("Road2");
        InitializeRoad(Road2, 2);
        BoxCollider2D Road2collider = Road2.AddComponent<BoxCollider2D>();        Road2collider.size = new Vector2(1, 0.05f);  
        
        //中间的路（不需要collider

    }

    void InitializeRoad(GameObject Road, int dir)
    {
        // 创建Road，并添加Collider和组件
        
        roadRenderer = Road.AddComponent<SpriteRenderer>();
        Sprite defaultSprite = Resources.Load<Sprite>("Square");
        roadRenderer.sprite = defaultSprite;
        roadRenderer.color = Color.red;
        //让道路渲染在障碍物的上面，不会被挡住
        roadRenderer.sortingOrder = 1; 
        //初始化路径大小
        //Road.transform.localScale = new Vector2(5.0f, 0.05f);
        

        // 初始化Road位置和旋转角度
        Road.transform.position = FlashLight.position;
        //if the road is on right/down side, adjust position to right/down
        switch (dir)
        {
            case 1:
                Road.transform.localScale = new Vector2(5.0f, 0.05f);
                Road.transform.position -= FlashLight.up * 0.5f;//Road1 is on the right/down side of the road
                break;
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
        UpdateRoad(Road1,Road1_1);
        //UpdateRoad(Road2, null); 

    }

    // Vector2 GetEndpoint(Transform roadTransform)
    // {
    //     // Length of the road is represented by the local scale x value
    //     float length = roadTransform.localScale.x;
        
    //     // Convert Transform.right to a Vector2
    //     Vector2 direction = (Vector2)roadTransform.right; // Explicit cast to Vector2
        
    //     // Convert Transform.position to a Vector2
    //     Vector2 position = (Vector2)roadTransform.position; // Explicit cast to Vector2
        
    //     // Calculate the endpoint by adding half the length to the road's position
    //     // Assuming the pivot is in the center of the road object
    //     Vector2 endpoint = position + direction * (length / 2);
        
    //     return endpoint;
    // }



    void UpdateRoad(GameObject Road, GameObject NewRoad)
    {
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

        // 如果未发生碰撞，或者！distance小于上次安全距离（即Road在缩短），则更新Road
        //也就是说，如果碰撞了（先lasteDis=手电筒到障碍物的位置，然后isColliding=1），但是在下一帧鼠标收回（distance更新，并小于LastDis）就可以继续更新Road的长度
        if (!isColliding || distance < lastSafeDistance)
        {
            //计算投影点N的坐标
            Vector2 N = flashlightPosition + FR_normalized * distance;
            Vector2 midPoint = (N + flashlightPosition) / 2f;

            //更新Road的中心点坐标
            Road.transform.position = new Vector2(midPoint.x, midPoint.y);
            Road.transform.position -= FlashLight.up * 0.5f;
            //更新Road的尺寸
            Road.transform.localScale = new Vector2(distance, 0.05f);
            // 更新上次安全距离
            lastSafeDistance = distance;
            //更新碰撞器大小
            BoxCollider2D Roadcollider = Road.GetComponent<BoxCollider2D>();
            Roadcollider.size = new Vector2(distance, 0.05f);
            //Roadcollider.offset = new Vector2(distance / 2, 0);
        }


        //检测碰撞
        RaycastHit2D hit = Physics2D.Raycast(flashlightPosition, FR_normalized, distance);

        //如果碰撞了
        if (hit.collider != null && !isColliding)
        {
            isColliding = true;
            if (hit.collider.gameObject.name.ToLower().Contains("metal"))
            //记录障碍物类型
            {
                ObstacleType = 1;
            }
            //记录碰撞点
            Vector2 hitPoint = hit.point;
            if(ObstacleType == 1)
            {
          
                //介质检测和hp的传输从这里开始
                // 生成反射光线
                ////函数还未完成，而且下面的Road1_1应该是NewRoad！！！CreateNewRoad(NewRoad,hitPoint,ObstacleType,)
                Road1_1 = Instantiate(Road);
                //设置长度为5.0f
                Road1_1.transform.localScale = new Vector2(5.0f, 0.05f);
                //position大致为撞击点
                Road1_1.transform.position = hitPoint;
                float roadRotation = Road.transform.eulerAngles.z;
                Debug.Log(Road.transform.eulerAngles.z);
                // 将 Road1_1 的旋转角度设置为 Road 的旋转角度逆时针转 120度
                Road1_1.transform.rotation = Quaternion.Euler(0, 0, roadRotation + 120f); //这个角度是相对x轴而言的角度
                Debug.Log(Road1_1.transform.eulerAngles.z);
                Road1_1.transform.position += Road1_1.transform.right * 2.0f;//得这样调，本来是2.5的，但是会有个洞
                Road1_1.transform.position += Road1_1.transform.up * 0.05f;

                 //create + position + rotate Road2_1, the bottom edge
                Road2_1 = Instantiate(Road2);
                float lengthOfRoad1 = Road1.transform.localScale.x;
                float Road2_1_length = lengthOfRoad1 - 1.5f;     
                Road2_1.transform.localScale = new Vector2(Road2_1_length, Road2.transform.localScale.y);
                //get position of tip of the flashlight
                Vector2 flashlightTip = (Vector2)FlashLight.position + (Vector2)FlashLight.right * FlashLight.localScale.x * 0.5f;
                //change position of Road2_1
                Road2_1.transform.position = flashlightTip;
                Road2_1.transform.position += Road2_1.transform.up * 0.55f;
                Road2_1.transform.right = FlashLight.right;
                Road2_1.transform.position = (Vector2)Road2_1.transform.position + (Vector2)Road2_1.transform.right * Road2_1.transform.localScale.x * 0.5f;


                //create + position + rotate Road2_2, the upper edge
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
        // 如果没有碰撞，重置isColliding
        else if (hit.collider == null)
        {
            //销毁反射光线
            Destroy(Road1_1);
            Destroy(Road2_1);
            Destroy(Road2_2);
            isColliding = false;
            Road2.SetActive(true); 
            lastSafeDistance = Mathf.Infinity;
        }
    }
}


