using UnityEngine;

public class DynamicRectangle : MonoBehaviour
{   /*
     * 每个游戏对象都有一个 Transform 组件，它负责保存和管理游戏对象的空间属性。
     * 而 GameObject 则是游戏对象的类，代表了场景中的实体，它可以具有多个组件，其中一个就是 Transform。
        因此，当你声明 FlashLight 为 public Transform 时，它实际上是指向了一个游戏对象的位置、旋转和比例等信息，而不是游戏对象本身。
     * */
    public Transform FlashLight; // 物体A的Transform
    private GameObject Road1; // 物体B
    private BoxCollider2D roadCollider;
    private SpriteRenderer roadRenderer;
    void Start()
    {
        Road1 = new GameObject("Road1");
        InitializeRoad1(Road1,1);

    }
    void InitializeRoad1(GameObject Road, int dir)
    {
        // 创建Road，并添加BoxCollider2D和SpriteRenderer
        roadCollider = Road.AddComponent<BoxCollider2D>();
        roadRenderer = Road.AddComponent<SpriteRenderer>();
        Sprite defaultSprite = Resources.Load<Sprite>("Square");
        roadRenderer.sprite = defaultSprite;
        roadRenderer.color = Color.red;

        // 初始化Road位置、旋转角度和尺寸
        Road.transform.position = FlashLight.position;
        if(dir==1)//if the road is on right/down side, adjust position to right/down
        {
            Road.transform.position -= FlashLight.up * 0.5f;//Road1 is on the right/down side of the road
        }
        else
        {
            Road.transform.position += FlashLight.up * 0.5f;
        }
        Road.transform.up = FlashLight.up;
        roadCollider.size = new Vector2(0, 0.05f);

        int roadLayerIndex = Road1.layer;
        string roadLayerName = LayerMask.LayerToName(roadLayerIndex);
        Debug.Log("Road layer: " + roadLayerName);
    }
    void Update()
    {
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
        //计算投影点N的坐标
        Vector2 N = flashlightPosition + FR_normalized * distance;
        Vector2 midPoint = (N + flashlightPosition) / 2f;
        //更新Road的中心点坐标
        Road1.transform.position = new Vector2(midPoint.x, midPoint.y);
        Road1.transform.position -= FlashLight.up * 0.5f;
        //更新Road的尺寸
        Road1.transform.localScale = new Vector2(distance, 0.05f);

        //更新collider尺寸
        roadCollider.size = new Vector2(distance, 0.05f);
        roadCollider.offset = new Vector2(0, 0); // 根据Road1的伸展方向调整偏移量

    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        
        Debug.Log("Collided with: " + collision.gameObject.name);
    }
}

