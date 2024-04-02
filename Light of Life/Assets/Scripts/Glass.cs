using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass : MonoBehaviour
{
    // ������Ҫ�ı���
    public string playerTag = "Player"; // ��ұ�ǩ
    private Vector2 collisionPoint1; // ��һ����ײ���λ��
    private Vector2 collisionPoint2; // �ڶ�����ײ���λ��

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collision with player detected.");
        }
        // �����ײ�Ƿ�������������
        else
        {
            Debug.Log("Collision with non-player object detected.");

            // ��ȡ��ײ���λ��
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

            // ������������ײ��֮�����ײ
            DisableColliderBetweenPoints();
        }
    }

    void DisableColliderBetweenPoints()
    {
        // �����������ײ��
        if (collisionPoint1 != Vector2.zero && collisionPoint2 != Vector2.zero)
        {
            // ��������֮��ľ�������Χ
            Vector2 minPoint = Vector2.Min(collisionPoint1, collisionPoint2);
            Vector2 maxPoint = Vector2.Max(collisionPoint1, collisionPoint2);

            // �������������Ƿ��� collider
            Collider2D[] colliders = Physics2D.OverlapAreaAll(minPoint, maxPoint);
            foreach (Collider2D collider in colliders)
            {
                if (!collider.enabled)
                {
                    Debug.Log("Collider between points is disabled.");
                }
            }

            // ������������λ��
            Debug.Log("Collision point 1: " + collisionPoint1);
            Debug.Log("Collision point 2: " + collisionPoint2);
        }
    }

    void Start(){ }
    void Update(){}
}
