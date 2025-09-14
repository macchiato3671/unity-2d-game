using UnityEngine;
using System.Collections;

public class EnemySkill : MonoBehaviour
{
    public GameObject player; // 플레이어 참조  
    public GameObject bulletPrefab; // Bullet 프리팹  

    public float reflectSpeed = 10f; // 반사 총알 속도  
    public float shootSpeed = 8f; // 무기 발사 속도  
    public float teleportDistance = 3f; // 순간이동 후 돌진 거리  
    public float bulletDestroyTime = 5f; // Bullet의 최대 수명  

    public int surroundBulletCount = 8; // 두 번째 스킬: 생성할 총알 개수  
    public float surroundRadius = 3f; // 두 번째 스킬: 생성할 총알 반지름  
    public float surroundDelay = 2f; // 두 번째 스킬: 총알 생성 후 플레이어를 향해 돌진까지의 대기 시간  

    private float secondSkillCooldown = 5f; // 두 번째 스킬 쿨타임  
    private float thirdSkillCooldown = 10f; // 세 번째 스킬 쿨타임  

    private bool canUseSecondSkill = true; // 두 번째 스킬 사용 가능 여부  
    private bool canUseThirdSkill = true; // 세 번째 스킬 사용 가능 여부  

    /*
     * 1. 플레이어의 스킬 반사(enemy parrying) (총알반사로구현)
     * 2. 플레이어 위치 기준 구형으로 총알 생성 및 돌진 (Song>prefab>range1 총알로 사용)
     * 3. 플레이어 근처 위치로 순간이동 후 1초뒤 돌진
     */

    void Start()
    {
        // 플레이어를 확인  
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    void Update()
    {
        // 두 번째 스킬: 일정 시간마다 발동  
        if (canUseSecondSkill)
        {
            StartCoroutine(TriggerSecondSkill());
        }

        // 세 번째 스킬: 일정 시간마다 발동  
        if (canUseThirdSkill)
        {
            StartCoroutine(TriggerThirdSkill());
        }
    }

    // 첫 번째 스킬: 플레이어의 공격 반사 (플레이어 불렛이 닿을 경우) 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            float chance = Random.value; // 0.0 ~ 1.0 사이의 랜덤 값

            if (chance <= 0.3f) // 30% 확률로 Reflect
            {
                ReflectSkill(collision.gameObject);
            }
            else
            {
                Destroy(collision.gameObject); // 반사안되면 사라짐. -> damaged 추가필요
            }
        }
    }

    public void ReflectSkill(GameObject playerBullet)
    {
        // 플레이어 총알을 반사하여 다시 플레이어를 향해 날아가게 함  
        Vector2 direction = (player.transform.position - playerBullet.transform.position).normalized;

        Range bulletRange = playerBullet.GetComponent<Range>();

        if (bulletRange != null)
        {
            bulletRange.Reflect(direction, 10);
        }

        // 총알 태그를 변경하여 적 총알로 인식되게 함 (플레이어가 맞도록)  
        playerBullet.tag = "EnemyBullet";
    }

    // 두 번째 스킬: 플레이어 위치 기준으로 구형으로 총알 생성 후 돌진  
    public IEnumerator TriggerSecondSkill()
    {
        canUseSecondSkill = false; // 스킬 사용 불가  
        SurroundShootSkill(surroundBulletCount, surroundRadius, surroundDelay); // 두 번째 스킬 발동  
        yield return new WaitForSeconds(secondSkillCooldown); // 쿨타임 대기  
        canUseSecondSkill = true; // 스킬 사용 가능  
    }

    public void SurroundShootSkill(int bulletCount, float radius, float delay)
    {
        StartCoroutine(SurroundShootCoroutine(bulletCount, radius, delay));
    }

    private IEnumerator SurroundShootCoroutine(int bulletCount, float radius, float delay)
    {
        Vector2 playerPosition = player.transform.position; // 플레이어의 초기 위치를 저장  

        // 플레이어를 중심으로 구 형태로 총알 생성  
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * (360f / bulletCount); // 균등한 각도  
            Vector2 spawnPosition = playerPosition + new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;

            Vector2 directionToPlayer = (playerPosition - spawnPosition).normalized;
            float angleZ = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, 0, angleZ);
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, rotation);
            bullet.tag = "EnemyBullet";

            // BoxCollider2D로 변경된 구성  
            BoxCollider2D bulletCollider = bullet.GetComponent<BoxCollider2D>();
            if (bulletCollider != null)
            {
                bulletCollider.isTrigger = true; // 충돌을 트리거로 설정하여 서로 만나면 사라지게 함  
            }

            bullet.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero; // 초기 속도 없음  

            BulletDirectionMemory memory = bullet.AddComponent<BulletDirectionMemory>();
            memory.targetPosition = playerPosition;

            // Bullet 삭제를 위한 자동 제거 처리  
            Destroy(bullet, bulletDestroyTime);
        }

        yield return new WaitForSeconds(delay); // 일정 시간 대기  

        // 생성된 총알들을 플레이어를 향해 돌진하도록 설정  
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        foreach (GameObject bullet in bullets)
        {
            BulletDirectionMemory memory = bullet.GetComponent<BulletDirectionMemory>();
            if (memory != null)
            {
                Vector2 dir = (memory.targetPosition - (Vector2)bullet.transform.position).normalized;
                bullet.GetComponent<Rigidbody2D>().linearVelocity = dir * shootSpeed;
            }
        }
    }

    // 세 번째 스킬: 순간이동 후 돌진  
    public IEnumerator TriggerThirdSkill()
    {
        canUseThirdSkill = false; // 스킬 사용 불가  
        yield return StartCoroutine(TeleportAndDashSkill());
        yield return new WaitForSeconds(thirdSkillCooldown); // 쿨타임 대기  
        canUseThirdSkill = true; // 스킬 사용 가능  
    }

    public IEnumerator TeleportAndDashSkill()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 teleportPosition = Vector2.zero;
        bool validPositionFound = false;

        int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            // 주변 랜덤 위치 선택
            Vector2 candidatePosition = playerPosition + (Random.insideUnitCircle * teleportDistance);

            // 1. 아래 방향으로 Raycast해서 Platform이 있는지 확인
            RaycastHit2D groundHit = Physics2D.Raycast(candidatePosition, Vector2.down, 1.5f, LayerMask.GetMask("Platform"));

            // 2. 그 위치에 뭔가 박혀 있는지 (즉, Platform 안에 파묻히는 경우) 확인
            Collider2D overlap = Physics2D.OverlapCircle(candidatePosition, 0.4f, LayerMask.GetMask("Platform"));

            // 조건: 아래에 Platform 있고, 현재 위치에 충돌 없음 (공중)
            if (groundHit.collider != null && overlap == null)
            {
                teleportPosition = candidatePosition;
                validPositionFound = true;
                break;
            }
        }

        if (!validPositionFound)
        {
            Debug.LogWarning("유효한 순간이동 위치를 찾지 못함 (플랫폼 위 or 공중).");
            yield break;
        }

        // 순간이동
        transform.position = teleportPosition;

        // 1초 대기
        yield return new WaitForSeconds(1f);

        // 돌진 방향 계산
        Vector2 dashDirection = (playerPosition - teleportPosition).normalized;

        // 돌진 실행
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = dashDirection * 10f;
    } //필요할 시에 돌진 후 플레이어와 부딪혔을 때에 EnemyMove(script)>knockBack 코루틴 추가해도 ㄱㅊ
}