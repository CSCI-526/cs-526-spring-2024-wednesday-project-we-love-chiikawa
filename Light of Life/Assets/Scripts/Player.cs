using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float runSpeed = 2.0f;
    public float jumpSpeed = 3.0f;
    private Rigidbody2D myRigidbody;
    private BoxCollider2D myFeet;
    private bool isGrounded;
    private bool isOnRoad;
    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myFeet = GetComponent<BoxCollider2D>(); 

    }

    // Update is called once per frame
    void Update()
    {
        Run();
        Jump();
        CheckGrounded();
    }
    void Run()
    {
        float moveDir=Input.GetAxis("Horizontal");
        Vector2 playerVel= new Vector2(moveDir*runSpeed,myRigidbody.velocity.y);
        myRigidbody.velocity=playerVel;
    }

    void CheckGrounded()
    {
        isGrounded = myFeet.IsTouchingLayers(LayerMask.GetMask("Ground"));
        isOnRoad = myFeet.IsTouchingLayers(LayerMask.GetMask("Road"));
    }
    void Jump()
    {
        
        if (Input.GetButtonDown("Jump"))// && isGrounded)
        {
            if (isGrounded||isOnRoad)
            {
                Vector2 jumpVel = new Vector2(0.0f, jumpSpeed);
                myRigidbody.velocity =Vector2.up*jumpVel;
            }
        }
    }
}
