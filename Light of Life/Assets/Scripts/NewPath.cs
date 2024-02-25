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
    public SpriteRenderer roadRenderer;
    private bool isColliding = false;//if is colliding, road stop growing
    private float lastSafeDistance = 0f; // ��¼��һ�ΰ�ȫ�ģ�δ��ײ�ģ����롣�����������������ϰ���󣬻������̼����ı䳤��
    private int ObstacleType; //�ϰ�������


    void Start()
    {
        Road1 = new GameObject("Road1");
        InitializeRoad1(Road1,1);
    }

    void InitializeRoad1(GameObject Road, int dir)
    {
        // ����Road�������SpriteRenderer���
        roadRenderer = Road.AddComponent<SpriteRenderer>();
        Sprite defaultSprite = Resources.Load<Sprite>("Square");
        roadRenderer.sprite = defaultSprite;
        roadRenderer.color = Color.red;
        roadRenderer.sortingOrder = 1; //�õ�·��Ⱦ���ϰ�������棬���ᱻ��ס
        // ��ʼ��Roadλ�ú���ת�Ƕ�
        Road.transform.position = FlashLight.position;
        //if the road is on right/down side, adjust position to right/down
        if (dir==1){
            Road.transform.position -= FlashLight.up * 0.5f;//Road1 is on the right/down side of the road
        }
        else{
            Road.transform.position += FlashLight.up * 0.5f;
        }
        Road.transform.up = FlashLight.up;
    }


    void Update()
    {
        UpdateRoad(Road1,Road1_1);
        /*
        //����ұߵ�·ײ��
        if(isColliding = true)
        {
            switch(ObstacleType)
            {
               //�ϰ���Ϊ����
                case 1: 
                    CreateNewRoad(Road1, Road1_1, 1, 1);
                    break;
            }
                
        }*/
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
            lastSafeDistance = distance; // �����ϴΰ�ȫ����
        }

        
        //�����ײ
        RaycastHit2D hit = Physics2D.Raycast(flashlightPosition, FR_normalized, distance);

        //�����ײ��
        if (hit.collider != null && !isColliding)
        {
            isColliding = true;
            if (hit.collider.gameObject.name.ToLower().Contains("Metal"))
                //��¼�ϰ�������
                ObstacleType = 1;
            //��¼��ײ��
            Vector2 hitPoint = hit.point;

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

        }
        // ���û����ײ������isColliding
        else if (hit.collider == null)
        {
            //���ٷ������
            Destroy(Road1_1);
            isColliding = false;
        }
    }
    void CreateNewRoad(GameObject NRoad, Vector2 Hitpoint, int Otype, int Rtype)
    {

        
    }
}

