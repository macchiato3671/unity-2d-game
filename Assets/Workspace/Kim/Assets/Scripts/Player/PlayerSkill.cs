using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSkill : MonoBehaviour
{
    public LayerMask enemyLayer;
    public PlayerAttack playerAttack;
    public PlayerMove playerMove;
    SpriteRenderer spriteRenderer;

    // 스킬들 쿨타임 돌았는지 확인용
    private Dictionary<string, bool> skillCooldowns = new Dictionary<string, bool>();

    public bool isAfterImageBuff = false;
    public bool isTrackingBuff = false;
    public bool isLaserCooldownBuff = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerAttack = GetComponent<PlayerAttack>();
        playerMove = GetComponent<PlayerMove>();

        skillCooldowns["MeleeQ"] = false;
        skillCooldowns["MeleeE"] = false;
        skillCooldowns["RangeQ"] = false;
        skillCooldowns["RangeE"] = false;
        skillCooldowns["LaserQ"] = false;
        skillCooldowns["LaserE"] = false;
        skillCooldowns["GravQ"] = false;
        skillCooldowns["GravE"] = false;
    }

    //근접 Q스킬(emp파 날리는 스킬)
    public void MeleeQSkill(WeaponData weapon)
    {
        if (skillCooldowns["MeleeQ"]) return; // 쿨타임이면 사용불가
        skillCooldowns["MeleeQ"] = true;
        GameManager.inst.screenUI.UpdateHackUI(0);

        // SkillData 구조체에서 필요한 data 가져오기(추후에 장신구 효과 곱하기)
        EMPSlashData empslash = (EMPSlashData)weapon.qSkillData;
        int damage = empslash.damage;
        float slowDuration = empslash.slowDuration;
        float slowAmount = empslash.slowAmount;
        float maxDistance = empslash.maxDistance;

        if (weapon.accessoryData1 is MeleeQAccessory acc1) {
            damage = Mathf.RoundToInt(damage * acc1.damageMult);
            slowDuration *= acc1.slowDurationMult;
            slowAmount *= acc1.slowAmountMult;
            maxDistance *= acc1.maxDistanceMult;
        }
        if (weapon.accessoryData2 is MeleeQAccessory acc2) {
            damage = Mathf.RoundToInt(damage * acc2.damageMult);
            slowDuration *= acc2.slowDurationMult;
            slowAmount *= acc2.slowAmountMult;
            maxDistance *= acc2.maxDistanceMult;
        }

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mouseWorld - transform.position);

        GameObject slash = GameManager.inst.pool.GetRange(6);
        slash.transform.position = transform.position;
        slash.transform.rotation = Quaternion.identity;

        slash.GetComponent<EMPSlash>().SetStats(dir, damage, slowDuration, slowAmount, maxDistance);
        if (empslash.attackSFX == null)
        {
            Debug.LogWarning("attackSFX is null!");
        }
        else
        {
            Debug.Log($"attackSFX name: {empslash.attackSFX.name}");
            SoundManager.inst.PlaySFX(empslash.attackSFX, 1);
        }

        StartCoroutine(ResetCooldown("MeleeQ", empslash.cooldown));
    }

    //근접 E스킬(추가 공격하는 분신 생성)
    public void MeleeESkill(WeaponData weapon)
    {
        if (skillCooldowns["MeleeE"]) return; // 쿨타임이면 사용불가
        skillCooldowns["MeleeE"] = true;
        GameManager.inst.screenUI.UpdateHackUI(1);

        StartCoroutine(PerformESkill(weapon));
    }

    IEnumerator PerformESkill(WeaponData weapon)
    {
        AfterImageData afterImage = (AfterImageData)weapon.eSkillData;
        StartCoroutine(ResetCooldown("MeleeE", afterImage.cooldown));

        isAfterImageBuff = true;
        float buffDuration = afterImage.buffDuration;
        Debug.Log("잔상 공격 시작");
        if (weapon.accessoryData1 is MeleeEAccessory acc1) {
            buffDuration *= acc1.buffDurationMult;
        }
        if (weapon.accessoryData2 is MeleeEAccessory acc2) {
            buffDuration *= acc2.buffDurationMult;
        }

        if (afterImage.buffSFX == null)
        {
            Debug.LogWarning("attackSFX is null!");
        }
        else
        {
            Debug.Log($"attackSFX name: {afterImage.buffSFX.name}");
            SoundManager.inst.PlaySFX(afterImage.buffSFX, 4);
        }

        yield return new WaitForSeconds(buffDuration);

        isAfterImageBuff = false;
        Debug.Log("잔상 공격 종료");
    }

    //원거리 Q스킬(부채꼴 모양으로 투척)
    public void RangeQSkill(WeaponData weapon)
    {
        if (skillCooldowns["RangeQ"]) return; // 쿨타임이면 사용불가
        skillCooldowns["RangeQ"] = true;
        GameManager.inst.screenUI.UpdateHackUI(0);

        List<GameObject> poolList = GameManager.inst.pool.rangePools[0];
        ThrowingWeaponData throwingWeapon = (ThrowingWeaponData)weapon;
        BladeThrowData bladeThrow = (BladeThrowData)weapon.qSkillData;

        // Active 되어있는 bullet수
        int activeCount = 0;
        foreach (var b in poolList)
            if (b.activeSelf) activeCount++;

        int MaxCount = throwingWeapon.maxKnifeCount;
        if (throwingWeapon.accessoryData1 is ThrowingNormalAccessory acc1)
        {
            MaxCount += acc1.maxKnifeCountAdd;
        }
        if (throwingWeapon.accessoryData2 is ThrowingNormalAccessory acc2)
        {
            MaxCount += acc2.maxKnifeCountAdd;
        }

        int availableBullets = MaxCount - activeCount;
        if (availableBullets <= 0) return;

        float spreadAngle = 30f;
        float startAngle = -spreadAngle / 2f;
        float angleStep;

        if (availableBullets == 1)
            angleStep = 0f;
        else
            angleStep = spreadAngle / (availableBullets - 1);

        float baseAngle = spriteRenderer.flipX ? 180f : 0f;

        for (int i = 0; i < availableBullets; i++)
        {
            float angle = baseAngle + startAngle + angleStep * i;
            Vector2 direction = AngleToDirection(angle);

            GameObject bullet = GameManager.inst.pool.GetRange(0);
            if (bullet == null) continue;

            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
            bullet.transform.Rotate(new Vector3(0f, 0f, -45f));

            Range range = bullet.GetComponent<Range>();
            if (range != null)
            {
                range.Init(direction, weapon);
            }
        }

        if (bladeThrow.attackSFX == null)
        {
            Debug.LogWarning("attackSFX is null!");
        }
        else
        {
            Debug.Log($"attackSFX name: {bladeThrow.attackSFX.name}");
            SoundManager.inst.PlaySFX(bladeThrow.attackSFX, 4);
        }
        StartCoroutine(ResetCooldown("RangeQ", bladeThrow.cooldown));
    }

    //원거리 E스킬(칼 회수)
    public void RangeESkill(WeaponData weapon)
    {
        bool recalled = false;
        ThrowingWeaponData throwingWeapon = (ThrowingWeaponData)weapon;
        RecallBladeData recallBlade = (RecallBladeData)weapon.eSkillData;
        GameManager.inst.screenUI.UpdateHackUI(1);

        foreach (GameObject bullet in GameManager.inst.pool.rangePools[0])
        {
            if (bullet.activeSelf)
            {
                Range range = bullet.GetComponent<Range>();
                if (range != null)
                {
                    range.RecallToPlayer(GameManager.inst.player.transform);
                    recalled = true;
                }
            }
        }
        if(recalled)
        {
            Debug.Log($"attackSFX name: {recallBlade.attackSFX.name}");
            SoundManager.inst.PlaySFX(recallBlade.attackSFX, 5);
        }
    }

    //레이저 Q스킬(레이저 추적)
    public void LaserQSkill(WeaponData weapon)
    {
        if (skillCooldowns["LaserQ"]) return; // 쿨타임이면 사용불가
        skillCooldowns["LaserQ"] = true;
        GameManager.inst.screenUI.UpdateHackUI(0);
        StartCoroutine(PerformQSkill(weapon));
    }

    IEnumerator PerformQSkill(WeaponData weapon)
    {
        LaserTrackingData laserTrack = (LaserTrackingData)weapon.qSkillData;
        StartCoroutine(ResetCooldown("LaserQ", laserTrack.cooldown));
        isTrackingBuff = true;
        Debug.Log("레이저 추적 시작");

        float duration = laserTrack.buffDuration;
        if (weapon.accessoryData1 is LaserQAccessory acc1)
        {
            duration *= acc1.buffDurationMult;
        }
        if (weapon.accessoryData2 is LaserQAccessory acc2)
        {
            duration *= acc2.buffDurationMult;
        }

        if (laserTrack.attackSFX == null)
        {
            Debug.LogWarning("attackSFX is null!");
        }
        else
        {
            Debug.Log($"attackSFX name: {laserTrack.attackSFX.name}");
            SoundManager.inst.PlaySFX(laserTrack.attackSFX);
        }

        yield return new WaitForSeconds(duration);

        isTrackingBuff = false;
        Debug.Log("레이저 추적 종료");
    }

    //레이저 E스킬(공이속 증가)
    public void LaserESkill(WeaponData weapon)
    {
        if (skillCooldowns["LaserE"]) return; // 쿨타임이면 사용불가
        skillCooldowns["LaserE"] = true;
        GameManager.inst.screenUI.UpdateHackUI(1);
        StartCoroutine(LaserEActivated(weapon));
    }

    IEnumerator LaserEActivated(WeaponData weapon)
    {
        LaserBuffData laserBuff = (LaserBuffData)weapon.eSkillData;

        float duration = laserBuff.buffDuration;
        if (weapon.accessoryData1 is LaserEAccessory acc1)
        {
            duration *= acc1.buffDurationMult;
        }
        if (weapon.accessoryData2 is LaserEAccessory acc2)
        {
            duration *= acc2.buffDurationMult;
        }

        if (laserBuff.attackSFX == null)
        {
            Debug.LogWarning("attackSFX is null!");
        }
        else
        {
            Debug.Log($"attackSFX name: {laserBuff.attackSFX.name}");
            SoundManager.inst.PlaySFX(laserBuff.attackSFX);
        }

        StartCoroutine(ResetCooldown("LaserE", laserBuff.cooldown));
        isLaserCooldownBuff = true;

        yield return new WaitForSeconds(duration);
        isLaserCooldownBuff = false;
    }

    //중력자탄 Q스킬(중력자탄 폭발)
    public void GravQSkill(WeaponData weapon)
    {
        if (skillCooldowns["GravQ"]) return; // 쿨타임이면 사용불가
        List<GameObject> activeGravBullets = GameManager.inst.pool.GetAllActiveInPool(3);
        GravExplosionData gravExplosion = (GravExplosionData)weapon.qSkillData;
        bool GravQUsed = false;

        int final_explosionDamage = gravExplosion.explosionDamage;
        float final_explosionRadius = gravExplosion.explosionRadius;
        if (weapon.accessoryData1 is GravityQAccessory acc1)
        {
            final_explosionDamage = Mathf.RoundToInt(final_explosionDamage * acc1.explosionDamageMult);
            final_explosionRadius *= acc1.explosionRadiusMult;
        }
        if (weapon.accessoryData2 is GravityQAccessory acc2)
        {
            final_explosionDamage = Mathf.RoundToInt(final_explosionDamage * acc2.explosionDamageMult);
            final_explosionRadius *= acc2.explosionRadiusMult;
        }

        foreach (GameObject bullet in activeGravBullets)
        {
            GravBullet gb = bullet.GetComponent<GravBullet>();
            if (gb != null)
            {
                GravQUsed = true;
                gb.Explode(final_explosionDamage, final_explosionRadius);
            }
        }

        if (GravQUsed)
        {
            skillCooldowns["GravQ"] = true;
            GameManager.inst.screenUI.UpdateHackUI(0);

            if (gravExplosion.attackSFX == null)
            {
                Debug.LogWarning("attackSFX is null!");
            }
            else
            {
                Debug.Log($"attackSFX name: {gravExplosion.attackSFX.name}");
                SoundManager.inst.PlaySFX(gravExplosion.attackSFX, 2);
            }

            StartCoroutine(ResetCooldown("GravQ", gravExplosion.cooldown));
        }
    }

    //중력자탄 E스킬(강력한 레이저 발사)
    public void GravESkill(WeaponData weapon)
    {
        if (skillCooldowns["GravE"]) return; // 쿨타임이면 사용불가
        skillCooldowns["GravE"] = true;
        GameManager.inst.screenUI.UpdateHackUI(1);

        GravLaserData gravLaser = (GravLaserData)weapon.eSkillData;
        float final_damage = gravLaser.damage;
        float final_width = gravLaser.width;
        float final_range = gravLaser.range;
        float final_duration = gravLaser.duration;
        if (weapon.accessoryData1 is GravityEAccessory acc1)
        {
            final_damage = Mathf.RoundToInt(final_damage * acc1.damageMult);
            final_width *= acc1.widthMult;
            final_range *= acc1.rangeMult;
        }
        if (weapon.accessoryData2 is GravityEAccessory acc2)
        {
            final_damage = Mathf.RoundToInt(final_damage * acc2.damageMult);
            final_width *= acc2.widthMult;
            final_range *= acc2.rangeMult;
        }

        GameObject laserGO = GameManager.inst.pool.GetRange(4);
        laserGO.transform.position = transform.position;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector2 dir = (mousePos - transform.position).normalized;

        StrongLaser laser = laserGO.GetComponent<StrongLaser>();
        laser.followPlayer = true;
        laser.playerRef = transform;
        laser.SetStats(dir, final_damage, final_width, final_range, final_duration);

        if (gravLaser.attackSFX == null)
        {
            Debug.LogWarning("attackSFX is null!");
        }
        else
        {
            Debug.Log($"attackSFX name: {gravLaser.attackSFX.name}");
            SoundManager.inst.PlaySFX(gravLaser.attackSFX, 2);
        }

        float forcePerFrame = 10f;  //밀리는 정도
        StartCoroutine(RecoilWhileFiring(dir, final_duration, forcePerFrame));
        StartCoroutine(ResetCooldown("GravE", gravLaser.cooldown));
    }

    IEnumerator RecoilWhileFiring(Vector2 direction, float duration, float forcePerFrame)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        float timer = 0f;
        while (timer < duration)
        {
            // 이동 관련이나 무기 교체 시 즉시 종료
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
                yield break; // 코루틴 즉시 종료
            }

            rb.AddForce(-direction * forcePerFrame, ForceMode2D.Force);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private Vector2 GetCameraSize(Camera cam)
    {
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        return new Vector2(width, height);
    }

    private Vector2 AngleToDirection(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
    }
    
    // 스킬 쿨타임 적용
    IEnumerator ResetCooldown(string key, float time)
    {
        yield return new WaitForSeconds(time);
        skillCooldowns[key] = false;
    }
}
