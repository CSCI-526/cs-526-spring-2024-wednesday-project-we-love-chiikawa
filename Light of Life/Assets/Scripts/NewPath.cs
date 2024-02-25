using UnityEngine;

public class DynamicRectangle : MonoBehaviour
{   /*
     * 每个游戏对象都有一个 Transform 组件，它负责保存和管理游戏对象的空间属性。
     * 而 GameObject 则是游戏对象的类，代表了场景中的实体，它可以具有多个组件，其中一个就是 Transform。
        因此，当你声明 FlashLight 为 public Transform 时，它实际上是指向了一个游戏对象的位置、旋转和比例等信息，而不是游戏对象本身。
     * */
    public Transform FlashLight; // 物体A的Transform
    private GameObject Road1; // 物体B
    private SpriteRenderer roadRenderer;
    private bool isColliding = false;//if is colliding, road stop growing
    private float lastSafeDistance = 0f; // 记录上一次安全的（未碰撞的）距离。作用是让射线碰到障碍物后，还能缩短继续改变长度

    void Start()
    {
        Road1 = new GameObject("Road1");
        InitializeRoad1(Road1,1);
    }
    void InitializeRoad1(GameObject Road, int dir)
    {
        // 创建Road，并添加SpriteRenderer
        roadRenderer = Road.AddComponent<SpriteRenderer>();
        Sprite defaultSprite = Resources.Load<Sprite>("Square");
        roadRenderer.sprite = defaultSprite;
        roadRenderer.color = Color.red;

        // 初始化Road位置和旋转角度
        Road.transform.position = FlashLight.position;
        //if the road is on right/down side, adjust position to right/down
        if (dir==1){
            Road.transform.position -= FlashLight.up * 0.5f;//Road1 is on the right/down side of the road
        }
        else{
            Road.transform.position += FlashLight.up * 0.5f;
        }
        Road.transform.up = FlashLight.up;

    }
    void Update()
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
            Road1.transform.position = new Vector2(midPoint.x, midPoint.y);
            Road1.transform.position -= FlashLight.up * 0.5f;
            //更新Road的尺寸
            Road1.transform.localScale = new Vector2(distance, 0.05f);
            lastSafeDistance = distance; // 更新上次安全距离
        }

        //检测碰撞
        RaycastHit2D hit = Physics2D.Raycast(flashlightPosition, FR_normalized, distance);
        if (hit.collider != null && !isColliding)
            {
              isColliding = true;
              //即使变量变成true了，只要鼠标收回到碰不到的位置，update函数就可以继续改变长度
              Debug.Log("Raycast hit: " + hit.collider.gameObject.name);
            }
        else if(hit.collider == null)
            isColliding = false; // 如果没有碰撞，重置isColliding
    }
    
}

