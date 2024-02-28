using UnityEngine;

public class FlashLightLV1 : MonoBehaviour
{
    /*
    * ÿ����Ϸ������һ�� Transform ����������𱣴�͹�����Ϸ����Ŀռ����ԡ�
    * �� GameObject ������Ϸ������࣬�����˳����е�ʵ�壬�����Ծ��ж�����������һ������ Transform��
       ��ˣ��������� FlashLight Ϊ public Transform ʱ����ʵ������ָ����һ����Ϸ�����λ�á���ת�ͱ�������Ϣ����������Ϸ��������
    * */
    public Transform FlashLight;
    private GameObject Road1;
    private GameObject Road1_1;
    private GameObject Road2;
    private GameObject Road2_1;
    private GameObject Road3;
    private GameObject Road3_1;
    public SpriteRenderer roadRenderer;
    private bool isColliding = false;//if is colliding, road stop growing
    private float lastSafeDistance = 0f; // ��¼��һ�ΰ�ȫ�ģ�δ��ײ�ģ����롣�����������������ϰ���󣬻������̼����ı䳤��
    private int ObstacleType = 0; //�ϰ�������
    private int CollidedRoad = 0;
    [HideInInspector]
    public bool fixedRoads = false;
    public GameObject player;
    public string scriptName = "FlashLightRe";
    void Start()
    {
        //�ұߵ�·
        Road1 = new GameObject("Road1");
        InitializeRoad(Road1);
        BoxCollider2D Road1collider = Road1.AddComponent<BoxCollider2D>();
        //Road1collider.size = new Vector2(1, 0.05f); // ��ʼ��С���Ժ����ݵ�·���ȸ���
        Road1.layer = LayerMask.NameToLayer("Ground");

        ////��ߵ�·
        Road2 = new GameObject("Road2");
        InitializeRoad(Road2);
        BoxCollider2D Road2collider = Road2.AddComponent<BoxCollider2D>();
        //Road2collider.size = new Vector2(0.05f, 0.05f);
        Road2.layer = LayerMask.NameToLayer("Ground");

        //�м��·������Ҫcollider��
        Road3 = new GameObject("Road3");
        InitializeRoad(Road3);


        //���collider size
        Debug.Log("1:"+Road1.transform.localScale+ Road1collider.size);
        Debug.Log("2:"+Road2.transform.localScale+ Road2collider.size);
    }


    void InitializeRoad(GameObject Road)
    {
        // ����Road��������Collider�����
        roadRenderer = Road.AddComponent<SpriteRenderer>();
        Sprite defaultSprite = Resources.Load<Sprite>("Square");
        roadRenderer.sprite = defaultSprite;
        roadRenderer.color = Color.red;
        roadRenderer.sortingOrder = 1; //�õ�·��Ⱦ���ϰ�������棬���ᱻ��ס
        //����Layer


        //����
        Road.transform.up = FlashLight.up;
    }


    void Update()
    {
        //��K���ýű�
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (Road3_1 != null) { Destroy(Road3_1); }
            if (Road1 != null) { Destroy(Road1); }
            if (Road2 != null) { Destroy(Road2); }
            if (Road3 != null) { Destroy(Road3); }
            if (Road1_1 != null) { Destroy(Road1_1); }
            if (Road2_1 != null) { Destroy(Road2_1); }

            MonoBehaviour scriptToDisable = GetComponent(scriptName) as MonoBehaviour;
            if (scriptToDisable != null)
            {
                scriptToDisable.enabled = false;
            }
            else
            {
                Debug.LogError("Cannot find script named " + scriptName + " on GameObject: " + gameObject.name);
            }
        }

        //��F���л��Ƿ�̶���·
        if (Input.GetKeyDown(KeyCode.F) && player.GetComponent<BatteryController>().batteryLevel > 0)
        {
            fixedRoads = !fixedRoads;

            if (fixedRoads == true)
            {
                player.GetComponent<BatteryController>().batteryLevel--;
                //Debug.Log(player.GetComponent<BatteryController>().batteryLevel);
            }
        }

