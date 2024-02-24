using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    private const float RWidth = 1.0f;  // Width of the road
    private const float RHeight = 0.05f; 
    private const float growSpeed = 5f;
    private GameObject Road1;
    private Vector3 surfaceUp;
    private Vector3 surfaceRight;
    private Vector3 surfaceDown;
    private BoxCollider2D roadCollider;
    private SpriteRenderer roadRenderer;
    void Start()
    {
        //Surfaces of A
        surfaceUp = transform.up;      
        surfaceRight= transform.right;
        surfaceDown = -transform.up;  
        Road1 = new GameObject("Road1");
        //apply collider to the road
        roadCollider = Road1.AddComponent<BoxCollider2D>();
        roadRenderer = Road1.AddComponent<SpriteRenderer>();
        Sprite defaultSprite = Resources.Load<Sprite>("Square");
        // Set loaded Sprite to the roadRenderer
        roadRenderer.sprite = defaultSprite;
        roadRenderer.color = Color.red;
        //Create Right Road 
        CreateRoad();             
    }
    void CreateRoad()
    {
        //Adjust the size of the road
        Road1.transform.localScale = new Vector2(RWidth, RHeight);
        // ������ײ���Ĵ�С
        roadCollider.size = new Vector2(RWidth, RHeight);
        //Position of the road
        Road1.transform.position = transform.position + (surfaceDown * 0.5f);
        Road1.transform.position += (surfaceRight * RHeight * 10.0f);

        //Rotate the road
        Road1.transform.up = surfaceUp;
        //Debug.Log("Road1 Rotation: " + Road1.transform.rotation.eulerAngles);

        // �������� B �ĸ�����Ϊ���� A��ȷ�������� A һ���ƶ���
        Road1.transform.SetParent(transform);
        // ������ײ���Ĵ�С
        roadCollider.size = new Vector2(RWidth, RHeight);
        Debug.Log("surfaceUp: " + surfaceUp);
        Debug.Log("Road1.transform.up " + Road1.transform.up);
        Debug.Log("surface.right" + surfaceRight);
    }
    void ExtendRoad(GameObject Road1)
    {
        /*// ������ B �����������Ϸ������ߡ�
        RaycastHit hit;
        if (Physics.Raycast(Road1.transform.position, Road1.transform.right, out hit))
        {
            // ������߼�⵽��ײ��ֹͣ�������������� B �ĳ��ȡ�
            float distance = hit.distance;
            if (distance < RHeight)
            {
                Road1.transform.localScale = new Vector3(RWidth, distance, RWidth);
                roadCollider.size = new Vector2(RWidth, distance); // ������ײ����С
                this.enabled = false; // ֹͣ��һ���� Update ���á�
            }
        }
        else
        {
            // ���û�м�⵽��ײ�������������� B �ĳ��ȡ�
            Road1.transform.localScale += new Vector3(0, growSpeed * Time.deltaTime, 0);
            Road1.transform.position += -Road1.transform.up * (growSpeed * Time.deltaTime * 0.5f); // ����λ���Ա��ֵײ�λ�ò��䡣
        }*/
        RaycastHit2D hit = Physics2D.Raycast(Road1.transform.position, Road1.transform.right, RHeight);
        if (hit.collider != null)
        {
            // If the raycast hits something, stop growing and adjust the road's length
            float distance = hit.distance;
            if (distance < RHeight)
            {
                roadCollider.size = new Vector2(distance, RWidth); // Update collider size
                this.enabled = false; // Stop further updates
            }
        }
        else
        {
            // If no collision, continue to extend the road
            var currentSize = roadCollider.size;
            roadCollider.size = new Vector2(currentSize.x + (growSpeed * Time.deltaTime), RWidth);
            // Adjust position to keep the base in place
            Road1.transform.position += Road1.transform.right * (growSpeed * Time.deltaTime * 0.5f);
        }
    }
    void Update()
    {
        //ExtendRoad(Road1);
    }
}
