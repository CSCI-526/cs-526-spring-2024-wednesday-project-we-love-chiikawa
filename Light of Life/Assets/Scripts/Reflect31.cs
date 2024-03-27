
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Reflect31 : MonoBehaviour
{
    public Transform FlashLight;
    private GameObject Road1;
    private GameObject Road1_1;
    private GameObject Road2;
    private GameObject Road2_1;
    public SpriteRenderer roadRenderer;
    private bool isColliding = false;//if is colliding, road stop growing
    private bool Colliding1 = false;
    private bool Colliding2 = false;
    private float lastSafeDistance = 0f; // ��¼��һ�ΰ�ȫ�ģ�δ��ײ�ģ����롣�����������������ϰ���󣬻������̼����ı䳤��
    public bool fixedRoads = false;
    public GameObject player;
    public bool isCollidingWithPlayer;
    public float maxChangePerFrame = 0.5f; // ÿ֡��󳤶Ȼ�λ�ñ仯
    private Vector2 lastPosition; // ��һ֡��λ��

    void Start()
    {
        Road1 = new GameObject("Road1");
        InitializeRoad(Road1);
        //BoxCollider2D Road1collider = Road1.AddComponent<BoxCollider2D>();
        Road1.layer = LayerMask.NameToLayer("Ground");

        Road2 = new GameObject("Road2");
        InitializeRoad(Road2);
        //BoxCollider2D Road2collider = Road2.AddComponent<BoxCollider2D>();
        Road2.layer = LayerMask.NameToLayer("Ground");

        fixedRoads = false;
    }
    void InitializeRoad(GameObject Road)
    {
        // ����Road����������
        Road.transform.localScale = new Vector2(0, 0);
        roadRenderer = Road.AddComponent<SpriteRenderer>();
        Sprite defaultSprite = Resources.Load<Sprite>("Square");
        roadRenderer.sprite = defaultSprite;
        roadRenderer.color = Color.yellow;
        roadRenderer.sortingOrder = 1; //�õ�·��Ⱦ���ϰ�������棬���ᱻ��ס

        //����

        Road.transform.up = FlashLight.up;

        Road.transform.position = FlashLight.position;

        BoxCollider2D roadCollider = Road.AddComponent<BoxCollider2D>();
        roadCollider.size = new Vector2(1, 0.05f);
        roadCollider.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isCollidingWithPlayer = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isCollidingWithPlayer = false;
        }
    }

    void Update()
    {
        //���playerײ��Flashlight
        if (isCollidingWithPlayer)
        {
            if (Input.GetKeyDown(KeyCode.F) && player.GetComponent<BatteryController>().batteryLevel > 0)
            {
                fixedRoads = !fixedRoads;
                ToggleColliders(fixedRoads); 

                if (fixedRoads == true)
                {
                    player.GetComponent<BatteryController>().batteryLevel--;
                    PlayerPrefs.SetInt("EnergyUsedCount", PlayerPrefs.GetInt("EnergyUsedCount", 0) + 1);
                    //Debug.Log(PlayerPrefs.GetInt("EnergyUsedCount", 0));
                }
            }
            if (!fixedRoads)
            {
                ExpandR1(Road1);
                ExpandR2(Road2);
            }
        }
        
    }
    void ToggleColliders(bool state)
    {
        // This method toggles the colliders for all roads
        Road1.GetComponent<BoxCollider2D>().enabled = state;
        Road2.GetComponent<BoxCollider2D>().enabled = state;

        if (Road1_1) Road1_1.GetComponent<BoxCollider2D>().enabled = state;
        if (Road2_1) Road2_1.GetComponent<BoxCollider2D>().enabled = state;
    }

    void ExpandR1(GameObject Road)
    {
        Destroy(Road1_1);
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastPosition = Vector2.Lerp(lastPosition, mousePosition, Time.deltaTime * 1.0f);//����ƶ�̫����ⲻ����ײ��������Ҫ����һ��
        Vector2 flashlightRight = new Vector2(FlashLight.right.x, FlashLight.right.y);
        Vector2 FR_normalized = flashlightRight.normalized;
        Vector2 flashlightPosition = FlashLight.position;
        Vector2 FM = flashlightPosition - lastPosition;//
        // ����FM��Fu�ϵ�ͶӰ�Ĵ�С
        //�о�distance�ڵ�һ֡�������⣬ʵ�ڲ�֪��Ϊɶ������ֱ��-4�ˣ��ֱ��Ľ����
        float distance = Mathf.Abs(Vector2.Dot(FM, FR_normalized)) - 4.0f;
        if (distance < 0) { distance = 0.0f; }
        //����ͶӰ��N������
        Vector2 N = flashlightPosition + FR_normalized * distance;
        Vector2 midPoint = (N + flashlightPosition) / 2f;


        RaycastHit2D hit1;
        Vector2 offset = (Vector2)FlashLight.transform.up * 0.4f; // ƫ����
        Vector2 adjustedPosition = flashlightPosition - offset; // Ӧ��ƫ����
        hit1 = Physics2D.Raycast(adjustedPosition, FR_normalized, distance);
        Debug.DrawLine(adjustedPosition, adjustedPosition + FR_normalized * distance, Color.green, duration: 20.0f);

        //Debug.Log("dist" + distance);

        //���R2��ײ���ˣ�����R1��R1_1
        if (Colliding2 == true)
        {
            Road.transform.localScale = new Vector2(Road2.transform.localScale.x - 1.5f, 0.05f);
            Road.transform.position = new Vector2(Road2.transform.position.x, Road2.transform.position.y);
            Road.transform.position -= Road.transform.up * 0.5f;
            Road.transform.position -= Road.transform.right * 0.75f;
            Road.transform.position -= FlashLight.up * 0.5f;
            BoxCollider2D Road1collider = Road1.GetComponent<BoxCollider2D>();
            SpriteRenderer sprite1Renderer = Road1.GetComponent<SpriteRenderer>();
            if (sprite1Renderer != null && Road1collider != null)
            {
                Road1collider.size = sprite1Renderer.size;
                Road1collider.offset = sprite1Renderer.bounds.center - Road1.transform.position;
            }

            Vector2 roadDirection = new Vector2(Mathf.Cos(Road.transform.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(Road.transform.eulerAngles.z * Mathf.Deg2Rad));
            Vector2 roadEndPoint = (Vector2)Road.transform.position + roadDirection * (Road.transform.localScale.x * 0.5f);
            Road1_1 = Instantiate(Road);
            Road1_1.transform.localScale = new Vector2(5.0f, 0.05f);
            Road1_1.transform.position = roadEndPoint;
            Road1_1.transform.rotation = Quaternion.Euler(0, 0, Road.transform.eulerAngles.z - 120f);
            Road1_1.transform.position += Road1_1.transform.right * 2.5f;
            BoxCollider2D Road1_1collider = Road1_1.GetComponent<BoxCollider2D>();
            SpriteRenderer sprite1_1Renderer = Road1_1.GetComponent<SpriteRenderer>();
            if (sprite1_1Renderer != null && Road1_1collider != null)
            {
                Road1_1collider.size = sprite1_1Renderer.size;
                Road1_1collider.offset = sprite1_1Renderer.bounds.center - Road1_1.transform.position;
            }
            return;
        }

        //R1��ײ����
        else if (hit1.collider != null)
        {
            isColliding = true;
            Colliding1 = true;
            Destroy(Road1_1);

            Vector2 road1Position = Road.transform.position;
            Vector2 road1Scale = Road.transform.localScale;
            float road1Rotation = Road.transform.eulerAngles.z;
            Vector2 roadDirection = new Vector2(Mathf.Cos(road1Rotation * Mathf.Deg2Rad), Mathf.Sin(road1Rotation * Mathf.Deg2Rad));

            // Since Road1 is centered on its pivot, you need to move half its length to get to the tip, assuming it's horizontal
            Vector2 roadEndPoint = road1Position + roadDirection * (road1Scale.x * 0.5f);

            // Instantiate Road1_1 and set its properties
            Road1_1 = Instantiate(Road);
            Road1_1.transform.localScale = new Vector2(5.0f, 0.05f);
            Road1_1.transform.position = roadEndPoint; // Initially set position to Road1's endpoint
            float roadRotation = Road.transform.eulerAngles.z;
            // �� Road1_1 ����ת�Ƕ�����Ϊ Road ����ת�Ƕ���ʱ��ת 120��
            Road1_1.transform.rotation = Quaternion.Euler(0, 0, roadRotation + 120f); //����Ƕ������x����ԵĽǶ�
            Road1_1.transform.position += Road1_1.transform.right * 2.5f;

            //Debug.Log("R1 R1_1" + Road.transform.localScale);
            //Debug.Log("R_1" + Road1_1.transform.localScale);
        }

        //R1��R2��δ��ײ
        else
        {
            //����Road�ߴ�
            Road.transform.localScale = new Vector2(distance, 0.05f);
            //����Road�����Collider�ߴ�
            Road.transform.position = new Vector2(midPoint.x, midPoint.y);
            Road.transform.position -= FlashLight.up * 0.5f;
            BoxCollider2D Road1collider = Road.GetComponent<BoxCollider2D>();
            SpriteRenderer spriteRenderer = Road.GetComponent<SpriteRenderer>();
            //collider��size��offset
            if (spriteRenderer != null && Road1collider != null)
            {
                Road1collider.size = spriteRenderer.size;
                Road1collider.offset = spriteRenderer.bounds.center - Road.transform.position;
            }
            lastSafeDistance = distance;

            isColliding = false;
            Colliding1 = false;
            lastSafeDistance = 0f;
            Destroy(Road1_1);
        }

    }
       


    void ExpandR2(GameObject Road)
    {
        Destroy(Road2_1);
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastPosition = Vector2.Lerp(lastPosition, mousePosition, Time.deltaTime * 1.0f);//����ƶ�̫����ⲻ����ײ��������Ҫ����һ��
        Vector2 flashlightRight = new Vector2(FlashLight.right.x, FlashLight.right.y);
        Vector2 FR_normalized = flashlightRight.normalized;
        Vector2 flashlightPosition = FlashLight.position;
        Vector2 FM = flashlightPosition - lastPosition;//
        // ����FM��Fu�ϵ�ͶӰ�Ĵ�С
        //�о�distance�ڵ�һ֡�������⣬ʵ�ڲ�֪��Ϊɶ������ֱ��-4�ˣ��ֱ��Ľ����
        float distance = Mathf.Abs(Vector2.Dot(FM, FR_normalized))-4.0f;
        if (distance < 0) { distance = 0.0f; }
        //����ͶӰ��N������
        Vector2 N = flashlightPosition + FR_normalized * distance;
        Vector2 midPoint = (N + flashlightPosition) / 2f;

        //Debug.Log("distance"+distance);
        RaycastHit2D hit2;
        Vector2 offset = -(Vector2)FlashLight.transform.up * 0.4f; // ƫ����
        Vector2 adjustedPosition = flashlightPosition - offset; // Ӧ��ƫ����
        hit2 = Physics2D.Raycast(adjustedPosition, FR_normalized, distance);
        Debug.DrawLine(adjustedPosition, adjustedPosition + FR_normalized * distance, Color.green, duration: 20.0f);

            //R1��ײ���ˣ�����R2��R2_1
        if (Colliding1 == true)
        {
            Road.transform.localScale = new Vector2(Road1.transform.localScale.x - 1.5f, 0.05f);
            Road.transform.position = new Vector2(Road1.transform.position.x, Road1.transform.position.y);
            Road.transform.position += Road.transform.up * 0.5f;
            Road.transform.position -= Road.transform.right * 0.75f;
            Road.transform.position += FlashLight.up * 0.5f;
            BoxCollider2D Road2collider = Road2.GetComponent<BoxCollider2D>();
            SpriteRenderer sprite2Renderer = Road2.GetComponent<SpriteRenderer>();
            if (sprite2Renderer != null && Road2collider != null)
            {
                Road2collider.size = sprite2Renderer.size;
                Road2collider.offset = sprite2Renderer.bounds.center - Road2.transform.position;
            }

            Vector2 roadDirection = new Vector2(Mathf.Cos(Road.transform.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(Road.transform.eulerAngles.z * Mathf.Deg2Rad));
            Vector2 roadEndPoint = (Vector2)Road.transform.position + roadDirection * (Road.transform.localScale.x * 0.5f);
            Road2_1 = Instantiate(Road);
            Road2_1.transform.localScale = new Vector2(5.0f, 0.05f);
            Road2_1.transform.position = roadEndPoint; 
            Road2_1.transform.rotation = Quaternion.Euler(0, 0, Road.transform.eulerAngles.z + 120f); //����Ƕ������x����ԵĽǶ�
            Road2_1.transform.position += Road2_1.transform.right * 2.5f;
            BoxCollider2D Road2_1collider = Road2_1.GetComponent<BoxCollider2D>();
            SpriteRenderer sprite2_1Renderer = Road2_1.GetComponent<SpriteRenderer>();
            if (sprite2_1Renderer != null && Road2_1collider != null)
            {
                Road2_1collider.size = sprite2_1Renderer.size;
                Road2_1collider.offset = sprite2_1Renderer.bounds.center - Road2_1.transform.position;
            }
            return;
        }

        //R2��ײ�ˣ�R1ûײ
        else if (hit2.collider != null)
        {
            isColliding = true;
            Colliding2 = true;
            Destroy(Road2_1);
            //Debug.Log("R2ײ��");
            Vector2 road1Position = Road.transform.position;
            Vector2 road1Scale = Road.transform.localScale;
            float road1Rotation = Road.transform.eulerAngles.z;
            // Calculate the direction in which Road1 is pointing based on its rotation
            Vector2 roadDirection = new Vector2(Mathf.Cos(road1Rotation * Mathf.Deg2Rad), Mathf.Sin(road1Rotation * Mathf.Deg2Rad));

            // Since Road1 is centered on its pivot, you need to move half its length to get to the tip, assuming it's horizontal
            Vector2 roadEndPoint = road1Position + roadDirection * (road1Scale.x * 0.5f);

            // Instantiate Road1_1 and set its properties
            Road2_1 = Instantiate(Road);
            Road2_1.transform.localScale = new Vector2(5.0f, 0.05f);
            Road2_1.transform.position = roadEndPoint; // Initially set position to Road1's endpoint
            float roadRotation = Road.transform.eulerAngles.z;
            // �� Road2_1 ����ת�Ƕ�����Ϊ Road ����ת�Ƕ�˳ʱ��ת 120��
            Road2_1.transform.rotation = Quaternion.Euler(0, 0, roadRotation - 120f); //����Ƕ������x����ԵĽǶ�
            Road2_1.transform.position += Road2_1.transform.right * 2.5f;

            Debug.Log(Road.transform.position);
            Debug.Log(Road.transform.localScale);
            Debug.Log(Road2_1.transform.position);
            Debug.Log(Road2_1.transform.localScale);

        }

            //R2δ��ײ,R1Ҳδ��ײ
            else 
        {
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
            lastSafeDistance = distance;
            isColliding = false;
            Colliding2 = false;
            lastSafeDistance = 0f;
            Destroy(Road2_1);
            }
    }
}
