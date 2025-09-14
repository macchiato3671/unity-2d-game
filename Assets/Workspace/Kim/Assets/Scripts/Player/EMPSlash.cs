using UnityEngine;

public class EMPSlash : MonoBehaviour
{
    Rigidbody2D rigid;
    int damage;
    float slowDuration;
    float slowAmount;
    float maxDistance;
    private Vector2 direction;
    private Vector3 startPosition;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.freezeRotation = true;

        // emp slash collider
        float halfWidth = 0.8f;
        float halfHeight = 0.3f;

        float angleRad = Mathf.PI / 4f;
        float cos = Mathf.Cos(angleRad);
        float sin = Mathf.Sin(angleRad);

        Vector2[] basePoints = new Vector2[]
        {
            new Vector2(-halfWidth, -halfHeight),
            new Vector2(-halfWidth,  halfHeight),
            new Vector2( halfWidth,  halfHeight),
            new Vector2( halfWidth, -halfHeight)
        };

        Vector2[] rotatedPoints = new Vector2[4];
        for (int i = 0; i < 4; i++)
        {
            float x = basePoints[i].x;
            float y = basePoints[i].y;
            rotatedPoints[i] = new Vector2(
                x * cos - y * sin,
                x * sin + y * cos
            );
        }

        PolygonCollider2D col = GetComponent<PolygonCollider2D>();
        col.pathCount = 1;
        col.SetPath(0, rotatedPoints);
    }

    public void SetStats(
        Vector2 dir,
        int final_damage,
        float final_slowDuration,
        float final_slowAmount,
        float final_maxDistance)
    {
        damage = final_damage;
        slowDuration = final_slowDuration;
        slowAmount = final_slowAmount;
        maxDistance = final_maxDistance;
        Debug.Log($"[EMPSlash.SetStats] Damage: {damage}, SlowAmount: {slowAmount}, SlowDuration: {slowDuration}, MaxDist: {maxDistance}");
        Init(dir, 30);
    }

    void Init(Vector2 dir, int speed)
    {
        direction = dir.normalized;
        startPosition = transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle += 45f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        rigid.linearVelocity = direction * speed;

        gameObject.SetActive(true);
    }

    void Update()
    {
        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            rigid.linearVelocity = Vector2.zero;
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy em = other.GetComponent<Enemy>();

            if (em != null)
            {
                em.Damaged(damage, direction);
                em.ApplySlow(slowAmount, slowDuration);

                // EMP 이펙트 생성
                GameObject effect = GameManager.inst.pool.GetRange(10);
                if (effect != null)
                {
                    effect.transform.position = other.bounds.center;

                    Vector2 enemySize = other.bounds.size;

                    SpriteRenderer spriteRenderer = effect.GetComponentInChildren<SpriteRenderer>();
                    if (spriteRenderer != null && spriteRenderer.sprite != null)
                    {
                        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

                        Vector3 scale = new Vector3(
                            enemySize.x / spriteSize.x,
                            enemySize.y / spriteSize.y,
                            1f
                        );

                        effect.transform.localScale = scale;
                    }

                    // 이펙트 최상단 정렬
                    SpriteRenderer sr = effect.GetComponentInChildren<SpriteRenderer>();
                    if (sr != null)
                    {
                        sr.sortingOrder = 1;
                    }

                    // 적에게 따라붙게 설정하고, 시간 지나면 사라지게
                    EffectAutoDestroy autoDestroy = effect.GetComponent<EffectAutoDestroy>();
                    if (autoDestroy != null)
                    {
                        autoDestroy.InitFollowAndDestroy(other.transform, slowDuration);
                    }

                    effect.SetActive(true);
                }
            }
        }
    }
}