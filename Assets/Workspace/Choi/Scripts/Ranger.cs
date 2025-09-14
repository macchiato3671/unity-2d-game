using UnityEngine;
using System.Collections;

public class Ranger2 : MonoBehaviour
{
    public float detectionRange = 5f; // 플레이어를 감지하는 범위  
    public float attackCooldown = 2f; // 공격 간격  
    public GameObject bulletPrefab; // 투사체 프리팹  
    public float bulletSpeed = 5f; // 투사체 속도  
    public Transform firePoint; // 투사체가 발사되는 위치  
    public int maxHealth = 100; // 최대 체력  
    public int attackPower = 1;
    public float knockbackForce = 4f;

    private Transform player; 
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;  
    private int currentHealth;   
    private float lastAttackTime;    
    private bool isKnockedBack = false; 

    private void Start()
    { 
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); 
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (!isKnockedBack)
        {
            float distance = Vector2.Distance(transform.position, player.position);

            // 플레이어가 감지 범위 안에 들어오면 추적 시작  
            if (distance < detectionRange)
            {

                // 공격 쿨다운이 끝났다면 공격  
                if (Time.time > lastAttackTime + attackCooldown)
                {
                    Attack();
                    lastAttackTime = Time.time; // 공격 시간 업데이트  
                }
            }
        }
 
    }

    private void Attack()
    {
        if (player == null) return; // 플레이어가 없으면 공격하지 않음  

        // 플레이어 방향 계산  
        Vector2 direction = (player.position - firePoint.position).normalized;

        //GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity); 
        Transform bullet = GameManager.inst.pool.GetRange(1).transform;
        bullet.localPosition = firePoint.position;
        bullet.localRotation = Quaternion.identity;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * bulletSpeed;

     
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 knockBackDirection = (transform.position - collision.transform.position).normalized;
            StartCoroutine(Knockback(knockBackDirection));

            // 💥 플레이어에게 데미지 주기
            if (Time.time > lastAttackTime + attackCooldown)
            {
                Player2 player = collision.gameObject.GetComponent<Player2>();
                if (player != null && player.Health != null)
                {
                    player.Health.TakeDamage(attackPower);
                    lastAttackTime = Time.time;
                }
            }
        }
    }

    private IEnumerator Knockback(Vector2 direction)
    {
        isKnockedBack = true;
        rb.linearVelocity = direction * knockbackForce;

        Damaged(20);
        yield return new WaitForSeconds(0.2f);

        isKnockedBack = false;
        rb.linearVelocity = Vector2.zero;
    }

    public void Damaged(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) Destroy(gameObject);
    }
}

