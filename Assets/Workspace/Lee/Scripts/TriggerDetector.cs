using UnityEngine;

public class TriggerDetector : MonoBehaviour
{
    private EnemySkill enemySkill;

    void Awake()
    {
        enemySkill = GetComponentInParent<EnemySkill>();  // 부모에서 EnemySkill 참조
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerBullet"))
        {
            enemySkill.ReflectSkill(collision.gameObject);
        }
    }
}
