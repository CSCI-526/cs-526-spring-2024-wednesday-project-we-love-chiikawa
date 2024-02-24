using UnityEngine;

public class DynamicRectangle : MonoBehaviour
{
    public Transform FlashLight; // 物体A的Transform
    private GameObject Road1; // 物体B
    
    private BoxCollider2D roadCollider;
    private SpriteRenderer roadRenderer;
    void Start()
    {
        // 创建物体B
        Road1 = new GameObject("Road1");
        roadCollider = Road1.AddComponent<BoxCollider2D>();
        roadRenderer = Road1.AddComponent<SpriteRenderer>();
        Sprite defaultSprite = Resources.Load<Sprite>("Square");
        // Set loaded Sprite to the roadRenderer
        roadRenderer.sprite = defaultSprite;
        roadRenderer.color = Color.red;

        // 初始化物体B的位置和尺寸
        Road1.transform.position = FlashLight.position;
        Road1.transform.localScale = new Vector2(0, 1); // 初始长度为0，高度为1
        roadCollider.size = new Vector2(0, 1); // 初始长度为0，高度为1

    }

    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float distance = Vector2.Distance(mousePosition, FlashLight.position);

        // 更新物体B的位置
        //Road1.transform.position = (Vector2)FlashLight.position + (mousePosition - (Vector2)FlashLight.position) / 2;
        //Vector2保证两个变量都是Vector2类型
        Road1.transform.position = (Vector2)FlashLight.position + ((Vector2)mousePosition - (Vector2)FlashLight.position) / 2;
        // 更新物体B的尺寸
        Road1.transform.localScale = new Vector2(distance, 1);
        
        //Debug.Log(roadCollider.size);
        //Debug.Log(Road1.transform.localScale);
        Vector2 direction = ((Vector2)mousePosition - (Vector2)FlashLight.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Road1.transform.rotation = Quaternion.Euler(0, 0, angle);
        // 调整物体B的方向指向鼠标位置
        //Vector2 direction = (mousePosition - (Vector2)FlashLight.position).normalized;
        //Road1.transform.right = direction;
        roadCollider.size = new Vector2(distance, roadCollider.size.y);
        roadCollider.offset = new Vector2(0.5f * distance, 0); // 根据Road1的伸展方向调整偏移量
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        
        Debug.Log("Collided with: " + collision.gameObject.name);
    }
}
