using System.Collections;
using UnityEngine;

public class SurroundRange : MonoBehaviour
{
    Rigidbody2D rigid;
    
    Vector2 dir;
    int speed;
    int bulletDamage;
    float maxRange;
    private Vector2 startPos;

    private TrailRenderer trail;
    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Bullet");
        rigid = GetComponent<Rigidbody2D>();
        rigid.freezeRotation = true;
        trail = GetComponent<TrailRenderer>();
    }

    private void OnEnable()
    {
        gameObject.layer = LayerMask.NameToLayer("Bullet");
    }

    // void Update()
    // {
    //     rigid.MovePosition(rigid.position + dir * speed * Time.fixedDeltaTime);
    // }

    void FixedUpdate()
    {
        float traveled = Vector2.Distance(transform.position, startPos);

        if (traveled >= maxRange)
        {
            rigid.linearVelocity = Vector2.zero;
            enabled = false;
        }
    }

    public void Init(Vector2 inDir, WeaponData weapon)
    {
        // 공격 방향, 속도 설정 및 시간에 따른 자동 소멸 설정
        ThrowingWeaponData throwingWeapon = (ThrowingWeaponData)weapon;

        dir = inDir;
        startPos = transform.position;
        speed = throwingWeapon.speed;
        bulletDamage = throwingWeapon.baseDamage;
        maxRange = throwingWeapon.maxRange;

        if (weapon.accessoryData1 is ThrowingNormalAccessory acc1)
        {
            bulletDamage = Mathf.RoundToInt(bulletDamage * acc1.damageMult);
            speed = Mathf.RoundToInt(speed * acc1.speedMult);
            maxRange *= acc1.maxRangeMult;
        }
        if (weapon.accessoryData2 is ThrowingNormalAccessory acc2)
        {
            bulletDamage = Mathf.RoundToInt(bulletDamage * acc2.damageMult);
            speed = Mathf.RoundToInt(speed * acc2.speedMult);
            maxRange *= acc2.maxRangeMult;
        }

        enabled = true;
        gameObject.layer = LayerMask.NameToLayer("Bullet");

        if(trail != null)
        {
            trail.Clear();
            trail.emitting = true;
        }

        // 총알 발사 시작
        rigid.linearVelocity = dir * speed;
        Debug.Log($"[Knife] Fired with Damage: {bulletDamage}, Speed: {speed}, Range: {maxRange}");
        //StartCoroutine(Remove());
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 적에 대한 hit
        if (gameObject.layer == LayerMask.NameToLayer("knifeBullet"))
        {
            if (collision.CompareTag("Enemy"))
            {
                Enemy enemy = collision.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.Damaged(bulletDamage,dir.normalized);
                    GameManager.inst.combatEffectManager.DoHitStop(0.03f);
                    Debug.Log("적을 공격함! 데미지: " + bulletDamage);
                }
            }else if(collision.CompareTag("BreakableBox")){
                 collision.GetComponent<BreakableBox>().Damaged(1);
            }
        }

        if (collision.CompareTag("EnemyBullet"))
        {
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }

    public void RecallToPlayer(Transform player)
    {
        StartCoroutine(ReturnCoroutine(player));
    }

    public void Reflect(Vector2 inDir, int inSpeed)
    {
        dir = inDir;
        speed = inSpeed;
        rigid.linearVelocity = dir * speed;
    }

    IEnumerator ReturnCoroutine(Transform player)
    {
        gameObject.layer = LayerMask.NameToLayer("Bullet");
        while (Vector2.Distance(transform.position, player.position) > 0.2f)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, 40f * Time.deltaTime);
            yield return null;
        }

        gameObject.SetActive(false); // 플레이어 도착 후 비활성화
    }

    IEnumerator Remove(){
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
    }
}
