using UnityEngine;

public class TrainingDummy : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider;
    public int nextMove;
    public float speedScale;
    public int maxHealth = 30;
    public Vector2 externalForce = Vector2.zero;
    private int currentHealth;
    
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        currentHealth = maxHealth; 

        Think();
        Invoke("Think", 5);
    }

    void FixedUpdate()
    {
        // 기존 이동
        Vector2 baseVelocity = new Vector2(nextMove * speedScale, rigid.linearVelocity.y);

        // 외부 힘 추가
        rigid.linearVelocity = baseVelocity + externalForce;

        // 외부 힘은 한 프레임만 유지
        externalForce = Vector2.zero;

        // Platform 체크
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if(rayHit.collider == null) {
            Turn();
        }
    }

    public void Damaged(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) Destroy(gameObject);
    }

    //재귀 함수
    void Think()
    {
        //Set Next Active
        nextMove = Random.Range(-1,2);
        
        //Flip Sprite
        if(nextMove != 0)
            spriteRenderer.flipX = nextMove == 1;

        //Recursive
        float nextThinkTime = Random.Range(2.0f, 5.0f);
        Invoke("Think", nextThinkTime);
    }

    void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke();
        Invoke("Think", 2);
    }

    public void Stop(float changeScale) {
        speedScale = changeScale;
        Invoke("MoveAgain", 5);
    }

    void MoveAgain() {
        speedScale = 3;
    }
}
