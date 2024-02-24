using UnityEngine;

public class DynamicRectangle : MonoBehaviour
{
    public Transform FlashLight; // ����A��Transform
    private GameObject Road1; // ����B
    
    private BoxCollider2D roadCollider;
    private SpriteRenderer roadRenderer;
    void Start()
    {
        // ��������B
        Road1 = new GameObject("Road1");
        roadCollider = Road1.AddComponent<BoxCollider2D>();
        roadRenderer = Road1.AddComponent<SpriteRenderer>();
        Sprite defaultSprite = Resources.Load<Sprite>("Square");
        // Set loaded Sprite to the roadRenderer
        roadRenderer.sprite = defaultSprite;
        roadRenderer.color = Color.red;

        // ��ʼ������B��λ�úͳߴ�
        Road1.transform.position = FlashLight.position;
        Road1.transform.localScale = new Vector2(0, 1); // ��ʼ����Ϊ0���߶�Ϊ1
        roadCollider.size = new Vector2(0, 1); // ��ʼ����Ϊ0���߶�Ϊ1

    }

    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float distance = Vector2.Distance(mousePosition, FlashLight.position);

        // ��������B��λ��
        //Road1.transform.position = (Vector2)FlashLight.position + (mousePosition - (Vector2)FlashLight.position) / 2;
        //Vector2��֤������������Vector2����
        Road1.transform.position = (Vector2)FlashLight.position + ((Vector2)mousePosition - (Vector2)FlashLight.position) / 2;
        // ��������B�ĳߴ�
        Road1.transform.localScale = new Vector2(distance, 1);
        
        //Debug.Log(roadCollider.size);
        //Debug.Log(Road1.transform.localScale);
        Vector2 direction = ((Vector2)mousePosition - (Vector2)FlashLight.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Road1.transform.rotation = Quaternion.Euler(0, 0, angle);
        // ��������B�ķ���ָ�����λ��
        //Vector2 direction = (mousePosition - (Vector2)FlashLight.position).normalized;
        //Road1.transform.right = direction;
        roadCollider.size = new Vector2(distance, roadCollider.size.y);
        roadCollider.offset = new Vector2(0.5f * distance, 0); // ����Road1����չ�������ƫ����
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        
        Debug.Log("Collided with: " + collision.gameObject.name);
    }
}
