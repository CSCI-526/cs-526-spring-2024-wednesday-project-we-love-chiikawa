using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;

public class DynamicRectangle : MonoBehaviour
{   /*
     * ÿ����Ϸ������һ�� Transform ����������𱣴�͹�����Ϸ����Ŀռ����ԡ�
     * �� GameObject ������Ϸ������࣬�����˳����е�ʵ�壬�����Ծ��ж�����������һ������ Transform��
        ��ˣ��������� FlashLight Ϊ public Transform ʱ����ʵ������ָ����һ����Ϸ�����λ�á���ת�ͱ�������Ϣ����������Ϸ������
     * */
    public Transform FlashLight; 
    private GameObject Road1; // Right Road
    private GameObject Road1_1;
    private GameObject Road2; // Left Road
    private GameObject Road2_1;
    public SpriteRenderer roadRenderer;
    private bool isColliding = false;//if is colliding, road stop growing
    private float lastSafeDistance = 0f; // ��¼��һ�ΰ�ȫ�ģ�δ��ײ�ģ����롣�����������������ϰ���󣬻������̼����ı䳤��
    private int ObstacleType=0; //�ϰ�������
    bool fixedRoads = false; //If press 'F', fix roads

    void Start()
    {
        //�ұߵ�·
        Road1 = new GameObject("Road1");
        InitializeRoad(Road1,1);
        BoxCollider2D Road1collider = Road1.AddComponent<BoxCollider2D>();
        Road1collider.size = new Vector2(1, 0.05f); // ��ʼ��С���Ժ����ݵ�·���ȸ���
        
        //��ߵ�·
        /*
        Road2 = new GameObject("Road2");
        InitializeRoad(Road2, 2);
        BoxCollider2D Road2collider = Road1.AddComponent<BoxCollider2D>();
        Road2collider.size = new Vector2(1, 0.05f); 
        */
        //�м��·������Ҫcollider��

    }

    void InitializeRoad(GameObject Road, int side)
    {
        // ����Road�������Collider�����
        
        roadRenderer = Road.AddComponent<SpriteRenderer>();
        Sprite defaultSprite = Resources.Load<Sprite>("Square");
        roadRenderer.sprite = defaultSprite;
        roadRenderer.color = Color.red;
        //�õ�·��Ⱦ���ϰ�������棬���ᱻ��ס
        roadRenderer.sortingOrder = 1; 
        //��ʼ��·����С
        Road.transform.localScale = new Vector2(5.0f, 0.05f);
        

        // ��ʼ��Roadλ�ú���ת�Ƕ�
        Road.transform.position = FlashLight.position;
        //if the road is on right/down side, adjust position to right/down
        switch (side)
        {
            case 1:
                Road.transform.position -= FlashLight.up * 0.5f;//Road1 is on the right/down side of the road
                break;
            case 2:
                Road.transform.position += FlashLight.up * 0.5f;
                break;
        }
            
        Road.transform.up = FlashLight.up;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            fixedRoads = !fixedRoads; // ����F���л�Road��Road1_1�Ĺ̶�״̬
        }

        if (!fixedRoads) // ֻ�е�Roadsδ���̶�ʱ���Ÿ������ǵ�λ�úʹ�С
        {
            UpdateRoad(Road1, Road1_1);
            
        }

    }

    void UpdateRoad(GameObject Road, GameObject NewRoad)
    {
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

        // ���δ������ײ�����ߣ�distanceС���ϴΰ�ȫ���루��Road�����̣��������Road
        //Ҳ����˵�������ײ�ˣ���lasteDis=�ֵ�Ͳ���ϰ����λ�ã�Ȼ��isColliding=1������������һ֡����ջأ�distance���£���С��LastDis���Ϳ��Լ�������Road�ĳ���
        if (!isColliding || distance < lastSafeDistance)
        {
            //����ͶӰ��N������
            Vector2 N = flashlightPosition + FR_normalized * distance;
            Vector2 midPoint = (N + flashlightPosition) / 2f;

            //����Road�����ĵ�����
            Road.transform.position = new Vector2(midPoint.x, midPoint.y);
            Road.transform.position -= FlashLight.up * 0.5f;
            //����Road�ĳߴ�
            Road.transform.localScale = new Vector2(distance, 0.05f);
            // �����ϴΰ�ȫ����
            lastSafeDistance = distance;
            //������ײ����С
            BoxCollider2D Roadcollider = Road.GetComponent<BoxCollider2D>();
            Roadcollider.size = new Vector2(distance, 0.05f);
            //Roadcollider.offset = new Vector2(distance / 2, 0);
        }


        //�����ײ
        RaycastHit2D hit = Physics2D.Raycast(flashlightPosition, FR_normalized, distance);

        //�����ײ��
        if (hit.collider != null && !isColliding)
        {
            isColliding = true;
            if (hit.collider.gameObject.name.ToLower().Contains("metal"))
            //��¼�ϰ�������
            {
                ObstacleType = 1;
            }
            //��¼��ײ��
            Vector2 hitPoint = hit.point;
            
            if(ObstacleType == 1)
            {
          
                //���ʼ���hp�Ĵ�������￪ʼ
                // ���ɷ������
                ////������δ��ɣ����������Road1_1Ӧ����NewRoad������CreateNewRoad(NewRoad,hitPoint,ObstacleType,)
                Road1_1 = Instantiate(Road);
                //���ó���Ϊ5.0f
                Road1_1.transform.localScale = new Vector2(5.0f, 0.05f);
                //position����Ϊײ����
                Road1_1.transform.position = hitPoint;
                float roadRotation = Road.transform.eulerAngles.z;
                Debug.Log(Road.transform.eulerAngles.z);
                // �� Road1_1 ����ת�Ƕ�����Ϊ Road ����ת�Ƕ���ʱ��ת 120��
                Road1_1.transform.rotation = Quaternion.Euler(0, 0, roadRotation + 120f); //����Ƕ������x����ԵĽǶ�
                Debug.Log(Road1_1.transform.eulerAngles.z);
                Road1_1.transform.position += Road1_1.transform.right * 2.0f;//����������������2.5�ģ����ǻ��и���
                Road1_1.transform.position += Road1_1.transform.up * 0.05f;

                Test(Road1, hitPoint, 1);
                var result = Test(gameObject, new Vector2(1f, 2f), 3);

                // �������ֵ�ĸ�����ɲ���
                Debug.Log("NRoad: " + result.Item1);
                Debug.Log("Hitpoint: " + result.Item2);
                Debug.Log("Otype: " + result.Item3);
            }
            ObstacleType = 0;

        }
        // ���û����ײ������isColliding
        else if (hit.collider == null)
        {
            //���ٷ������
            Destroy(Road1_1);
            isColliding = false;
        }
    }

    (GameObject,  Vector2,int) Test(GameObject NRoad, Vector2 Hitpoint, int Otype)
    {
        return (NRoad, Hitpoint, Otype);
    }
    void CreateNewRoad(GameObject NRoad, Vector2 Hitpoint, int Otype, int Rtype)
    {

        
    }
}

