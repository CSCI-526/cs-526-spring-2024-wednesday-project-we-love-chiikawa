using UnityEngine;

public class DynamicRectangle : MonoBehaviour
{   /*
     * ÿ����Ϸ������һ�� Transform ����������𱣴�͹�����Ϸ����Ŀռ����ԡ�
     * �� GameObject ������Ϸ������࣬�����˳����е�ʵ�壬�����Ծ��ж�����������һ������ Transform��
        ��ˣ��������� FlashLight Ϊ public Transform ʱ����ʵ������ָ����һ����Ϸ�����λ�á���ת�ͱ�������Ϣ����������Ϸ������
     * */
    public Transform FlashLight; // ����A��Transform
    private GameObject Road1; // ����B
    private SpriteRenderer roadRenderer;
    private bool isColliding = false;//if is colliding, road stop growing
    private float lastSafeDistance = 0f; // ��¼��һ�ΰ�ȫ�ģ�δ��ײ�ģ����롣�����������������ϰ���󣬻������̼����ı䳤��

    void Start()
    {
        Road1 = new GameObject("Road1");
        InitializeRoad1(Road1,1);
    }
    void InitializeRoad1(GameObject Road, int dir)
    {
        // ����Road�������SpriteRenderer
        roadRenderer = Road.AddComponent<SpriteRenderer>();
        Sprite defaultSprite = Resources.Load<Sprite>("Square");
        roadRenderer.sprite = defaultSprite;
        roadRenderer.color = Color.red;

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
            Road1.transform.position = new Vector2(midPoint.x, midPoint.y);
            Road1.transform.position -= FlashLight.up * 0.5f;
            //����Road�ĳߴ�
            Road1.transform.localScale = new Vector2(distance, 0.05f);
            lastSafeDistance = distance; // �����ϴΰ�ȫ����
        }

        //�����ײ
        RaycastHit2D hit = Physics2D.Raycast(flashlightPosition, FR_normalized, distance);
        if (hit.collider != null && !isColliding)
            {
              isColliding = true;
              //��ʹ�������true�ˣ�ֻҪ����ջص���������λ�ã�update�����Ϳ��Լ����ı䳤��
              Debug.Log("Raycast hit: " + hit.collider.gameObject.name);
            }
        else if(hit.collider == null)
            isColliding = false; // ���û����ײ������isColliding
    }
    
}

