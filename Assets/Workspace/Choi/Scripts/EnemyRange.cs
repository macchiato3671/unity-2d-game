using System.Collections;
using UnityEngine;
public class EnemyBulletRange : MonoBehaviour
{
    Rigidbody2D rigid;
    Vector2 dir;
    int speed;
    int bulletDamage;
    float maxRange;
    Vector2 startPos;

    private TrailRenderer trail;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();
        rigid.freezeRotation = true;
    }

    void FixedUpdate()
    {
        float traveled = Vector2.Distance(transform.position, startPos);
        if (traveled >= maxRange)
        {
            gameObject.SetActive(false);
            enabled = false;
        }
    }

    public void Init(Vector2 inDir, int inSpeed, int inDamage, float inMaxRange)
    {
        dir = inDir.normalized;
        speed = inSpeed;
        bulletDamage = inDamage;
        maxRange = inMaxRange;
        startPos = transform.position;

        rigid.linearVelocity = dir * speed;

        if (trail != null)
        {
            trail.Clear();
            trail.emitting = true;
        }

        enabled = true;
        gameObject.layer = LayerMask.NameToLayer("EnemyBullet");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("플레이어 공격 성공!");

            PlayerMove player = collision.GetComponent<PlayerMove>();
            if (player != null)
            {
                player.OnDamaged(transform.position);
            }

            gameObject.SetActive(false);
        }
    }
}

