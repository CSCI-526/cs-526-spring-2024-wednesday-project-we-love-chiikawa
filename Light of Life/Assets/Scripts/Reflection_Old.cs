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
    public Transform FlashLight; // ����A��Transform
    private GameObject Road1; // ����B
    private GameObject Road1_1;
    private GameObject Road2; // ����B
    private GameObject Road2_1;
    private GameObject Road2_2;
    public SpriteRenderer roadRenderer;
    private bool isColliding = false;//if is colliding, road stop growing
    private float lastSafeDistance = 0f; // ��¼��һ�ΰ�ȫ�ģ�δ��ײ�ģ����롣�����������������ϰ���󣬻������̼����ı䳤��
    private int ObstacleType=0; //�ϰ�������


    void Start()
    {
        //�ұߵ�·
        Road1 = new GameObject("Road1");
        InitializeRoad(Road1,1);
        BoxCollider2D Road1collider = Road1.AddComponent<BoxCollider2D>();
        Road1collider.size = new Vector2(1, 0.05f); // ��ʼ��С���Ժ����ݵ�·���ȸ���

        //��ߵ�·
        Road2 = new GameObject("Road2");
        InitializeRoad(Road2, 2);
        BoxCollider2D Road2collider = Road2.AddComponent<BoxCollider2D>();        Road2collider.size = new Vector2(1, 0.05f);  
        
        //�м��·������Ҫcollider

    }

    void InitializeRoad(GameObject Road, int dir)
    {
        // ����Road�������Collider�����
        
        roadRenderer = Road.AddComponent<SpriteRenderer>();
        Sprite defaultSprite = Resources.Load<Sprite>("Square");
        roadRenderer.sprite = defaultSprite;
        roadRenderer.color = Color.red;
        //�õ�·��Ⱦ���ϰ�������棬���ᱻ��ס
        roadRenderer.sortingOrder = 1; 
        //��ʼ��·����С
        //Road.transform.localScale = new Vector2(5.0f, 0.05f);
        

        // ��ʼ��Roadλ�ú���ת�Ƕ�
        Road.transform.position = FlashLight.position;
        //if the road is on right/down side, adjust position to right/down
        switch (dir)
        {
            case 1:
                Road.transform.localScale = new Vector2(5.0f, 0.05f);
                Road.transform.position -= FlashLight.up * 0.5f;//Road1 is on the right/down side of the road
                break;
            case 2:
                Road.transform.localScale = new Vector2(4.0f, 0.05f);
                Road.transform.position += FlashLight.up * 0.5f;
                Road.transform.position += FlashLight.right * 2.0f;
                break;

        }
            
        Road.transform.up = FlashLight.up;
    }


    void Update()
    {
        UpdateRoad(Road1,Road1_1);
        //UpdateRoad(Road2, null); 

    }

    // Vector2 GetEndpoint(Transform roadTransform)
    // {
    //     // Length of the road is represented by the local scale x value
    //     float length = roadTransform.localScale.x;
        
    //     // Convert Transform.right to a Vector2
    //     Vector2 direction = (Vector2)roadTransform.right; // Explicit cast to Vector2
        
    //     // Convert Transform.position to a Vector2
    //     Vector2 position = (Vector2)roadTransform.position; // Explicit cast to Vector2
        
    //     // Calculate the endpoint by adding half the length to the road's position
    //     // Assuming the pivot is in the center of the road object
    //     Vector2 endpoint = position + direction * (length / 2);
        
    //     return endpoint;
    // }



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

                 //create + position + rotate Road2_1, the bottom edge
                Road2_1 = Instantiate(Road2);
                float lengthOfRoad1 = Road1.transform.localScale.x;
                float Road2_1_length = lengthOfRoad1 - 1.5f;     
                Road2_1.transform.localScale = new Vector2(Road2_1_length, Road2.transform.localScale.y);
                //get position of tip of the flashlight
                Vector2 flashlightTip = (Vector2)FlashLight.position + (Vector2)FlashLight.right * FlashLight.localScale.x * 0.5f;
                //change position of Road2_1
                Road2_1.transform.position = flashlightTip;
                Road2_1.transform.position += Road2_1.transform.up * 0.55f;
                Road2_1.transform.right = FlashLight.right;
                Road2_1.transform.position = (Vector2)Road2_1.transform.position + (Vector2)Road2_1.transform.right * Road2_1.transform.localScale.x * 0.5f;


                //create + position + rotate Road2_2, the upper edge
                float lengthOfRoad1_1 = Road1_1.transform.localScale.x;
                float Road2_2_length = lengthOfRoad1_1 * 0.6f;
                Road2_2 = Instantiate(Road2);
                Road2_2.transform.localScale = new Vector2(Road2_2_length, Road1_1.transform.localScale.y);
                Road2_2.transform.rotation = Road1_1.transform.rotation * Quaternion.Euler(0, 0, 180);
                Vector2 endOfRoad2_1 = (Vector2)Road2_1.transform.position + (Vector2)Road2_1.transform.right * Road2_1.transform.localScale.x * 0.5f; 
                Vector2 startOfRoad2_2 = endOfRoad2_1 - (Vector2)Road2_2.transform.right * Road2_2.transform.localScale.x * 0.5f;
                Road2_2.transform.position = startOfRoad2_2;

                Road2.SetActive(false);

            }

            ObstacleType = 0;

        }
        // ���û����ײ������isColliding
        else if (hit.collider == null)
        {
            //���ٷ������
            Destroy(Road1_1);
            Destroy(Road2_1);
            Destroy(Road2_2);
            isColliding = false;
            Road2.SetActive(true); 
            lastSafeDistance = Mathf.Infinity;
        }
    }
}


