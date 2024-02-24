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
        //Road scale
        Road1.transform.localScale = new Vector2(RWidth, RHeight);
        // 设置碰撞器的大小
        roadCollider.size = new Vector2(RWidth, RHeight);
        //Position of the road
        Road1.transform.position = transform.position + (surfaceDown * 0.5f);
        Road1.transform.position += (surfaceRight * RHeight * 10.0f);

        //Rotate the road
        Road1.transform.up = surfaceUp;
        //Debug.Log("Road1 Rotation: " + Road1.transform.rotation.eulerAngles);

        // 设置物体 B 的父对象为物体 A，确保与物体 A 一起移动。
        Road1.transform.SetParent(transform);
        // 设置碰撞器的大小
        roadCollider.size = new Vector2(RWidth, RHeight);
        Debug.Log("surfaceUp: " + surfaceUp);
        Debug.Log("Road1.transform.up " + Road1.transform.up);
        Debug.Log("surface.right" + surfaceRight);
    }
    void ExtendRoad(GameObject Road1)
    {
        
    }
    void Update()
    {
        ExtendRoad(Road1);
    }
}
