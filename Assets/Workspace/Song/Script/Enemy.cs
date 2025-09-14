using System;
using System.Collections;
using System.Data;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // 컴포넌트
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator anim;
    [SerializeField] private Collider2D coll;
    [SerializeField] private EnemyHackable hackable;
    [SerializeField] private BossController bossController;


    Transform player;
    WorldUI worldUI;
    BossUI bossUI;
    EnemyHealthUI healthUI;

    public static event Action<int> OnEnemyDied;

    //부품프리팹
    public GameObject resourcePrefab;

    // Init되는 변수
    // 에너미 공통 변수
    public int enemyID; // (김택림) private -> public으로 변경했습니다
    float defaultSpeed;
    float chaseSpeed;
    float detectionRange;
    int attackPower;
    int maxHealth;
    public int MaxHealth => maxHealth;
    float knockbackForce;
    AudioClip[] audioClips;
    // 투사체 발사 에너미 관련 변수
    float attackCooldown;
    float bulletSpeed;
    Vector3 firePoint;
    public Transform target; // 플레이어 or 전향된 적의 적

    // 그 외 내부 사용 변수
    // 에너미 공통 변수
    Vector2 _externalForce = Vector2.zero; // 외부에서 작용하는 힘
    public Vector2 externalForce
    {
        get
        {
            return _externalForce;
        }
        set
        {
            _externalForce = value;
        }
    }

    int currentHealth;
    public int CurrentHealth => currentHealth; // 현재 체력

    float adjustPositionTime; // 에너미가 제자리에 멈춰버리는 버그 방지를 위한 위치 변경 타이머 

    float flipCooldown = 0.5f; // 방향 변경 쿨타임
    float lastFlipTime; // 방향 변경 쿨타임 적용을 위한 변수
    float lastSpriteFlipTime; // 스프라이트의 Flip 방향 변경 쿨타임 적용을 위한 변수

    float distance; // 플레이어와 에너미간의 거리 계산시 사용

    bool movingRight = true;

    bool isKnockedBack = false; // 피격시 넉백되며 true
    bool isCollided = false;  // 플레이어와 직접 충돌시 true
    bool isInRange = false; // 플레이어가 범위 안에 있을때 true
    bool isDead = false; // 사망시 true
    public bool isOnTrait = false; // 에너미 별 특성 작동시 true // (김택림) private -> public으로 변경했습니다
    bool isInvincible = false; // 무적시 true

    float lastAttackTime; // 투사체 발사 에너미 관련 변수

    AnimatorStateInfo info; // 애니메이션 상태 저장 변수

    //(김택림) 추가 변수
    bool isSlowed = false;
    float slowAmount;
    float slowTimer = 0f;
    float speedFactor;
    BossHealthUI bossHealthUI;
    //int patternIndex = 0; // 패턴 순서 관리

    [SerializeField] private AudioClip damagedSound;
    [SerializeField] private AudioClip deathSound;

    private EnemyData data;
    IEnumerator Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;
        target = player;

        yield return null; // 프레임 한 번 기다림
    }
    
    void OnDisable()
    {
        OnEnemyDied.Invoke(0);
        if (healthUI != null) healthUI.gameObject.SetActive(false);
    }

    Transform FindClosestEnemyTarget()
    {
        float minDist = Mathf.Infinity;
        Transform closest = null;
        if (hackable != null && hackable.IsTurncoat())
        {
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 15f, LayerMask.GetMask("Enemy"));
            foreach (var col in enemies)
            {
                if (col.gameObject == gameObject) continue;

                Enemy enemy = col.GetComponent<Enemy>();
                if (enemy != null && enemy.enemyID >= 10)
                {
                    return enemy.transform;
                }
            }
        }


        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, 15f, LayerMask.GetMask("Enemy"));
        foreach (var col in nearbyEnemies)
        {
            if (col.gameObject == gameObject) continue;

            EnemyHackable other = col.GetComponent<EnemyHackable>();
            if (other != null && !other.IsTurncoat())
            {
                float dist = Vector2.Distance(transform.position, col.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = col.transform;
                }
            }
        }
        if (closest == null) closest = transform;
        return closest;
    }
    Transform FindBossTarget()
    {
        GameObject boss = GameObject.FindGameObjectWithTag("Boss"); // Boss 태그 필요
        if (boss != null)
        {
            return boss.transform;
        }
        return null;
    }



    void FixedUpdate()
    {
        if (isDead || isKnockedBack) return;
        Vector2 downOrigin = new Vector2(transform.position.x, transform.position.y);
        RaycastHit2D downInfo = Physics2D.Raycast(downOrigin, Vector2.down, 0.04f, LayerMask.GetMask("Platform"));
        Debug.DrawRay(downOrigin, Vector2.down * 0.04f, downInfo.collider != null ? Color.green : Color.red);

        if (isPatternActive)
        {
            rigid.linearVelocity = new Vector2(0, rigid.linearVelocityY);
            return;
        }
        if (hackable.IsStunned())
        {
            rigid.linearVelocity = new Vector2(0, rigid.linearVelocityY);
            return;
        }
        if (hackable.IsTurncoat())
        {
            if (enemyID == 4) // 4번 자폭병
            {
                target = FindBossTarget();
            }
            else
            {
                target = FindClosestEnemyTarget();
            }
        }
        else
        {
            target = player;
        }

        // Turncoat 자폭병이 보스 근처에 가면 자폭
        if (hackable.IsTurncoat() && enemyID == 4 && target != null)
        {
            float explodeDistance = 2f; // 거리 2 이하일 때 폭발
            if (Vector2.Distance(transform.position, target.position) <= explodeDistance)
            {
                Debug.Log($"{name} 보스 근처에서 자폭!");

                // 보스 데미지
                Enemy bossEnemy = target.GetComponent<Enemy>();
                if (bossEnemy != null)
                {
                    bossEnemy.Damaged(20, (target.position - transform.position).normalized); // 20 데미지
                }

                // 자폭 이펙트
                Transform explosion = GameManager.inst.pool.GetRange(5).transform;
                explosion.localPosition = transform.position + new Vector3(0, 0.5f, 0);
                explosion.localRotation = Quaternion.identity;

                gameObject.SetActive(false); // 자폭 후 본인 제거
            }
        }


        if (!isDead && EnemyHackable.currentLureTarget != null && EnemyHackable.currentLureTarget != transform)
        {
            Vector2 dir = (EnemyHackable.currentLureTarget.position - transform.position).normalized;
            Adjust();
            if (downInfo.collider == null) rigid.gravityScale = 10f;
            else rigid.gravityScale = 0;
            rigid.linearVelocity = new Vector2(dir.x * chaseSpeed * 1.5f, rigid.linearVelocityY);
            anim.SetTrigger("Walk");
            return;
        }
        if (isKnockedBack) return; // 피격에 의한 넉백, 사망, 해킹으로 인한 스턴시에는 이동 처리 X
        else if (isDead)
        {
            rigid.linearVelocity = new Vector2(0, rigid.linearVelocityY);
            rigid.angularVelocity = 0f;
            return;
        }
        else if (hackable.IsStunned())
        {
            rigid.gravityScale = 10f;
            rigid.linearVelocity = new Vector2(0, rigid.linearVelocityY);
            rigid.angularVelocity = 0f;
            return;
        }

        Adjust(); // 주기적으로 바닥에 끼는 버그 방지

        // 공중에 떠있을 때에만 중력 적용(이렇게 안 하면 에너미가 갑자기 멈춰버리는 버그 발생)
        if (downInfo.collider == null) rigid.gravityScale = 10f;
        else rigid.gravityScale = 0;
        //

        distance = Vector2.Distance(transform.position, target.position);
        isInRange = (distance < detectionRange) ? true : false;

        switch (enemyID)
        { // 에너미 종류에 따른 처리
            case 0: // 단순 추적
                // 플레이어 탐지 로직
                if (isInRange)
                    ChasePlayer();
                else Idle();
                break;
            case 1: // 배회하다가 원거리 공격
                if (isInRange)
                {
                    // 공격 쿨다운이 끝났다면 공격
                    if (Time.time > lastAttackTime + attackCooldown)
                    {
                        RangeAttack();
                        lastAttackTime = Time.time; // 공격 시간 업데이트  
                    }
                    else anim.SetTrigger("Idle");
                }
                else
                {
                    anim.SetTrigger("Walk");
                    Idle();
                }
                break;
            case 2: // 차징 후 돌진
                if (!isOnTrait)
                {
                    if (isInRange)
                    {
                        StartCoroutine(Rush()); // 코루틴 시작시 isOnTrait = true, 끝나면 false
                    }
                    else Idle();
                }
                break;
            case 3: // 차징 후 점프
                if (!isOnTrait)
                {
                    if (isInRange)
                    {
                        StartCoroutine(Jump()); // 코루틴 시작시 isOnTrait = true, 끝나면 false
                    }
                    else
                    {
                        anim.SetTrigger("Walk");
                        Idle();
                    }
                }
                break;
            case 4: // 비행하다가 돌진 후 자폭
                rigid.gravityScale = 0f;
                if (!isOnTrait)
                {
                    if (isInRange)
                    {
                        StartCoroutine(RushExplode()); // 코루틴 시작시 isOnTrait = true, 끝나면 false
                    }
                    else
                    {
                        anim.SetTrigger("Walk");
                        Idle_Fly();
                    }
                }
                break;
            case 5: // 숨어있다가 가까이 오면 연사
                if (isInRange)
                {
                    isInvincible = false;

                    info = anim.GetCurrentAnimatorStateInfo(0);
                    if (info.IsName("Hide")) anim.SetTrigger("Walk");

                    if (!isOnTrait) StartCoroutine(RangeAttack2());
                }
                else
                {
                    isInvincible = true;
                    anim.SetTrigger("Hide");
                }
                rigid.linearVelocityX = 0;
                break;
            case 101:
            case 103:
            case 104:
                // 101번, 103번: 1번 적 행동 + 보스 패턴도 써야 함
                if (!isPatternActive) // 패턴 중이 아닐 때만 사격
                {
                    if (isInRange)
                    {
                        if (Time.time > lastAttackTime + attackCooldown)
                        {
                            RangeAttack();
                            lastAttackTime = Time.time;
                        }
                        else
                        {
                            anim.SetTrigger("Idle");
                        }
                    }
                    else
                    {
                        anim.SetTrigger("Walk");
                        Idle();
                    }
                }
                else
                {
                    // 패턴 중이면 움직이기만 가능 (또는 가만히)
                    anim.SetTrigger("Idle");
                    rigid.linearVelocityX = 0f;
                }

                HandleBossEnemy_Walk(); // 보스 컨트롤러 패턴 처리
                break;

            case 10:
            case 100:
            case 102:
                rigid.gravityScale = 0f; // 부유
                HandleBossEnemy();
                break;
        }


        // 외부 힘 추가
        //rigid.AddForce(externalForce);
        switch (enemyID)
        {
            case 5:
            case 10:
            case 100:
            case 101:
            case 102:
            case 103:
            case 104:
                break;
            default:
                if (externalForce != Vector2.zero)
                    rigid.linearVelocity = 0.1f * rigid.linearVelocity + 5 * (externalForce.y >= 0 ? externalForce : new Vector2(externalForce.x, 0));
                externalForce = Vector2.zero;
                break;
        }

        //(김택림)
        //적 느려지는 디버프(emp slash효과)
        speedFactor = isSlowed ? slowAmount : 1f;
        if (isSlowed)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0f)
            {
                isSlowed = false;
                slowAmount = 1f;
            }
        }
    }
    bool isPatternActive = false; // 보스 패턴 진행 중이면 true
    public void SetPatternActive(bool active)
    {
        isPatternActive = active;
    }

    void HandleBossEnemy_Walk()
    {
        if (isDead) return;

        if (bossController != null && !bossController.isActiveAndEnabled)
        {
            bossController.enabled = true;
        }
    }

    void HandleBossEnemy()
    {
        if (isDead) return;

        if (rigid.bodyType != RigidbodyType2D.Kinematic)
            rigid.bodyType = RigidbodyType2D.Kinematic;

        rigid.gravityScale = 0f; // 항상 부유

        if (bossController != null)
        {
            if (!isPatternActive) // 패턴 중에는 이동 멈춤
            {
                ChasePlayer();
            }

            if (!bossController.isActiveAndEnabled)
            {
                bossController.enabled = true;
            }
        }
    }


    void LateUpdate()
    {
        if (isKnockedBack || isOnTrait || isDead) return;

        if (isInRange)
        {
            if ((target.position.x >= transform.position.x) && (sprite.flipX == true) && (Time.time > lastSpriteFlipTime + flipCooldown))
            {
                sprite.flipX = false;
                lastSpriteFlipTime = Time.time;
            }
            else if ((target.position.x < transform.position.x) && (sprite.flipX == false) && (Time.time > lastSpriteFlipTime + flipCooldown))
            {
                sprite.flipX = true;
                lastSpriteFlipTime = Time.time;
            }
        }
        else
        {
            if (enemyID == 5) return;
            if ((rigid.linearVelocityX >= 0) && (sprite.flipX == true) && (Time.time > lastSpriteFlipTime + flipCooldown))
            {
                sprite.flipX = false;
                lastSpriteFlipTime = Time.time;
            }
            else if ((rigid.linearVelocityX < 0) && (sprite.flipX == false) && (Time.time > lastSpriteFlipTime + flipCooldown))
            {
                sprite.flipX = true;
                lastSpriteFlipTime = Time.time;
            }
        }
    }

    public void Init(EnemyData enemyData)
    {

        this.data = enemyData;
        //(김택림) 이전에 setactive(false)했던 enemy pool하는 과정에서 이전 정보가 저장되어 추가했습니다.
        StopAllCoroutines();

        enemyID = data.enemyID;
        defaultSpeed = data.defaultSpeed;
        chaseSpeed = data.chaseSpeed;
        detectionRange = data.detectionRange;
        attackPower = data.attackPower;
        maxHealth = data.maxHealth;
        knockbackForce = data.knockbackForce;
        transform.localScale = data.size;

        attackCooldown = data.attackCooldown;
        bulletSpeed = data.bulletSpeed;
        firePoint = data.firePoint;
        anim.runtimeAnimatorController = data.animationController;

        coll.offset = data.colliderOffset;
        coll.GetComponent<BoxCollider2D>().size = data.colliderSize;

        audioClips = data.audioClips;

        currentHealth = maxHealth;

        movingRight = true;
        isKnockedBack = false;
        isInRange = false;
        isDead = false;
        isOnTrait = false;
        isInvincible = false;

        transform.tag = "Enemy"; // 태그, 충돌 레이어 처리 재설정
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        coll.excludeLayers &= ~LayerMask.GetMask("Player");

        GetComponent<PlayerConfiner>().SetMax((int)GameManager.inst.roomManager.curRoom.GetPortalPos().x);

        sprite.color = Color.white;

        if (UnityEngine.Random.Range(0, 2) == 0) movingRight = !movingRight; // 시작 방향 랜덤
        if (worldUI == null) worldUI = GameManager.inst.worldUI;
        if (bossUI == null) bossUI = GameManager.inst.bossUI;

        if (enemyID >= 10)
        {
            if (bossController != null)
            {
                if(enemyID >= 100) bossHealthUI = bossUI.GetBossUI(gameObject, data.description, maxHealth);
                if (enemyID == 102 || enemyID == 103 || enemyID == 104)
                {
                    bossController.currentDifficulty = Difficulty.Hard;
                }
                else
                {
                    bossController.currentDifficulty = Difficulty.Easy;
                }
            }
        }
        else
        {
            healthUI = worldUI.GetEnemyUI(gameObject);
        }
    }

    // 에너미 공통 함수(피격, 넉백, 사망 처리)
    public void Damaged(int damage, Vector2 dir)
    {
        if (isDead || isInvincible) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} 체력 감소: {currentHealth + damage} -> {currentHealth} (피해량: {damage})");

        isInvincible = true;
        anim.SetTrigger("Hurt");

        SoundManager.inst.PlaySFX(damagedSound);

        worldUI.GetDamageUI(transform.position, damage);

        if (currentHealth <= 0)
        {
            StartCoroutine(Dead());
            return;
        }

        StartCoroutine(Knockback(dir));
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;
        // Debug.Log("gd");
        // Debug.Log(coll);
        //Debug.Log(coll.gameObject.name);
        // [전향 상태일 때] 다른 적을 공격
        

        // 플레이어 공격 처리 (기존)
        // if (collision.gameObject.CompareTag("Player"))
        // {
        //     Vector2 KnockBackDirection = (transform.position - collision.transform.position).normalized;
        //     Damaged(20, KnockBackDirection);
        //     isCollided = true;
        // }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead || isCollided) return;

        if (hackable.IsTurncoat() && collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("공격");
            Enemy other = collision.gameObject.GetComponent<Enemy>();
            if (other != null && !other.hackable.IsTurncoat() && !other.isDead)
            {
                Vector2 dir = (other.transform.position - transform.position).normalized;
                other.Damaged(attackPower, dir);
                Debug.Log($"{name} (전향)이 {other.name}에게 {attackPower} 피해!");
            }
            return;
        }

        // 플레이어 공격 처리 (기존)
        if (collision.gameObject.CompareTag("Player"))
        {
            if (enemyID == 0) SoundManager.inst.PlaySFX(audioClips[0]);
            Vector2 KnockBackDirection = (transform.position - collision.transform.position).normalized;
            Damaged(5, KnockBackDirection);
            isCollided = true;
        }
    }

    public void ReceiveDamageFromTurncoat(int damage, Vector2 dir)
    {
        if (isDead || isInvincible) return;

        currentHealth -= damage;
        anim.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            StartCoroutine(Dead());
        }
        else
        {
            StartCoroutine(Knockback(dir));
        }
    }

    IEnumerator Knockback(Vector2 direction)
    {
        if (enemyID >= 10)
        {
            yield return new WaitForSeconds(0.2f);
            isInvincible = false;
            yield break;
        }
        isKnockedBack = true;
        rigid.linearVelocity = direction * knockbackForce;
        yield return new WaitForSeconds(0.2f);

        isKnockedBack = false;
        isCollided = false;
        isInvincible = false;
    }

    IEnumerator Dead()
    {
        isDead = true;
        rigid.linearVelocity = Vector2.zero;
        rigid.gravityScale = 10f;
        transform.tag = "Untagged";
        gameObject.layer = LayerMask.NameToLayer("Default");
        coll.excludeLayers = LayerMask.GetMask("Player");

        anim.SetTrigger("Death");

        SoundManager.inst.PlaySFX(deathSound);

        TryDropResource(transform.position + new Vector3(0, 1f, 0));
        GameManager.inst.accessoryDropManager.TrySpawnAccessoryDrop(transform.position + new Vector3(0, 1f, 0));

        if (bossHealthUI != null) bossHealthUI.gameObject.SetActive(false); // 풀로 반납

        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }


    private void TryDropResource(Vector3 dropPosition)
    {
        float dropChance = 0.8f; // 80% 확률
        if (enemyID >= 10) dropChance = 1f;

        if (UnityEngine.Random.value <= dropChance)
        {
            GameObject resourceObj = Instantiate(resourcePrefab, dropPosition, Quaternion.identity);
            resourceObj.GetComponent<Resource>().SetAmount(enemyID >= 10? 30 : UnityEngine.Random.Range(2, 5));
        }
    }

    //
    void ChasePlayer()
    {
        float direction = target.position.x >= transform.position.x ? 1f : -1f;
        //(김택림) speedFactor변수 곱하기 추가
        rigid.linearVelocity = new Vector2(direction * chaseSpeed * speedFactor, rigid.linearVelocity.y);
    }

    void Idle()
    {
        float moveDirection = movingRight ? 1f : -1f;

        // 이동 설정  
        rigid.linearVelocity = new Vector2(moveDirection * defaultSpeed, rigid.linearVelocity.y);

        // 적의 Collider 크기 정보 가져오기  
        float colliderWidth = coll.bounds.extents.x; // Collider의 가로 반지름  
        float colliderHeight = coll.bounds.extents.y; // Collider의 세로 반지름  

        // Raycast 발사 위치 계산 (좌하단,우하단,좌,우 각각)  
        Vector2 leftdownOrigin = new Vector2(transform.position.x - colliderWidth, transform.position.y);
        Vector2 rightdownOrigin = new Vector2(transform.position.x + colliderWidth, transform.position.y);
        Vector2 leftOrigin = new Vector2(transform.position.x - colliderWidth, transform.position.y);
        Vector2 rightOrigin = new Vector2(transform.position.x + colliderWidth, transform.position.y);

        // Raycast를 아래로 발사하여 바닥 감지 (좌하단,우하단,좌,우 각각)  
        RaycastHit2D leftdownInfo = Physics2D.Raycast(leftdownOrigin, Vector2.down, 0.5f, LayerMask.GetMask("Platform"));
        RaycastHit2D rightdownInfo = Physics2D.Raycast(rightdownOrigin, Vector2.down, 0.5f, LayerMask.GetMask("Platform"));
        RaycastHit2D leftInfo = Physics2D.Raycast(leftOrigin, Vector2.left, 1f, LayerMask.GetMask("Platform"));
        RaycastHit2D rightInfo = Physics2D.Raycast(rightOrigin, Vector2.right, 1f, LayerMask.GetMask("Platform"));

        // 디버깅용: Raycast를 시각적으로 표시  
        Debug.DrawRay(leftdownOrigin, Vector2.down * 0.5f, leftdownInfo.collider != null ? Color.green : Color.red);
        Debug.DrawRay(rightdownOrigin, Vector2.down * 0.5f, rightdownInfo.collider != null ? Color.green : Color.red);
        Debug.DrawRay(leftOrigin, Vector2.left * 1f, leftInfo.collider != null ? Color.green : Color.red);
        Debug.DrawRay(rightOrigin, Vector2.right * 1f, rightInfo.collider != null ? Color.green : Color.red);

        // 좌하단 우하단 중 하나라도 바닥을 감지하지 못하면 Flip  
        if ((leftdownInfo.collider == null || rightdownInfo.collider == null) && Time.time > lastFlipTime + flipCooldown)
        {
            movingRight = !movingRight;
            lastFlipTime = Time.time;
        }
        // 좌 우 중 하나라도 벽을 감지하면 Flip(벽에 가로막혔을시 반대 방향으로)
        else if ((leftInfo.collider != null || rightInfo.collider != null) && Time.time > lastFlipTime + flipCooldown)
        {
            movingRight = !movingRight;
            lastFlipTime = Time.time;
        }
        else if (Time.time > lastFlipTime + 5f)
        { // 주기적으로 Flip
            movingRight = !movingRight;
            lastFlipTime = Time.time;
        }
    }

    void Idle_Fly()
    {
        float moveDirection = movingRight ? 1f : -1f;

        // 이동 설정  
        rigid.linearVelocity = new Vector2(moveDirection * defaultSpeed, 0);

        // 적의 Collider 크기 정보 가져오기  
        float colliderWidth = coll.bounds.extents.x; // Collider의 가로 반지름  
        float colliderHeight = coll.bounds.extents.y; // Collider의 세로 반지름  

        // Raycast 발사 위치 계산 (좌,우 각각)  
        Vector2 leftOrigin = new Vector2(transform.position.x - colliderWidth, transform.position.y);
        Vector2 rightOrigin = new Vector2(transform.position.x + colliderWidth, transform.position.y);

        // Raycast를 아래로 발사하여 바닥 감지 (좌,우 각각)  
        RaycastHit2D leftInfo = Physics2D.Raycast(leftOrigin, Vector2.left, 1f, LayerMask.GetMask("Platform"));
        RaycastHit2D rightInfo = Physics2D.Raycast(rightOrigin, Vector2.right, 1f, LayerMask.GetMask("Platform"));

        // 디버깅용: Raycast를 시각적으로 표시  
        Debug.DrawRay(leftOrigin, Vector2.left * 1f, leftInfo.collider != null ? Color.green : Color.red);
        Debug.DrawRay(rightOrigin, Vector2.right * 1f, rightInfo.collider != null ? Color.green : Color.red);

        // 좌 우 중 하나라도 벽을 감지하면 Flip(벽에 가로막혔을시 반대 방향으로)
        if ((leftInfo.collider != null || rightInfo.collider != null) && Time.time > lastFlipTime + flipCooldown)
        {
            movingRight = !movingRight;
            lastFlipTime = Time.time;
        }
        else if (Time.time > lastFlipTime + 5f)
        {
            movingRight = !movingRight;
            lastFlipTime = Time.time;
        }
    }

    void RangeAttack()
    {
        if (player == null) return; // 플레이어가 없으면 공격하지 않음  

        SoundManager.inst.PlaySFX(audioClips[0]);

        anim.SetTrigger("Attack");

        rigid.linearVelocityX = 0;
        movingRight = !movingRight;

        // 플레이어 방향 계산  
        Vector2 direction = (target.position - transform.position).normalized;

        Transform bullet = GameManager.inst.pool.GetRange(1).transform;
        bullet.localPosition = transform.position + (direction.x >= 0 ? firePoint : new Vector2(-firePoint.x, firePoint.y));
        bullet.localRotation = Quaternion.identity;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * bulletSpeed;

        bullet.GetComponent<Bullet>().Init(direction);
    }

    IEnumerator RangeAttack2()
    {
        isOnTrait = true;
        sprite.flipX = (target.position.x >= transform.position.x) ? false : true;
        anim.SetTrigger("Attack");

        float direction = target.position.x - transform.position.x > 0 ? 1f : -1f;

        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(attackCooldown);
            if (!isInRange || isDead) break;

            SoundManager.inst.PlaySFX(audioClips[0]);

            Transform bullet = GameManager.inst.pool.GetRange(1).transform;
            bullet.localPosition = transform.position + (direction >= 0 ? firePoint : new Vector2(-firePoint.x, firePoint.y));
            bullet.localRotation = Quaternion.identity;

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = new Vector2(direction, 0) * bulletSpeed;

            bullet.GetComponent<Bullet>().Init(new Vector2(direction, 0));
        }
        if (isInRange) yield return new WaitForSeconds(1f);

        isOnTrait = false;
    }

    IEnumerator Rush()
    {
        float direction, savedForce;

        SoundManager.inst.PlaySFX(audioClips[0]);

        isOnTrait = true;
        rigid.linearVelocity = Vector2.zero;
        sprite.flipX = (target.position.x >= transform.position.x) ? false : true;
        savedForce = knockbackForce;
        knockbackForce = 0f;

        yield return new WaitForSeconds(0.8f);
        //(김택림) speedfactor곱셈 추가
        if (!isDead)
        {
            SoundManager.inst.PlaySFX(audioClips[1]);

            sprite.flipX = (target.position.x >= transform.position.x) ? false : true;
            direction = target.position.x >= transform.position.x ? 1f : -1f;
            sprite.color = Color.red;
            rigid.linearVelocity = new Vector2(direction * 15 * speedFactor, rigid.linearVelocity.y);
        }
        yield return new WaitForSeconds(0.8f);
        knockbackForce = savedForce;
        sprite.color = Color.white;

        isOnTrait = false;
    }

    IEnumerator Jump()
    {
        float direction;

        isOnTrait = true;
        rigid.linearVelocity = Vector2.zero;
        sprite.flipX = (target.position.x >= transform.position.x) ? false : true;

        anim.SetTrigger("Idle");
        yield return new WaitForSeconds(0.4f);
        anim.SetTrigger("Attack");
        direction = target.position.x >= transform.position.x ? 1f : -1f;
        //(김택림) speedFactor 곱셈 추가
        if (!isDead)
        {
            SoundManager.inst.PlaySFX(audioClips[0]);
            rigid.linearVelocity = new Vector2(direction * 6f * speedFactor, 25);
        }
        yield return new WaitForSeconds(0.8f);

        isOnTrait = false;
    }

    IEnumerator RushExplode()
    {
        Vector2 dir;
        float curTime = 0f, maxTime = 1.25f;

        SoundManager.inst.PlaySFX(audioClips[0]);

        isOnTrait = true;
        sprite.flipX = (target.position.x >= transform.position.x) ? false : true;
        anim.SetTrigger("Attack");

        dir = (target.position - transform.position).normalized;
        //(김택림) speedFactor 곱셈 추가
        while (curTime < maxTime)
        {
            if (isDead || isCollided) break;
            if (!isKnockedBack) rigid.linearVelocity = dir * 8 * speedFactor;

            curTime += Time.fixedDeltaTime;
            sprite.color = Color.Lerp(Color.white, Color.red, curTime / maxTime);
            yield return new WaitForFixedUpdate();
        }
        if (!isDead)
        {
            SoundManager.inst.PlaySFX(audioClips[1]);

            Transform explosion = GameManager.inst.pool.GetRange(5).transform;
            explosion.localPosition = transform.position + new Vector3(0, 0.5f, 0);
            explosion.localRotation = Quaternion.identity;
            gameObject.SetActive(false);
        }
        isOnTrait = false;
    }

    //(김택림) 적 느리게 하기
    public void ApplySlow(float amount, float duration)
    {
        slowAmount = amount;
        slowTimer = duration;
        isSlowed = true;
    }

    void Adjust()
    {
        adjustPositionTime += Time.fixedDeltaTime;
        if (adjustPositionTime > 1f)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + 0.01f);
            adjustPositionTime = 0f;
        }
    }
}