        if (!fixedRoads)
        {
            //���ײ��������Road1��Road2�Ļ�ûд��
            bool road1Collided = ExtendRoad(Road1, Road1_1, 1) == 1;

            // Extend Road2 and Road3 only if Road1 has not collided
            // This assumes ExtendRoad returns 1 if collision occurs for the specific road
            if (!road1Collided)
            {
                ExtendRoad(Road2, Road2_1, 2);
                ExtendRoad(Road3, Road3_1, 3);
            }

            else
            {
                //����Road1������������Road��λ�úͳ���
                Road2.transform.localScale = new Vector2(Road1.transform.localScale.x - 1.5f, 0.05f); //����һ�س���
                Road2.transform.position = Road1.transform.position + FlashLight.up * 1.0f; //����λ��
                Road2.transform.position -= Road2.transform.right * 0.75f;
                BoxCollider2D Road2collider = Road2.GetComponent<BoxCollider2D>();
                SpriteRenderer sprite2Renderer = Road2.GetComponent<SpriteRenderer>();
                //R2 collider��size��offset
                if (sprite2Renderer != null && Road2collider != null)
                {
                    Road2collider.size = sprite2Renderer.size;
                    Road2collider.offset = sprite2Renderer.bounds.center - Road2.transform.position;
                }

                Road3.transform.localScale = new Vector2(Road1.transform.localScale.x - 0.5f, 0.05f); //����һ�س���
                Road3.transform.position = Road1.transform.position + FlashLight.up * 0.5f; //����λ��
                Road3.transform.position -= Road3.transform.right * 0.25f;

                //����Road1_1������������Road��λ�úͳ���
                Road2_1 = Instantiate(Road2);

                Rigidbody2D rb = Road2_1.AddComponent<Rigidbody2D>();

                // Sets the Rigidbody2D to Kinematic
                rb.isKinematic = true;

                float roadRotation2 = Road2.transform.eulerAngles.z; //��תR2_1
                Road2_1.transform.rotation = Quaternion.Euler(0, 0, roadRotation2 + 120f);
                Road2_1.transform.localScale = new Vector2(Road1_1.transform.localScale.x - 1.0f, 0.05f);
                Road2_1.transform.position = Road1_1.transform.position + Road1_1.transform.up * 1.0f;
                Road2_1.transform.position += Road2_1.transform.right * 1.0f;  //
                BoxCollider2D Road2_1collider = Road2_1.GetComponent<BoxCollider2D>();
                SpriteRenderer sprite2_1Renderer = Road2_1.GetComponent<SpriteRenderer>();
                //R2_1 collider��size��offset
                if (sprite2_1Renderer != null && Road2_1collider != null)
                {
                    Road2_1collider.size = sprite2_1Renderer.size;
                    Road2_1collider.offset = sprite2_1Renderer.bounds.center - Road2_1.transform.position;
                }

                Road3_1 = Instantiate(Road3);
                float roadRotation3 = Road3.transform.eulerAngles.z; //��תR3_1
                Road3_1.transform.rotation = Quaternion.Euler(0, 0, roadRotation3 + 120f);
                Road3_1.transform.localScale = new Vector2(Road1_1.transform.localScale.x - 0.5f, 0.05f); //����һ�س���
                Road3_1.transform.position = Road1_1.transform.position + Road1_1.transform.up * 0.5f; //����λ��
                Road3_1.transform.position += Road3_1.transform.right * 0.25f;
            }



    //�������λ���쳤road����ײ����/����
    int ExtendRoad(GameObject Road, GameObject NewRoad, int RoadNo)
    {
        Destroy(Road2_1);
        Destroy(Road3_1);
        //��ȡ���λ�� ����Road��λ�úͳ���
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

        //δ��ײ��������ƶ�
        if (!isColliding || distance < lastSafeDistance)
        {
            //����ͶӰ��N������
            Vector2 N = flashlightPosition + FR_normalized * distance;
            Vector2 midPoint = (N + flashlightPosition) / 2f;

            //����Road�ߴ�
            Road.transform.localScale = new Vector2(distance, 0.05f);

            //����Road�����Collider�ߴ�

            switch (RoadNo)
            {
                case 1:
                    {
                        Road.transform.position = new Vector2(midPoint.x, midPoint.y);
                        Road.transform.position -= FlashLight.up * 0.5f;
                        BoxCollider2D Roadcollider = Road.GetComponent<BoxCollider2D>();
                        SpriteRenderer spriteRenderer = Road.GetComponent<SpriteRenderer>();
                        //collider��size��offset
                        if (spriteRenderer != null && Roadcollider != null)
                        {
                            Roadcollider.size = spriteRenderer.size;
                            Roadcollider.offset = spriteRenderer.bounds.center - Road.transform.position;
                        }
                                break;
                    }
                case 2:
                    {
                        Road.transform.position = new Vector2(midPoint.x, midPoint.y);
                        Road.transform.position += FlashLight.up * 0.5f;
                        BoxCollider2D Roadcollider = Road.GetComponent<BoxCollider2D>();
                        SpriteRenderer spriteRenderer = Road.GetComponent<SpriteRenderer>();
                        //collider��size��offset
                        if (spriteRenderer != null && Roadcollider != null)
                        {
                            Roadcollider.size = spriteRenderer.size;
                            Roadcollider.offset = spriteRenderer.bounds.center - Road.transform.position;
                        }
                        break;
                    }
                case 3:
                    {
                        Road.transform.position = new Vector2(midPoint.x, midPoint.y);
                        break;
                    }
            }
            // �����ϴΰ�ȫ����
            lastSafeDistance = distance;
        }

        //�����ײ
        RaycastHit2D hit = Physics2D.Raycast(flashlightPosition, FR_normalized, distance);

        //�����ײ��
        if (hit.collider != null)
        {
            isColliding = true;
            Vector2 hitPoint = hit.point;
            //�����ײ����metal��ת120��
            //������Ը��ݽ����޸ĽǶ�
            if (hit.collider.gameObject.name.ToLower().Contains("metal"))
            {
                Road1_1 = Instantiate(Road);
                //���ó���Ϊ5.0f
                Road1_1.transform.localScale = new Vector2(5.0f, 0.05f);
                BoxCollider2D Roadcollider = Road.GetComponent<BoxCollider2D>();
                Roadcollider.size = new Vector2(5.0f, 0.05f);
                SpriteRenderer spriteRenderer = Road.GetComponent<SpriteRenderer>();
                //collider��size��offset
                if (spriteRenderer != null && Roadcollider != null)
                {
                    Roadcollider.size = spriteRenderer.size;
                    Roadcollider.offset = spriteRenderer.bounds.center - Road.transform.position;
                }
                //position����Ϊײ����
                Road1_1.transform.position = hitPoint;
                float roadRotation = Road.transform.eulerAngles.z;
                // �� Road1_1 ����ת�Ƕ�����Ϊ Road ����ת�Ƕ���ʱ��ת 120��
                Road1_1.transform.rotation = Quaternion.Euler(0, 0, roadRotation + 120f); //����Ƕ������x����ԵĽǶ�
                Road1_1.transform.position += Road1_1.transform.right * 2.0f;//����������������2.5�ģ����ǻ��и���
                Road1_1.transform.position += Road1_1.transform.up * 0.05f;
                        
            }

            return RoadNo; //������ײ��Road���
        }
        // ���û����ײ������isColliding
        else
        {
            isColliding = false;
            Destroy(Road1_1);
        }
        return 0; //����0��ʾû����ײ
    }
        }
    }
}