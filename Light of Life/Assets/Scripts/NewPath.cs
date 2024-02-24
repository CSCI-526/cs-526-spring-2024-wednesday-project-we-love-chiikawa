using UnityEngine;

public class DynamicRectangle : MonoBehaviour
{   /*
     * ÿ����Ϸ������һ�� Transform ����������𱣴�͹�����Ϸ����Ŀռ����ԡ�
     * �� GameObject ������Ϸ������࣬�����˳����е�ʵ�壬�����Ծ��ж�����������һ������ Transform��
        ��ˣ��������� FlashLight Ϊ public Transform ʱ����ʵ������ָ����һ����Ϸ�����λ�á���ת�ͱ�������Ϣ����������Ϸ������
     * */
    public Transform FlashLight; // ����A��Transform
    private GameObject Road1; // ����B
    private BoxCollider2D roadCollider;
    private SpriteRenderer roadRenderer;
    void Start()
    {
        Road1 = new GameObject("Road1");
        InitializeRoad1(Road1,1);

    }
    void InitializeRoad1(GameObject Road, int dir)
    {
        // ����Road�������BoxCollider2D��SpriteRenderer
        roadCollider = Road.AddComponent<BoxCollider2D>();
        roadRenderer = Road.AddComponent<SpriteRenderer>();
        Sprite defaultSprite = Resources.Load<Sprite>("Square");
        roadRenderer.sprite = defaultSprite;
        roadRenderer.color = Color.red;

        // ��ʼ��Roadλ�á���ת�ǶȺͳߴ�
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
        //Flashlight����ķ������������㵥λ����
        Vector2 flashlightRight = new Vector2(FlashLight.right.x, FlashLight.right.y);
        Vector2 FR_normalized = flashlightRight.normalized;
        //��ȡFlashlight��Vector2����
        Vector2 flashlightPosition = FlashLight.position;
        //Flashlight��Mouse������FM
        Vector2 FM = flashlightPosition - mousePosition;
        // ����FM��Fu�ϵ�ͶӰ�Ĵ�С
        float distance = Mathf.Abs(Vector2.Dot(FM, FR_normalized));
        //����ͶӰ��N������
        Vector2 N = flashlightPosition + FR_normalized * distance;
        Vector2 midPoint = (N + flashlightPosition) / 2f;
        //����Road�����ĵ�����
        Road1.transform.position = new Vector2(midPoint.x, midPoint.y);
        Road1.transform.position -= FlashLight.up * 0.5f;
        //����Road�ĳߴ�
        Road1.transform.localScale = new Vector2(distance, 0.05f);

        //����collider�ߴ�
        roadCollider.size = new Vector2(distance, 0.05f);
        roadCollider.offset = new Vector2(0, 0); // ����Road1����չ�������ƫ����

    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        
        Debug.Log("Collided with: " + collision.gameObject.name);
    }
}

