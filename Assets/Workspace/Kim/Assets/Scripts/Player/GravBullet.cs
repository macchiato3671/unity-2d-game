using UnityEngine;
using System.Collections;

public class GravBullet : MonoBehaviour
{
    float pullRadius; // 끌어당기는 범위
    float pullForce; // 끌어당기는 힘
    float duration; // 유지 시간
    float maxLifetime; // 플랫폼 안 닿았을 때 유지 시간
    float tickInterval; // 데미지 들어가는 틱
    float GravDamage; // GravBullet 데미지
    private float tickDamage;
    private float gizmoTimer;
    float explosionRadius;
    int explosionDamage;
    public bool isExploding = false;
    Rigidbody2D rb;
    private bool isPullingActive = false; // 플랫폼에 닿았을 때만 당기기
    public AudioClip pullAudio;
    private GameObject distortionEffect; // 공간 일그러지는 효과

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isExploding)
        {
            gizmoTimer -= Time.deltaTime;
            if (gizmoTimer <= 0f)
                isExploding = false;
        }
    }

    public void SetStats(
        Vector2 direction,
        float final_pullRadius,
        float final_pullForce,
        float final_duration,
        float final_maxLifetime,
        float final_tickInterval,
        float final_totalTickDamage
    )
    {
        pullRadius = final_pullRadius;
        pullForce = final_pullForce;
        duration = final_duration;
        maxLifetime = final_maxLifetime;
        tickInterval = final_tickInterval;
        GravDamage = final_totalTickDamage;

        tickDamage = GravDamage / (duration / tickInterval);

        Init(direction);
        StartCoroutine(PullCoroutine());
        StartCoroutine(MaxLifetimeCoroutine());
    }

    void Init(Vector2 direction)
    {
        //다시 움직일 수 있게 만듦
        rb.bodyType = RigidbodyType2D.Dynamic;

        //상태 초기화
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        //방향으로 힘 주기
        rb.AddForce(direction * 15f, ForceMode2D.Impulse);
    }

    void OnEnable()
    {
        isPullingActive = false;
    }

    IEnumerator PullCoroutine()
    {
        float timer = 0f;
        float pullTimer = 0f;
        float tickTimer = 0f;
        float sfxTimer = 0.5f;

        while (true)
        {
            if (isPullingActive)
            {
                timer += Time.deltaTime;
                pullTimer += Time.deltaTime;
                tickTimer += Time.deltaTime;
                sfxTimer += Time.deltaTime;

                if (pullTimer >= 0.01f)
                {
                    PullEnemies();
                    pullTimer = 0f;
                }

                if (tickTimer >= tickInterval)
                {
                    DamageEnemies(); // 틱 데미지 주는 함수
                    tickTimer = 0f;
                }

                if (sfxTimer >= 0.5f)
                {
                    if (pullAudio != null)
                    {
                        SoundManager.inst.PlaySFX(pullAudio, 2);
                    }
                    sfxTimer = 0f;
                }

                // 플랫폼에 닿은 후 duration 지났으면 종료
                if (timer >= duration)
                    break;
            }

            yield return null;
        }

        // 끌어당기는 이펙트 비활성화
        if (distortionEffect != null)
            distortionEffect.SetActive(false);
        
        gameObject.SetActive(false); // 풀링 시스템에서 비활성화
    }

    IEnumerator MaxLifetimeCoroutine()
    {
        yield return new WaitForSeconds(maxLifetime);

        // 아직 플랫폼에 안 닿았으면 강제 제거
        if (!isPullingActive)
        {
            Debug.Log("중력자탄 maxLifetime 초과로 강제 제거됨");
            StopAllCoroutines();
            gameObject.SetActive(false);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            // 속도 0으로
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;

            Debug.Log("중력자탄 플랫폼에 닿음");
            isPullingActive = true;

            distortionEffect = GameManager.inst.pool.GetRange(8);
            distortionEffect.transform.position = transform.position;
            
            float scaleFactor = pullRadius; // 현재 GravBullet의 pullRadius
            distortionEffect.transform.localScale = new Vector3(19.2f, 10.8f, 1f) * (scaleFactor / 2);
            distortionEffect.SetActive(true);
        }
    }
    void PullEnemies()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, pullRadius, LayerMask.GetMask("Enemy"));

        foreach (Collider2D enemy in enemies)
        {
            TrainingDummy dummy = enemy.GetComponent<TrainingDummy>();
            Enemy move = enemy.GetComponent<Enemy>();
            if (dummy != null)
            {
                Vector2 dir = (transform.position - dummy.transform.position).normalized;
                dummy.externalForce += dir * pullForce;
            }
            if (move != null)
            {
                Vector2 dir = (transform.position - move.transform.position).normalized;
                move.externalForce = dir * pullForce;
            }
        }
    }

    void DamageEnemies()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, pullRadius, LayerMask.GetMask("Enemy"));

        foreach (Collider2D enemy in enemies)
        {
            Enemy em = enemy.GetComponent<Enemy>();

            if (em != null)
            {
                em.Damaged((int)tickDamage, Vector2.zero);
                Debug.Log($"[GravTick] {enemy.name}에게 {tickDamage} 틱 데미지");
            }
        }
    }

    public void Explode(int damage, float explosRadius)
    {
        explosionRadius = explosRadius;
        explosionDamage = damage;

        Debug.Log($"중력자탄 폭발");

        isExploding = true;
        gizmoTimer = 0.3f; // 0.3초 동안만 표시

        GameObject effect = GameManager.inst.pool.GetRange(9);
        if (effect != null) {
            effect.transform.position = transform.position;

            float baseRadius = 1f;
            float scale = explosionRadius / baseRadius;
            effect.transform.localScale = new Vector3(scale, scale, 1f);

            effect.SetActive(true);
        }

        // 주변 적 검색
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius, LayerMask.GetMask("Enemy"));
        
        foreach (Collider2D enemy in hitEnemies)
        {
            Enemy em = enemy.GetComponent<Enemy>();

            if (em != null)
            {
                em.Damaged(explosionDamage,Vector2.zero);
                Debug.Log($"{enemy.name}에게 {explosionDamage} 데미지");
            }
        }

        // 끌어당기는 이펙트 비활성화
        if (distortionEffect != null)
            distortionEffect.SetActive(false);

        StopAllCoroutines();
        gameObject.SetActive(false);
    }

}
