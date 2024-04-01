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
        // �����ײ�Ƿ�������������
        if (!collision.gameObject.CompareTag(playerTag))
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

    // ��������ײ��֮����� collider
    void DisableColliderBetweenPoints()
    {
        // �����������ײ��
        if (collisionPoint1 != Vector2.zero && collisionPoint2 != Vector2.zero)
        {
            // ��������֮����е�
            Vector2 midpoint = (collisionPoint1 + collisionPoint2) / 2f;

            // �����е�� collider
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
