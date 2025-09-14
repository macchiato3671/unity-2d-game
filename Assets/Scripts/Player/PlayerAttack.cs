using System.Collections;
using UnityEditor;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    Player player;
    SpriteRenderer sprite;
    Rigidbody2D rigid;
    Animator anim;
    LineRenderer lineRenderer;
    [SerializeField] private GameObject hitbox;
    public GameObject checkPlayer;
    PlayerSkill playerSkill;
    PlayerMove playerMove;
    HackModeManager hackModeManager;
    public WeaponData[] equippedWeapons = new WeaponData[2]; // 무기 2개 장착
    public int currentWeaponIndex = 0;
    public WeaponData CurrentWeapon => equippedWeapons[currentWeaponIndex];

    bool onAttack; // 근접 공격이 작동중일시 true
    bool canShot; // 원거리 공격 가능시 true
    bool canParry = true; // parry 쿨타임
    bool canAvoid = true; // avoid 쿨타임
    public bool isParrying = false; // 패링 중인지 확인
    public LayerMask enemyLayer;

    public Transform weaponPoint;
    public SpriteRenderer weaponSpriteRenderer;
    public Sprite parrySprite;
    public AudioClip avoidAudio;

    public PlayerHealth health;

    void Awake()
    {
        player = GetComponent<Player>();
        sprite = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerSkill = GetComponent<PlayerSkill>();
        playerMove = GetComponent<PlayerMove>();
        hackModeManager = GetComponent<HackModeManager>();
        onAttack = false;
        canShot = true;

        // LineRenderer 컴포넌트를 가져오거나 추가
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // LineRenderer 기본 설정
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // 기본 흰색 라인
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    void Start()
    {
        UpdateEquippedWeapon();
    }

    void Update()
    {
        if (GameManager.inst.IsPaused || GameManager.inst.IsOnInventory) return;

        if (health != null && health.CurrentHP <= 0) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerSkill.isAfterImageBuff = false;
            playerSkill.isTrackingBuff = false;
            currentWeaponIndex = 0;
            GameManager.inst.currentWeaponIndex = currentWeaponIndex;
            GameManager.inst.screenUI.SetWeaponSelectUI(currentWeaponIndex); // 무기 UI 처리
            GameManager.inst.screenUI.SetHackUI(currentWeaponIndex);
            Debug.Log("무기 1번 장착: " + CurrentWeapon.weaponName);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            playerSkill.isAfterImageBuff = false;
            playerSkill.isTrackingBuff = false;
            currentWeaponIndex = 1;
            GameManager.inst.currentWeaponIndex = currentWeaponIndex;
            GameManager.inst.screenUI.SetWeaponSelectUI(currentWeaponIndex); // 무기 UI 처리
            GameManager.inst.screenUI.SetHackUI(currentWeaponIndex);
            Debug.Log("무기 2번 장착: " + CurrentWeapon.weaponName);
        }

        // 마우스 좌클릭에 따라 플레이어의 애니메이션 설정 / 우클릭에 따라 원거리 공격
        // if(Input.GetMouseButtonDown(0) && AttackType == 1) {
        //     player.State = Player.PlayerState.Attack;
        //     anim.SetTrigger("Attack1");
        //     if(!onAttack) {
        //         //StartCoroutine(ActivateHitbox());
        //         StartCoroutine(Attack());
        //     }
        // }
        // else if(Input.GetMouseButtonDown(0) && canShot && AttackType == 2){
        //     StartCoroutine(Shot());
        // }
        // else if(Input.GetMouseButtonUp(0)){
        //     player.State = Player.PlayerState.Default;
        // }

        //if (hackModeManager.IsHackMode) return; // 해킹 모드에 들어갔을때에는 해킹만 사용할 수 있게 했습니다!!

        // 마우스 좌클릭 - 기본 공격
        if (Input.GetMouseButtonDown(0))
        {
            player.State = Player.PlayerState.Attack;
            switch (CurrentWeapon.type)
            {
                case WeaponType.Melee:
                    anim.SetInteger("weaponID", 2);
                    anim.SetTrigger("Attack");

                    if (!onAttack)
                        StartCoroutine(Attack());
                    break;

                case WeaponType.Throwing:
                    anim.SetInteger("weaponID", 3);
                    anim.SetTrigger("Attack");
                    if (canShot)
                        StartCoroutine(Shot());
                    break;

                case WeaponType.Laser:
                    anim.SetInteger("weaponID", 1);
                    anim.SetTrigger("Attack");
                    if (canShot)
                    {
                        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        Vector2 dir = (mousePos - transform.position).normalized;
                        StartCoroutine(LaserShot(dir));
                    }
                    break;

                case WeaponType.Gravity:
                    anim.SetInteger("weaponID", 0);
                    anim.SetTrigger("Attack");
                    if (canShot)
                    {
                        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        mousePos.z = 0f;
                        Vector2 dir = (mousePos - transform.position).normalized;
                        StartCoroutine(GravShot(dir));
                    }
                    break;
            }
        }

        // 마우스 우클릭 - 회피 or 패링
        if (Input.GetMouseButtonDown(1))
        {
            switch (CurrentWeapon.type)
            {
                case WeaponType.Melee:
                    if (!isParrying && canParry)
                    {
                        canParry = false;
                        StartCoroutine(ParryCoroutine());
                    }
                    break;

                default:
                    if (canAvoid)
                    {
                        canAvoid = false;
                        StartCoroutine(AvoidCoroutine());
                    }
                    break;
            }
        }

        // Q - 스킬 1
        if (Input.GetKeyDown(KeyCode.Q))
        {
            switch (CurrentWeapon.type)
            {
                case WeaponType.Melee: playerSkill.MeleeQSkill(CurrentWeapon); break;
                case WeaponType.Throwing: playerSkill.RangeQSkill(CurrentWeapon); break;
                case WeaponType.Laser: playerSkill.LaserQSkill(CurrentWeapon); break;
                case WeaponType.Gravity: playerSkill.GravQSkill(CurrentWeapon); break;
            }
        }

        // E - 스킬 2
        if (Input.GetKeyDown(KeyCode.E))
        {
            switch (CurrentWeapon.type)
            {
                case WeaponType.Melee: playerSkill.MeleeESkill(CurrentWeapon); break;
                case WeaponType.Throwing: playerSkill.RangeESkill(CurrentWeapon); break;
                case WeaponType.Laser: playerSkill.LaserESkill(CurrentWeapon); break;
                case WeaponType.Gravity: playerSkill.GravESkill(CurrentWeapon); break;
            }
        }
    }

    // void LateUpdate()
    // {
    //     // 공격중에만, 마우스 위치 계산을 통한 플레이어의 바라보는 방향 처리
    //     if(player.State == Player.PlayerState.Attack){
    //         Vector3 mouseVec = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
    //         if(mouseVec.x < 0){
    //             sprite.flipX = true;
    //             hitbox.transform.localPosition = new Vector2(-1.5f, 0);
    //         }
    //         else{
    //             sprite.flipX = false;
    //             hitbox.transform.localPosition = new Vector2(1.5f, 0);
    //         }
    //     }
    // }

    private void LateUpdate()
    {
        bool isAttacking = (player.State == Player.PlayerState.Attack);
        // 무기 표시 여부
        weaponSpriteRenderer.enabled = isAttacking;
        if (!isAttacking) return;
        if (isAttacking)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;

            // 플레이어 기준으로 마우스 좌/우 판별
            bool facingLeft = (mouseWorld.x < transform.position.x);

            // flipX 설정
            playerMove.spriteRenderer.flipX = facingLeft;
            weaponSpriteRenderer.flipX = facingLeft;

            // WeaponPoint 위치 반전
            if (facingLeft)
            {
                Vector3 wp = weaponPoint.position;

                if (CurrentWeapon.weaponID == 0 || CurrentWeapon.weaponID == 1)
                {
                    wp.x = 2f * transform.position.x - wp.x - 0.14f;
                }
                else
                {
                    wp.x = 2f * transform.position.x - wp.x - 0.3f;
                }
                weaponPoint.position = wp;
            }

            RotateWeaponTowardMouse(); // 위에서 만들었던 함수
        }


        if (weaponSpriteRenderer.sprite != CurrentWeapon.icon)
        {
            weaponSpriteRenderer.sprite = CurrentWeapon.icon;
        }
    }

    private void RotateWeaponTowardMouse()
    {
        // 1. 마우스 월드 좌표 구하기
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        // 2. WeaponPoint 기준 마우스 방향
        Vector3 dir = mouseWorld - weaponPoint.position;

        // 3. 각도 계산 (라디안 → 도)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // 4. 디폴트가 반시계 45도 회전이므로 → 시계방향으로 45도 보정
        angle -= 45f;

        //flipX 상태면 방향 벡터를 X축 기준 좌우 반전
        if (playerMove.spriteRenderer.flipX)
        {
            angle = 90f - angle;
            angle *= -1f;
        }

        // 5. 회전 적용 (Z축 회전)
        weaponPoint.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    //Hitbox 구현
    IEnumerator Attack()
    {
        Vector2 dir;

        onAttack = true;
        enemyLayer = LayerMask.GetMask("Enemy");

        // 매 공격마다 현재 무기에서 최신 정보 가져오기
        WeaponData weapon = CurrentWeapon;
        MeleeWeaponData meleeWeapon = (MeleeWeaponData)weapon;

        int damage = meleeWeapon.baseDamage;
        float width = meleeWeapon.hitboxWidth;
        float height = meleeWeapon.hitboxHeight;
        if (weapon.accessoryData1 is MeleeNormalAccessory acc1)
        {
            damage = Mathf.RoundToInt(damage * acc1.damageMult);
            width *= acc1.hitboxWidthMult;
            height *= acc1.hitboxHeightMult;
        }
        if (weapon.accessoryData2 is MeleeNormalAccessory acc2)
        {
            damage = Mathf.RoundToInt(damage * acc2.damageMult);
            width *= acc2.hitboxWidthMult;
            height *= acc2.hitboxHeightMult;
        }

        // hitbox 범위 내의 모든 적 검색
        Vector2 hitboxPosition = checkPlayer.transform.position;
        Vector3 mouseVec = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float slashX = 0;
        if (mouseVec.x < 0)
        {
            sprite.flipX = true;
            hitboxPosition += new Vector2(-width / 2f, 0.4f);
            slashX = -width / 2f;
        }
        else
        {
            sprite.flipX = false;
            hitboxPosition += new Vector2(width / 2f, 0.4f);
            slashX = width / 2f;
        }

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(
            hitboxPosition,
            new Vector2(width, height),
            0f,
            enemyLayer
        );

        bool enemyHit = false;
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (enemyCollider.CompareTag("Enemy"))
            {
                enemyHit = true;
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    dir = (enemy.transform.position - transform.position).normalized;
                    enemy.Damaged(damage, dir);
                    Debug.Log("적을 공격함! 데미지: " + damage);
                }
            }
            else if (enemyCollider.CompareTag("BreakableBox"))
            {
                enemyCollider.GetComponent<BreakableBox>().Damaged(1);
            }
        }
        if (enemyHit)
        {
            //GameManager.inst.combatEffectManager.DoHitStop(hitStopTime);
            GameManager.inst.combatEffectManager.EasedHitStop();
        }

        if (meleeWeapon.attackSFX == null)
        {
            Debug.LogWarning("attackSFX is null!");
        }
        else
        {
            Debug.Log($"attackSFX name: {meleeWeapon.attackSFX.name}");
            SoundManager.inst.PlaySFX(meleeWeapon.attackSFX, 2);
        }

        dir = sprite.flipX ? Vector2.left : Vector2.right;
        if (playerSkill.isAfterImageBuff)
        {
            SpawnAfterImageSlash(dir);
        }

        yield return new WaitForSeconds(0.15f);
        GameObject slash = GameManager.inst.pool.GetEffect(0);
        slash.transform.parent = transform;
        slash.transform.localPosition = Vector3.zero;
        slash.transform.localPosition += new Vector3(slashX, 0.4f,0);
        slash.transform.rotation = Quaternion.identity;

        SpriteRenderer sr = slash.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            slash.transform.localScale = Vector3.one; // 누적 방지

            Vector2 baseSize = sr.sprite.bounds.size;
            float scaleX = width / baseSize.x;
            float scaleY = height / baseSize.y;

            slash.transform.localScale = new Vector3(scaleX, scaleY, 1f);
            sr.flipX = dir.x > 0 ? false : true;
            if (dir.x > 0) slash.transform.position += new Vector3(0.5f, 0f, 0);
            else slash.transform.position += new Vector3(-0.5f, 0f, 0);
        }

        yield return new WaitForSeconds(weapon.attackSpeed);
        player.State = Player.PlayerState.Default;
        onAttack = false;
    }

    void SpawnAfterImageSlash(Vector2 dir)
    {
        // SkillData 구조체에서 필요한 data 가져오기(추후에 장신구 효과 곱하기)
        if (CurrentWeapon.type == WeaponType.Melee)
        {
            AfterImageData afterImage = (AfterImageData)CurrentWeapon.eSkillData;

            int damage = afterImage.damage;
            float speed = afterImage.speed;
            float maxDistance = afterImage.maxDistance;

            if (CurrentWeapon.accessoryData1 is MeleeEAccessory acc1)
            {
                damage = Mathf.RoundToInt(damage * acc1.damageMult);
                speed *= acc1.speedMult;
                maxDistance *= acc1.maxDistanceMult;
            }
            if (CurrentWeapon.accessoryData2 is MeleeEAccessory acc2)
            {
                damage = Mathf.RoundToInt(damage * acc2.damageMult);
                speed *= acc2.speedMult;
                maxDistance *= acc2.maxDistanceMult;
            }

            // Melee E스킬 실행 코드
            GameObject slash = GameManager.inst.pool.GetRange(7); // 잔상 프리팹
            slash.transform.position = transform.position;
            slash.transform.rotation = Quaternion.identity;
            slash.GetComponent<AfterImageSlash>().SetStats(dir, damage, speed, maxDistance);
        }
    }

    // hitbox gizmo 생성
    private void OnDrawGizmos()
    {
        Vector2 hitboxPosition = checkPlayer.transform.position;
        Vector3 mouseVec = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        if (mouseVec.x < 0)
        {
            hitboxPosition += new Vector2(-1.5f, 0.4f);
        }
        else
        {
            hitboxPosition += new Vector2(1.5f, 0.4f);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(hitboxPosition, new Vector3(3.0f, 2.0f, 1f));
    }

    // 패링, 회피 쿨타임
    IEnumerator ParryAvoidCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        canAvoid = true;
        canParry = true;
    }

    //패링
    IEnumerator ParryCoroutine()
    {
        isParrying = true;
        //player.State = Player.PlayerState.Attack;
        anim.enabled = false;
        playerMove.spriteRenderer.sprite = parrySprite;
        Debug.Log("패링!");

        yield return new WaitForSeconds(1.0f);

        // 애니메이터 다시 활성화 → 자동으로 idle 애니메이션으로 복귀됨
        anim.enabled = true;
        isParrying = false; ;

        player.State = Player.PlayerState.Default;
        Debug.Log("패링 종료");
        StartCoroutine(ParryAvoidCoroutine());
    }

    IEnumerator Shot()
    {
        // 무기 스탯 가져오기
        WeaponData weapon = CurrentWeapon;
        ThrowingWeaponData throwingWeapon = (ThrowingWeaponData)weapon;

        int activeCount = GameManager.inst.pool.GetActiveCount(0);
        int maxCount = throwingWeapon.maxKnifeCount;
        if (weapon.accessoryData1 is ThrowingNormalAccessory acc1)
        {
            maxCount += acc1.maxKnifeCountAdd;
        }
        if (weapon.accessoryData2 is ThrowingNormalAccessory acc2)
        {
            maxCount += acc2.maxKnifeCountAdd;
        }

        if (activeCount >= maxCount)
        {
            Debug.Log("No More Bullet");
            yield break;
        }

        // PoolManager를 통해 투사체를 생성하고, 위치, 회전, 공격 방향 설정 후 공격 쿨타임 적용
        GameObject projGO = GameManager.inst.pool.GetRange(0);
        if (projGO == null)
        {
            yield break;
        }
        Transform proj = projGO.transform;
        Range projComp = proj.GetComponent<Range>();
        Vector3 mouseVec = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        mouseVec.z = 0;

        proj.localPosition = transform.position;
        proj.localRotation = Quaternion.identity;
        proj.rotation = Quaternion.FromToRotation(Vector3.up, mouseVec);
        proj.Rotate(new Vector3(0f, 0f, 45f));
        proj.Translate(mouseVec.normalized * 1f, Space.World);

        projComp.Init(mouseVec.normalized, weapon);
        if (throwingWeapon.attackSFX == null)
        {
            Debug.LogWarning("attackSFX is null!");
        }
        else
        {
            Debug.Log($"attackSFX name: {throwingWeapon.attackSFX.name}");
            SoundManager.inst.PlaySFX(throwingWeapon.attackSFX, 3);
        }

        canShot = false;
        StartCoroutine(ShotCoolDown(weapon.attackSpeed));
        yield return null;
    }

    IEnumerator ShotCoolDown(float attackSpeed)
    {
        yield return new WaitForSeconds(attackSpeed);
        player.State = Player.PlayerState.Default;
        canShot = true;
    }

    //원거리 회피
    IEnumerator AvoidCoroutine()
    {
        Debug.Log("회피 트리거 실행됨");
        playerMove.avoid = true;
        anim.SetTrigger("Avoid");
        gameObject.layer = 8;
        playerMove.isInvincible = true;

        // if (spriteRenderer.flipX)
        //     rigid.AddForce(new Vector2(-1.0f, -1.0f) * max_speed, ForceMode2D.Impulse);
        // else
        //     rigid.AddForce(new Vector2(1.0f, -1.0f) * max_speed, ForceMode2D.Impulse);

        float dashSpeed = 15f;  // 회피 속도 (원하는 만큼 조절)

        // 회피 방향 결정
        int dir = sprite.flipX ? -1 : 1;
        rigid.linearVelocity = new Vector2(dir * dashSpeed, -dashSpeed * 0.7f); // 대각선 회피 느낌

        if (avoidAudio == null)
        {
            Debug.LogWarning("attackSFX is null!");
        }
        else
        {
            Debug.Log($"attackSFX name: {avoidAudio.name}");
            SoundManager.inst.PlaySFX(avoidAudio, 2);
        }

        //Avoid time
        yield return new WaitForSeconds(0.4f);

        playerMove.StopKnockBack();
        playerMove.OffDamaged();

        playerMove.avoid = false;

        StartCoroutine(ParryAvoidCoroutine());
    }

    IEnumerator LaserShot(Vector3 direction)
    {
        WeaponData weapon = CurrentWeapon;
        LaserWeaponData laserWeapon = (LaserWeaponData)weapon;
        LaserTrackingData laserTracking = (LaserTrackingData)weapon.qSkillData;
        LaserBuffData laserBuff = (LaserBuffData)weapon.eSkillData;

        // laser 기본 공격 장신구 효과 적용
        int final_damage = laserWeapon.baseDamage;
        float final_length = laserWeapon.laserLength;
        float final_lifetime = laserWeapon.lifeTime;
        if (weapon.accessoryData1 is LaserNormalAccessory normalAcc1)
        {
            final_damage = Mathf.RoundToInt(final_damage * normalAcc1.damageMult);
            final_length *= normalAcc1.laserLengthMult;
            final_lifetime *= normalAcc1.lifeTimeMult;
        }
        if (weapon.accessoryData2 is LaserNormalAccessory normalAcc2)
        {
            final_damage = Mathf.RoundToInt(final_damage * normalAcc2.damageMult);
            final_length *= normalAcc2.laserLengthMult;
            final_lifetime *= normalAcc2.lifeTimeMult;
        }

        // laser q스킬 장신구 효과 적용
        float final_trackingRange = laserTracking.trackingRange;
        int final_remainingBounces = laserTracking.trackEnemyNum;
        if (weapon.accessoryData1 is LaserQAccessory Qacc1)
        {
            final_trackingRange *= Qacc1.trackingRangeMult;
            final_remainingBounces += Qacc1.trackEnemyNumAdd;
        }
        if (weapon.accessoryData2 is LaserQAccessory Qacc2)
        {
            final_trackingRange *= Qacc2.trackingRangeMult;
            final_remainingBounces += Qacc2.trackEnemyNumAdd;
        }

        GameObject laserGO = GameManager.inst.pool.GetRange(2);
        if (laserGO == null) yield break;

        Vector3 offset = transform.TransformVector(weaponPoint.localPosition);
        Vector3 finalPos = transform.position + offset;
        laserGO.transform.position = finalPos;
        laserGO.transform.rotation = Quaternion.identity;

        Laser laserComp = laserGO.GetComponent<Laser>();
        if (laserComp == null) yield break;
        laserComp.followPlayer = true;
        laserComp.playerRef = this.transform;
        laserComp.weaponPoint = weaponPoint;

        if (playerSkill.isTrackingBuff)
        {
            Debug.Log("Track!");
            laserComp.SetStats(direction, final_damage, final_length, final_lifetime, final_trackingRange, final_remainingBounces);
        }
        else
        {
            laserComp.SetStats(direction, final_damage, final_length, final_lifetime, 0, 0);
        }

        canShot = false;

        // laser e스킬 장신구 효과 적용
        float attackSpeedRate = 1f;
        if (playerSkill.isLaserCooldownBuff)
        {
            attackSpeedRate = laserBuff.attackSpeedRate;

            if (weapon.accessoryData1 is LaserEAccessory Eacc1)
            {
                attackSpeedRate *= Eacc1.attackSpeedRateMult;
            }
            if (weapon.accessoryData2 is LaserEAccessory Eacc2)
            {
                attackSpeedRate *= Eacc2.attackSpeedRateMult;
            }
        }
        StartCoroutine(LaserCoolDown(weapon.attackSpeed * attackSpeedRate));
        yield return null;
    }

    IEnumerator LaserCoolDown(float attackSpeed)
    {
        yield return new WaitForSeconds(attackSpeed);
        player.State = Player.PlayerState.Default;
        canShot = true;
    }

    IEnumerator GravShot(Vector3 direction)
    {
        yield return new WaitForEndOfFrame();
        WeaponData weapon = CurrentWeapon;
        GravityWeaponData gravityWeapon = (GravityWeaponData)weapon;
        float final_pullRadius = gravityWeapon.pullRadius;
        float final_pullForce = gravityWeapon.pullForce;
        float final_duration = gravityWeapon.duration;
        float final_maxLifetime = gravityWeapon.maxLifetime;
        float final_tickInterval = gravityWeapon.tickInterval;
        float final_totalTickDamage = gravityWeapon.totalTickDamage;

        if (weapon.accessoryData1 is GravityNormalAccessory acc1)
        {
            final_pullRadius *= acc1.pullRadiusMult;
            final_pullForce *= acc1.pullForceMult;
            final_duration *= acc1.durationMult;
            final_totalTickDamage *= acc1.totalTickDamageMult;
        }
        if (weapon.accessoryData2 is GravityNormalAccessory acc2)
        {
            final_pullRadius *= acc2.pullRadiusMult;
            final_pullForce *= acc2.pullForceMult;
            final_duration *= acc2.durationMult;
            final_totalTickDamage *= acc2.totalTickDamageMult;
        }

        GameObject bulletGO = GameManager.inst.pool.GetRange(3); // 중력자탄
        Vector3 finalPos = weaponPoint.position;
        if (weaponPoint.localPosition.x < 0f)
        {
            float yOffset = 0.1f;
            finalPos += Vector3.up * yOffset;
        }
        bulletGO.transform.position = finalPos;
        Debug.Log($"[중력자탄] 최종 발사 위치: {finalPos}, weaponPoint pos: {weaponPoint.position}, local: {weaponPoint.localPosition}");
        // bulletGO.transform.position = transform.position;

        GravBullet gravity = bulletGO.GetComponent<GravBullet>();
        gravity.SetStats(
            direction,
            final_pullRadius,
            final_pullForce,
            final_duration,
            final_maxLifetime,
            final_tickInterval,
            final_totalTickDamage
        );
        if (gravityWeapon.attackSFX == null)
        {
            Debug.LogWarning("attackSFX is null!");
        }
        else
        {
            Debug.Log($"attackSFX name: {gravityWeapon.attackSFX.name}");
            SoundManager.inst.PlaySFX(gravityWeapon.attackSFX, 0.8f);
        }

        canShot = false;
        StartCoroutine(GravCoolDown(weapon.attackSpeed));
        yield return null;
    }

    IEnumerator GravCoolDown(float attackSpeed)
    {
        yield return new WaitForSeconds(attackSpeed);
        player.State = Player.PlayerState.Default;
        canShot = true;
    }

    /*
        IEnumerator ActivateHitbox()
        {
            onAttack = true;
            hitbox.SetActive(true);
            yield return new WaitForSeconds(0.6f);
            hitbox.SetActive(false);
            onAttack = false;
        }
    */
    public void UpdateEquippedWeapon()
    {
        currentWeaponIndex = 0;
        for (int i = 0; i < 2; i++)
        {
            if (GameManager.inst.equippedWeapons[i] != null)
            {
                equippedWeapons[i] = GameManager.inst.equippedWeapons[i];
                Debug.Log($"[장착됨] 무기 슬롯 {i + 1}: {equippedWeapons[i].weaponName}");
            }
            else
            {
                Debug.LogWarning($"무기 {i}번 슬롯이 비어있습니다!");
            }
        }
        GameManager.inst.screenUI.SetHackUI(currentWeaponIndex);
    }
}