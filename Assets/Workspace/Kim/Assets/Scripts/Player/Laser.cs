using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Laser : MonoBehaviour
{
    Vector2 direction;
    int laserDamage;
    float laserLength;
    float lifeTime; // 레이저 유지 시간
    float trackingRange;
    int remainingBounces;
    private HashSet<GameObject> hitEnemies; // 공격한 적 저장
    public bool followPlayer = false; // 직접 쏜 건 true, 튕긴 건 false
    public Transform playerRef; // 플레이어 참조
    public Transform weaponPoint; // 무기 상대 위치
    public AudioClip laser;
    [SerializeField] private SpriteRenderer laserSprite;

    void Awake()
    {
        if (laserSprite == null)
            laserSprite = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (laserSprite.enabled)
        {
            if (followPlayer && playerRef != null)
            {
                // 끝점은 유지하고 시작점을 플레이어 위치로 계속 맞춤
                Vector2 origin = weaponPoint.position;
                // 왼쪽 쏠 때 레이저가 무기보다 아래에 있어서 보정값 추가
                if (weaponPoint.localPosition.x < 0f)
                {
                    float yOffset = 0.1f;
                    origin += Vector2.up * yOffset;
                }
                Vector2 endPoint = origin + direction.normalized * laserLength;
                Vector2 diff = endPoint - origin;

                Vector2 dir = endPoint - origin;
                float length = dir.magnitude;

                // 위치: 시작점 = 플레이어 위치
                laserSprite.transform.position = origin;

                // 회전
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                laserSprite.transform.rotation = Quaternion.Euler(0f, 0f, angle);

                // 길이 조정
                laserSprite.size = new Vector2(length, laserSprite.size.y);
            }
        }
    }
    
    public void SetStats(
    Vector2 dir,
    int final_damage,
    float final_length,
    float final_lifetime,
    float final_trackingRange,
    int final_remainingBounces,
    HashSet<GameObject> passedHitEnemies = null)
    {
        laserDamage = final_damage;
        laserLength = final_length;
        lifeTime = final_lifetime;
        trackingRange = final_trackingRange;
        remainingBounces = final_remainingBounces;
        
        Init(dir, remainingBounces, passedHitEnemies);
    }

    void Init(Vector2 dir, int bounces = 0, HashSet<GameObject> previousHits = null)
    {
        if (laser == null)
        {
            Debug.LogWarning("attackSFX is null!");
        }
        else
        {
            Debug.Log($"attackSFX name: {laser}");
            SoundManager.inst.PlaySFX(laser);
        }

        direction = dir.normalized;

        if (previousHits == null)
            hitEnemies = new HashSet<GameObject>();
        else
            hitEnemies = new HashSet<GameObject>(previousHits); // 복사하여 유지

        Vector2 origin = transform.position;
        Vector2 rayStart = origin;
        float rayLength = laserLength;

        RaycastHit2D[] hits = Physics2D.RaycastAll(
            rayStart,
            direction,
            rayLength,
            LayerMask.GetMask("Enemy")
        );

        RaycastHit2D? closestValidHit = null;
        float closestDistance = float.MaxValue;

        foreach (var h in hits)
        {
            GameObject obj = h.collider.gameObject;

            // 이미 맞은 적은 제외
            if (obj.CompareTag("Enemy") && hitEnemies.Contains(obj))
                continue;

            float dist = Vector2.Distance(rayStart, h.point);
            if (dist < closestDistance)
            {
                closestValidHit = h;
                closestDistance = dist;
            }
        }

        Vector2 endPoint = origin + direction * laserLength;

        if (closestValidHit != null)
        {
            RaycastHit2D hit = closestValidHit.Value;

            if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("BreakableBox"))
            {
                GameObject hitEnemy = hit.collider.gameObject;

                // 중심으로 레이저 시각 조정
                endPoint = hit.collider.bounds.center;

                // 데미지 처리
                hitEnemy.GetComponent<Enemy>()?.Damaged(laserDamage, direction);
                hitEnemy.GetComponent<BreakableBox>()?.Damaged(laserDamage);
                hitEnemies.Add(hitEnemy);
                GameManager.inst.combatEffectManager.DoHitStop(0.02f);

                // 튕김
                if (remainingBounces > 0)
                {
                    StartCoroutine(BounceToNearbyEnemies(hit.collider.bounds.center));
                }
            }
        }

        laserSprite.transform.position = origin;

        Vector2 diff = endPoint - origin;
        float distance = diff.magnitude;
        float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        laserSprite.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        float spriteWorldWidth = laserSprite.sprite.bounds.size.x; // 월드 단위 가로 길이
        float desiredWorldLength = distance;
        float scaleX = desiredWorldLength / spriteWorldWidth;
        laserSprite.transform.localScale = new Vector3(scaleX, 1f, 1f);
        laserSprite.enabled = true;

        CancelInvoke();
        Invoke(nameof(DisableLaser), lifeTime);
    }

    public void DisableLaser()
    {
        laserSprite.enabled = false;
        hitEnemies = null;
        gameObject.SetActive(false);
    }

    IEnumerator BounceToNearbyEnemies(Vector2 from)
    {
        yield return new WaitForSeconds(0.05f);

        Collider2D[] nearby = Physics2D.OverlapCircleAll(from, trackingRange, LayerMask.GetMask("Enemy"));
        foreach (var col in nearby)
        {
            if (!hitEnemies.Contains(col.gameObject))
            {
                GameObject next = GameManager.inst.pool.GetRange(2);
                next.transform.position = from;

                Vector2 nextDir = (col.transform.position - (Vector3)from).normalized;
                Laser laser = next.GetComponent<Laser>();
                laser.SetStats(nextDir, laserDamage, laserLength, lifeTime, trackingRange, remainingBounces - 1, hitEnemies);

                break; // 한 명만
            }
        }
    }
}
