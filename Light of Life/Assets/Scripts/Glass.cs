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
        // 检查碰撞是否是由玩家引起的
        if (!collision.gameObject.CompareTag(playerTag))
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

    // 在两个碰撞点之间禁用 collider
    void DisableColliderBetweenPoints()
    {
        // 如果有两个碰撞点
        if (collisionPoint1 != Vector2.zero && collisionPoint2 != Vector2.zero)
        {
            // 计算两点之间的中点
            Vector2 midpoint = (collisionPoint1 + collisionPoint2) / 2f;

            // 禁用中点的 collider
            Collider2D[] colliders = Physics2D.OverlapPointAll(midpoint);
            foreach (Collider2D collider in colliders)
            {
                collider.enabled = false;
            }
        }
    }

    void Start(){ }
    void Update(){}
}
