using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass : MonoBehaviour
{
    // 定义需要的变量
    public string playerTag = "Player"; // 玩家标签
    private Vector2 collisionPoint1; // 第一个碰撞点的位置
    private Vector2 collisionPoint2; // 第二个碰撞点的位置

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collision with player detected.");
        }
        // 检查碰撞是否是由玩家引起的
        else
        {
            Debug.Log("Collision with non-player object detected.");

            // 获取碰撞点的位置
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (collisionPoint1 == Vector2.zero)
                {
                    collisionPoint1 = contact.point;
                }
                else if (collisionPoint2 == Vector2.zero)
                {
                    collisionPoint2 = contact.point;
                }
                else
                {
                    break;
                }
            }

            // 禁用这两个碰撞点之间的碰撞
            DisableColliderBetweenPoints();
        }
    }

    void DisableColliderBetweenPoints()
    {
        // 如果有两个碰撞点
        if (collisionPoint1 != Vector2.zero && collisionPoint2 != Vector2.zero)
        {
            // 计算两点之间的矩形区域范围
            Vector2 minPoint = Vector2.Min(collisionPoint1, collisionPoint2);
            Vector2 maxPoint = Vector2.Max(collisionPoint1, collisionPoint2);

            // 检查这个区域内是否有 collider
            Collider2D[] colliders = Physics2D.OverlapAreaAll(minPoint, maxPoint);
            foreach (Collider2D collider in colliders)
            {
                if (!collider.enabled)
                {
                    Debug.Log("Collider between points is disabled.");
                }
            }

            // 输出这两个点的位置
            Debug.Log("Collision point 1: " + collisionPoint1);
            Debug.Log("Collision point 2: " + collisionPoint2);
        }
    }

    void Start(){ }
    void Update(){}
}
