using UnityEngine;
using System.Collections;

public class EnemyMove : MonoBehaviour
{

    public float defaultSpeed = 2f;
    public float chaseSpeed = 4f;
    public float detectionRange = 5f;
    public int attackPower = 1; // 플레이어 체력 1칸 하락
    public int maxHealth = 100;
    public float knockbackForce = 4f;
    public float flipCooldown = 0.5f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private UnityEngine.Transform player;
    private int currentHealth;
    private float lastFlipTime;

    private bool isChasing = false;
    private bool movingRight = true;
    private bool isKnockedBack = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth; 
    }

    // Update is called once per frame
    void Update()
    {
        if (!isKnockedBack)
        {
            float distance = Vector2.Distance(transform.position, player.position);

            if (distance < detectionRange)
            {
                isChasing = true;
            }
            else
            {
                isChasing = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isKnockedBack) { if (isChasing) { ChasePlayer(); } else { Idle(); } }
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * chaseSpeed, rb.linearVelocity.y);

        // 플레이어 쪽으로 스프라이트 방향 조정  
        if (direction.x > 0 && !movingRight)
        {
            Flip();
        }
        else if (direction.x < 0 && movingRight)
        {
            Flip();
        }
    }

    private void Idle()
    {
        float moveDirection = movingRight ? 1f : -1f;

        // 이동 설정  
        rb.linearVelocity = new Vector2(moveDirection * defaultSpeed, 0);

        // 적의 Collider 크기 정보 가져오기  
        Collider2D collider = GetComponent<Collider2D>();
        float colliderWidth = collider.bounds.extents.x; // Collider의 가로 반지름  
        float colliderHeight = collider.bounds.extents.y; // Collider의 세로 반지름  

        // Raycast 발사 위치 계산 (좌우 끝 각각)  
        Vector2 leftRaycastOrigin = new Vector2(transform.position.x - colliderWidth, transform.position.y - (colliderHeight + 0.1f));
        Vector2 rightRaycastOrigin = new Vector2(transform.position.x + colliderWidth, transform.position.y - (colliderHeight + 0.1f));

        // Raycast를 아래로 발사하여 바닥 감지 (좌우 각각)  
        RaycastHit2D leftGroundInfo = Physics2D.Raycast(leftRaycastOrigin, Vector2.down, 0.5f, LayerMask.GetMask("Ground"));
        RaycastHit2D rightGroundInfo = Physics2D.Raycast(rightRaycastOrigin, Vector2.down, 0.5f, LayerMask.GetMask("Ground"));

        // 디버깅용: Raycast를 시각적으로 표시  
        Debug.DrawRay(leftRaycastOrigin, Vector2.down * 0.5f, leftGroundInfo.collider != null ? Color.green : Color.red);
        Debug.DrawRay(rightRaycastOrigin, Vector2.down * 0.5f, rightGroundInfo.collider != null ? Color.green : Color.red);

        // 좌우 끝 중 하나라도 바닥을 감지하지 못하면 Flip  
        if ((leftGroundInfo.collider == null || rightGroundInfo.collider == null) && Time.time > lastFlipTime + flipCooldown)
        {
            Flip(); // 방향 전환  
        }
    }

    private void Flip()
    {
        movingRight = !movingRight;
        spriteRenderer.flipX = !spriteRenderer.flipX;

        lastFlipTime = Time.time;
    }

    public void Damaged(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 KnockBackDirection = (transform.position - collision.transform.position).normalized;
            StartCoroutine(Knockback(KnockBackDirection));
        }
        //부딪혔을시에 플레이어에게도 데미지 가게 (아직 코딩x)
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
}
