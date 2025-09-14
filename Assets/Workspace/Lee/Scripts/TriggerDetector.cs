using UnityEngine;

public class TriggerDetector : MonoBehaviour
{
    private EnemySkill enemySkill;

    void Awake()
    {
        enemySkill = GetComponentInParent<EnemySkill>();  // �θ𿡼� EnemySkill ����
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBullet"))
        {
            enemySkill.ReflectSkill(collision.gameObject);
        }
    }
}
