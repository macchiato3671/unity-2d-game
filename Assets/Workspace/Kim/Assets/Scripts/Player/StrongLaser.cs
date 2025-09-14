using UnityEngine;

public class StrongLaser : MonoBehaviour
{
    float damage;
    float width;
    float range;
    float duration;
    float damageInterval = 0.2f;
    float damageTimer = 0f;
    float tickDamage;

    public SpriteRenderer laserSprite;
    private LineRenderer lineRenderer;
    private BoxCollider2D boxCollider;
    public bool followPlayer = false;
    public Transform playerRef; // 플레이어 참조

    Vector2 dir;

    void Awake()
    {
        if (laserSprite == null)
            laserSprite = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (laserSprite.enabled && followPlayer && playerRef != null)
        {
            // 시작점 갱신
            Vector2 origin = playerRef.position;
            laserSprite.transform.position = origin;

            // 끝점 갱신
            Vector2 end = origin + dir.normalized * range;
            Vector2 diff = end - origin;
            float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

            laserSprite.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            float spriteWidth = laserSprite.sprite.bounds.size.x;
            float spriteHeight = laserSprite.sprite.bounds.size.y;

            float scaleX = range / spriteWidth;
            float scaleY = width / spriteHeight;
            laserSprite.transform.localScale = new Vector3(scaleX, scaleY, 1f);

            damageTimer += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                ApplyLaserDamage();  // 적 탐지 후 데미지 주는 함수
                damageTimer = 0f;
            }

            // 이동 관련이나 무기 교체시 끊김
            if (Input.GetKeyDown(KeyCode.LeftShift) ||
            Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.S) ||
            Input.GetKeyDown(KeyCode.D) ||
            Input.GetKeyDown(KeyCode.Alpha1) ||
            Input.GetKeyDown(KeyCode.Alpha2) ||
            Input.GetKeyDown(KeyCode.Alpha3) ||
            Input.GetMouseButtonDown(1))
            {
                Disable();
            }
        }
    }

    public void SetStats(Vector2 dir, float final_damage, float final_width, float final_range, float final_duration)
    {
        damage = final_damage;
        width = final_width;
        range = final_range;
        duration = final_duration;
        tickDamage = damage / (duration / damageInterval); 
        Init(dir);
    }

    void Init(Vector2 direction)
    {
        dir = direction.normalized;

        Vector2 origin = transform.position;
        Vector2 end = origin + dir * range;
        Vector2 diff = end - origin;
        float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        // Sprite 조정
        laserSprite.transform.position = origin;
        laserSprite.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        float spriteWidth = laserSprite.sprite.bounds.size.x;
        float spriteHeight = laserSprite.sprite.bounds.size.y;
        float scaleX = range / spriteWidth;
        float scaleY = width / spriteHeight;

        laserSprite.transform.localScale = new Vector3(scaleX, scaleY, 1f);
        laserSprite.enabled = true;

        Invoke(nameof(Disable), duration);
    }

    void ApplyLaserDamage()
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Vector2 center = (Vector2)transform.position + dir * (range / 2f);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, new Vector2(range, width), angle);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                hit.GetComponent<Enemy>()?.Damaged((int)tickDamage, dir);
                Debug.Log($"[TickHit] Target: {hit.gameObject.name}, Damage: {(int)tickDamage}");
            }
        }
    }

    void Disable()
    {
        laserSprite.enabled = false;
        gameObject.SetActive(false);
    }
}
