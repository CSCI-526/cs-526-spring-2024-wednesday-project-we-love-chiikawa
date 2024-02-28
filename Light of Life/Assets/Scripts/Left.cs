using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Left : MonoBehaviour
{
    /*
* ÿ����Ϸ������һ�� Transform ����������𱣴�͹�����Ϸ����Ŀռ����ԡ�
* �� GameObject ������Ϸ������࣬�����˳����е�ʵ�壬�����Ծ��ж�����������һ������ Transform��
��ˣ��������� FlashLight Ϊ public Transform ʱ����ʵ������ָ����һ����Ϸ�����λ�á���ת�ͱ�������Ϣ����������Ϸ������
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
    private float lastSafeDistance = 0f; // ��¼��һ�ΰ�ȫ�ģ�δ��ײ�ģ����롣�����������������ϰ���󣬻������̼����ı䳤��
    private int ObstacleType = 0; //�ϰ�������
    private int CollidedRoad = 0;
    [HideInInspector]
    public bool fixedRoads = false;

    void Start()
    {
        //�ұߵ�·
        Road1 = new GameObject("Road1");
        InitializeRoad(Road1);
        BoxCollider2D Road1collider = Road1.AddComponent<BoxCollider2D>();
        Road1collider.size = new Vector2(1, 0.05f); // ��ʼ��С���Ժ����ݵ�·���ȸ���

        ////��ߵ�·
        Road2 = new GameObject("Road2");
        InitializeRoad(Road2);
        BoxCollider2D Road2collider = Road2.AddComponent<BoxCollider2D>();
        Road2collider.size = new Vector2(0.05f, 0.05f);

        //�м��·������Ҫcollider��
        Road3 = new GameObject("Road3");
        InitializeRoad(Road3);

    }


    void InitializeRoad(GameObject Road)
    {
        // ����Road�������Collider�����
        roadRenderer = Road.AddComponent<SpriteRenderer>();
        Sprite defaultSprite = Resources.Load<Sprite>("Square");
        roadRenderer.sprite = defaultSprite;
        roadRenderer.color = Color.red;
        roadRenderer.sortingOrder = 1; //�õ�·��Ⱦ���ϰ�������棬���ᱻ��ס
        //����
        Road.transform.up = FlashLight.up;
    }


    void Update()
    {
        //��F���л��Ƿ�̶���·
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
            //���ײ��������Road1��Road2�Ļ�ûд��
            bool road1Collided = ExtendRoad1(Road1, 1) == 1;

            //1ûײ��
            if (!road1Collided)
            {
                //��Ҷ��쳤
                ExtendRoad2(Road2, 2);
                ExtendRoad1(Road3, 3);
                Debug.Log("Road1ûײ��");

                //����2ײ����û
                bool road2Collided = ExtendRoad2(Road2, 2) == 2;

                //2ײ����
                if (road2Collided)
                {
                    Debug.Log("Road2ײ����");
                    //����Road2_1������������Road��λ�úͳ���
                    ReR1R3();
                }
            }

            //1ײ����
            else
            {
                Debug.Log("R1ײ����");
                //����Road1_1������������Road��λ�úͳ���
                ReR2R3();
            }
        }
    }
    void ReR2R3()
    {
        Debug.Log("ReR2R3");
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
        Road3.transform.position -= Road3.transform.right * 1.0f;

        //����Road1_1������������Road��λ�úͳ���
        if (Road1_1 != null)
        {
            Road2_1 = Instantiate(Road2);
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
    }

    void ReR1R3()
    {
        Debug.Log("ReR1R3");
        Road1.transform.localScale = new Vector2(Road2.transform.localScale.x - 1.5f, 0.05f); //��һ�س���
        Road1.transform.position = Road2.transform.position - FlashLight.up * 1.0f; //����λ��
        Road1.transform.position -= Road2.transform.right * 0.75f;
        BoxCollider2D Road1collider = Road1.GetComponent<BoxCollider2D>();
        SpriteRenderer sprite1Renderer = Road1.GetComponent<SpriteRenderer>();
        //R1 collider��size��offset
        if (sprite1Renderer != null && Road1collider != null)
        {
            Road1collider.size = sprite1Renderer.size;
            Road1collider.offset = sprite1Renderer.bounds.center - Road1.transform.position;
        }

        Road3.transform.localScale = new Vector2(Road2.transform.localScale.x - 0.5f, 0.05f); //����һ�س���
        Road3.transform.position = Road2.transform.position - FlashLight.up * 0.5f; //����λ��
        Road3.transform.position += Road3.transform.right * 0.25f;

        if (Road2_1 != null)
        {


            //����Road2_1������������Road��λ�úͳ���
            Road1_1 = Instantiate(Road1);
            float roadRotation1 = Road1.transform.eulerAngles.z; //��תR1_1
            Road1_1.transform.rotation = Quaternion.Euler(0, 0, roadRotation1 + 60f);
            Road1_1.transform.localScale = new Vector2(Road2_1.transform.localScale.x - 1.0f, 0.05f);
            Road1_1.transform.position = Road2_1.transform.position + Road2_1.transform.up * 0.75f;
            Road1_1.transform.position -= Road1_1.transform.right * 1.25f;  //
            BoxCollider2D Road1_1collider = Road1_1.GetComponent<BoxCollider2D>();
            SpriteRenderer sprite1_1Renderer = Road1_1.GetComponent<SpriteRenderer>();
            //R2_1 collider��size��offset
            if (sprite1_1Renderer != null && Road1_1collider != null)
            {
                Road1_1collider.size = sprite1_1Renderer.size;
                Road1_1collider.offset = sprite1_1Renderer.bounds.center - Road1_1.transform.position;
            }

            Road3_1 = Instantiate(Road3);
            float roadRotation3 = Road3.transform.eulerAngles.z; //��תR3_1
            Road3_1.transform.rotation = Quaternion.Euler(0, 0, roadRotation3 + 60f);
            Road3_1.transform.localScale = new Vector2(Road2_1.transform.localScale.x - 0.5f, 0.05f); //����һ�س���
            Road3_1.transform.position = Road2_1.transform.position + Road2_1.transform.up * 0.5f; //����λ��
            Road3_1.transform.position += Road3_1.transform.right * 0.25f;
        }
    }


    //�������λ���쳤road����ײ����/����

    int ExtendRoad1(GameObject Road, int RoadNo)
    {
        //�������Σ���Ȼ������߲�����ʧ�
        Destroy(Road2_1);
        Destroy(Road3_1);
        Destroy(Road1_1);
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
            if (RoadNo == 1)
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
            }
            else
            {
                Road.transform.position = new Vector2(midPoint.x, midPoint.y);
            }
            // �����ϴΰ�ȫ����
            lastSafeDistance = distance;
        }

        //�����ײ
        //RaycastHit2D hit = Physics2D.Raycast(flashlightPosition-(Vector2)FlashLight.transform.up*0.5f, FR_normalized, distance);
        //RaycastHit2D hit = Physics2D.Raycast(flashlightPosition, FR_normalized, distance);
        RaycastHit2D hit;
        Vector2 offset = (Vector2)FlashLight.transform.up * 0.4f; // ƫ����
        Vector2 adjustedPosition = flashlightPosition - offset; // Ӧ��ƫ����
        hit = Physics2D.Raycast(adjustedPosition, FR_normalized, distance);
        Debug.DrawLine(adjustedPosition, adjustedPosition + FR_normalized * distance, Color.green, duration: 20.0f);


        //�����ײ��
        if (hit.collider != null)
        {
            isColliding = true;
            Vector2 hitPoint = hit.point + (Vector2)FlashLight.transform.up * 0.4f;
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
            if (Road1_1 == null)
                Debug.Log("Road1_1����");
        }
        return 0; //����0��ʾû����ײ
    }
    int ExtendRoad2(GameObject Road, int RoadNo)
    {
        Destroy(Road2_1);
        Destroy(Road3_1);
        Destroy(Road1_1);

        //��ȡ���λ�� ������굽�ֵ�Ͳ�ľ���
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 flashlightRight = new Vector2(FlashLight.right.x, FlashLight.right.y);
        Vector2 FR_normalized = flashlightRight.normalized;
        Vector2 flashlightPosition = FlashLight.position;
        Vector2 FM = flashlightPosition - mousePosition;
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
            // �����ϴΰ�ȫ����
            lastSafeDistance = distance;
        }

        //�����ײ
        RaycastHit2D hit2;
        Vector2 offset = (Vector2)FlashLight.transform.up * 0.4f; // ƫ����
        Vector2 adjustedPosition = flashlightPosition + offset; // Ӧ��ƫ����

        hit2 = Physics2D.Raycast(adjustedPosition, FR_normalized, distance);

        Debug.DrawLine(adjustedPosition, adjustedPosition + FR_normalized * distance, Color.green, duration: 20.0f);
        //�����ײ��
        if (hit2.collider != null)
        {
            isColliding = true;
            Vector2 hit2Point = hit2.point + (Vector2)FlashLight.transform.up * 0.4f;

            if (hit2.collider.gameObject.name.ToLower().Contains("metal"))
            {
                Road2_1 = Instantiate(Road);
                //���ó���Ϊ5.0f
                Road2_1.transform.localScale = new Vector2(5.0f, 0.05f);
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
                Road2_1.transform.position = hit2Point;
                float roadRotation = Road.transform.eulerAngles.z;
                // �� Road2_1 ����ת�Ƕ�����Ϊ Road ����ת�Ƕ���ʱ��60��
                Road2_1.transform.rotation = Quaternion.Euler(0, 0, roadRotation + 60f); //����Ƕ������x����ԵĽǶ�
                Road2_1.transform.position -= Road2_1.transform.up * 0.1f;
                Road2_1.transform.position -= Road2_1.transform.right * 2.8f;//����������������2.5�ģ����ǻ��и���


            }
            Debug.Log("11111");
            return RoadNo; //������ײ��Road���
        }
        // ���û����ײ������isColliding
        else
        {
            isColliding = false;
            Destroy(Road2_1);

        }
        return 0; //����0��ʾû����ײ
    }
}
