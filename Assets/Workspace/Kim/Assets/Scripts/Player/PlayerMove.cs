using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMove : MonoBehaviour
{
    Player player;
    Rigidbody2D rigid;
    public SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D capsuleCollider;
    PlayerHealth health;
    public GameManager gameManager;
    

    public PlayerAttack playerAttack;
    public Player.PlayerState currentState;
    public bool avoid = false;

    public float horizontal_input;
    public float vertical_input;
    public float move_speed;
    public float climb_speed = 5f;
    public float jump_power;
    public float basemovespeed = 10f;
    public float basejumppower = 15f;
    public float gravS = 5f;
    bool isDashing = false;
    public bool isInvincible = false;
    //bool isDead = false;
    float speedFactor = 1f;
    bool canDoubleJump = true;

    public BoxCollider2D ladderCheck;
    private bool nearLadder = false;
    private bool isClimbing = false;
    private bool isGrounded = true;
    private bool wasGrounded = true;
    private bool AlignedLadder = false;
    //private bool isNearShop = false;
   // public GameObject shopUI;  // 상점 UI 오브젝트
    //public ShopUIManager shopuimanager;
    DashCooldownManager dashCooldown;

    public bool isTutorial = false;

    private float footstepTimer = 0.2f;
    private float footstepInterval = 0.25f;
    private float runEffectInterval = 0.1f;
    private float runEffectTimer = 0f;
    public AudioClip dashAudio;
    public AudioClip parryAudio;
    public AudioClip runAudio;
    public AudioClip jumpAudio;
    public AudioClip landingAudio;

    
    void Awake()
    {
        currentState = Player.PlayerState.Default;
        playerAttack = GetComponent<PlayerAttack>();
        player = GetComponent<Player>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        health = GetComponent<PlayerHealth>();
        dashCooldown = GetComponent<DashCooldownManager>();

        // Transform ladderObj = transform.Find("LadderCheck");
        // if (ladderObj != null)
        //     ladderCheck = ladderObj.GetComponent<BoxCollider2D>();
        // else
        //     Debug.LogError("LadderCheck 오브젝트를 찾을 수 없습니다");

        health.Init();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // 일시정지 UI 출력하는건데 놓을곳이 애매하네요
            GameManager.inst.screenUI.SetPauseUI();

        if (GameManager.inst.IsPaused || GameManager.inst.IsOnInventory) return;

        if (health.CurrentHP <= 0)
        {
            rigid.linearVelocity = Vector2.zero;
            currentState = Player.PlayerState.Dead;
            return;
        }

        if (health.CurrentHP > 0)
        {
            horizontal_input = Input.GetAxisRaw("Horizontal");
            vertical_input = Input.GetAxisRaw("Vertical");

            if (nearLadder && vertical_input > 0f && !isClimbing)
            {
                isClimbing = true;
                AlignedLadder = false;
            }

            //JUMP
            if (Input.GetButtonDown("Jump"))
            {
                if (isGrounded)
                {
                    if (jumpAudio == null)
                    {
                        Debug.LogWarning("attackSFX is null!");
                    }
                    else
                    {
                        Debug.Log($"attackSFX name: {jumpAudio.name}");
                        SoundManager.inst.PlaySFX(jumpAudio, 4);
                    }
                    canDoubleJump = true;
                    rigid.AddForce(Vector2.up * jump_power, ForceMode2D.Impulse);
                    if (isClimbing) isClimbing = false;
                }
                else if (canDoubleJump && !isClimbing)
                {
                    if (jumpAudio == null)
                    {
                        Debug.LogWarning("attackSFX is null!");
                    }
                    else
                    {
                        Debug.Log($"attackSFX name: {jumpAudio.name}");
                        SoundManager.inst.PlaySFX(jumpAudio, 4);
                    }
                    canDoubleJump = false;
                    rigid.linearVelocity = new Vector2(rigid.linearVelocityX, 0);
                    rigid.AddForce(Vector2.up * jump_power, ForceMode2D.Impulse);
                    if (isClimbing) isClimbing = false;
                    anim.SetTrigger("doubleJump");
                }
            }

            //Stop Speed
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
                rigid.linearVelocity = new Vector2(rigid.linearVelocity.normalized.x * 0.00001f, rigid.linearVelocity.y);

            //Direction <- 공격중이지 않을때만 처리하게 했습니다. 공격중 방향은 PlayerAttack에서 처리합니다.
            if (currentState == Player.PlayerState.Default)
            {
                if (horizontal_input < 0)
                {
                    //rigid.linearVelocity = new Vector2(horizontal_input * move_speed, rigid.linearVelocityY);
                    spriteRenderer.flipX = true;

                }
                else if (horizontal_input > 0)
                {
                    //rigid.linearVelocity = new Vector2(horizontal_input * move_speed, rigid.linearVelocityY);
                    spriteRenderer.flipX = false;

                }
            }



            // Dash
            if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && dashCooldown.CanDash())
            {
                StartCoroutine(Dash());
                dashCooldown.StartCooldown();
            }

            // if (Input.GetKeyDown(KeyCode.P)) // 디버그용
            //     GameManager.inst.enemyManager.Despawn();
        }

       /*     if (Input.GetKeyDown(KeyCode.G))
            {
                ResourceManager.CollectResource(30);
                ToggleShopUI();
            }
        }*/
    }
    /*
    void ToggleShopUI()
    {
        bool isActive = shopUI.activeSelf;
        if (!isActive)
        {
            Time.timeScale = 0f;
            shopuimanager.GenerateRandomShopItems();
            shopuimanager.PopulateShop();  // 상점 UI 내용 업데이트
            shopUI.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            shopUI.SetActive(false);
        }
    }
    */

    void FixedUpdate()
    {
        //Move Speed
        if (!isDashing && (health.CurrentHP > 0) && !avoid)
        {
            rigid.linearVelocity = new Vector2(horizontal_input * move_speed * speedFactor, rigid.linearVelocity.y);
        }

        //Landing Platform -> isGrounded라는 변수 새로 설정
        if (health.CurrentHP > 0)
        {
            Vector2 origin = new Vector2(rigid.position.x, capsuleCollider.bounds.min.y - 0.05f); // 발바닥 기준
            Debug.DrawRay(origin, Vector2.down * 0.1f, new Color(0, 1, 0));

            RaycastHit2D rayHit = Physics2D.Raycast(origin, Vector2.down, 0.1f, LayerMask.GetMask("Platform", "Ladder"));
            bool nowGrounded = rayHit.collider != null && rayHit.collider.gameObject.layer != LayerMask.NameToLayer("Ladder");

            // 착지 순간만 감지
            if (!wasGrounded && nowGrounded)
            {
                if (landingAudio != null)
                {
                    Debug.Log($"[착지 사운드] {landingAudio.name}");
                    SoundManager.inst.PlaySFX(landingAudio, 1f);
                }
                else
                {
                    Debug.LogWarning("landingAudio is null!");
                }

                    GameObject dust = GameManager.inst.pool.GetEffect(1);
                    if (dust != null)
                    {
                        dust.transform.position = transform.position + Vector3.down * 0.5f;
                        dust.transform.rotation = Quaternion.identity;
                    }
            }

            // 점프 애니메이션 처리
            anim.SetBool("isJump", !nowGrounded);
            isGrounded = nowGrounded;
            wasGrounded = nowGrounded; // 이전 상태 갱신
        }

        if (isClimbing)
        {
            if (!AlignedLadder) StartClimbing();
            rigid.gravityScale = 0f;
            rigid.linearVelocity = new Vector2(rigid.linearVelocityX, vertical_input * climb_speed);
            if (AlignedLadder && isGrounded) { isClimbing = false; anim.SetBool("isClimbing", false); }

        }
        else
        {
            AlignedLadder = false;
            rigid.gravityScale = gravS;
        }

        //Run Animation
        bool isRunning = rigid.linearVelocityX != 0f && isGrounded && !avoid;
        anim.SetBool("isRun", isRunning);

        if (isRunning)
        {
            footstepTimer += Time.fixedDeltaTime;
            runEffectTimer += Time.fixedDeltaTime;
            if (footstepTimer >= footstepInterval)
            {
                footstepTimer = 0f;

                if (runAudio != null)
                {
                    SoundManager.inst.PlaySFX(runAudio, 0.8f);
                }
            }

            if (runEffectTimer >= runEffectInterval)
            {
                runEffectTimer = 0f;

                GameObject dust = GameManager.inst.pool.GetEffect(1);
                if (dust != null)
                {
                    dust.transform.position = new Vector3(transform.position.x, capsuleCollider.bounds.min.y, -1f);
                    dust.transform.rotation = Quaternion.identity;
                }
            }
        }
        else
        {
            footstepTimer = 0f;
            runEffectTimer = 0f;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            // 적 특수 공격에 한해 패링 가능, 내가 패링 중이라면 데미지 무시
            if (enemy != null && playerAttack.isParrying && enemy.isOnTrait)
            {
                Debug.Log("패링 성공 - 플레이어는 데미지 없음");
                GameManager.inst.combatEffectManager.DoHitStop(0.25f);
                if (parryAudio == null)
                {
                    Debug.LogWarning("attackSFX is null!");
                }
                else
                {
                    Debug.Log($"attackSFX name: {parryAudio}");
                    SoundManager.inst.PlaySFX(parryAudio);
                }
                return; // 데미지 입지 않음
            }

            if (!isInvincible) OnDamaged(collision.transform.position);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (!isInvincible) OnDamaged(collision.transform.position);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            if (playerAttack.isParrying)
            {
                Debug.Log("패링 성공!");
                GameManager.inst.combatEffectManager.DoHitStop(0.25f);
                if (parryAudio == null)
                {
                    Debug.LogWarning("attackSFX is null!");
                }
                else
                {
                    Debug.Log($"attackSFX name: {parryAudio}");
                    SoundManager.inst.PlaySFX(parryAudio);
                }
                collision.gameObject.SetActive(false);
            }
            else
            {
                if (!isInvincible) OnDamaged(collision.transform.position);
            }
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            nearLadder = true;
        }

        // if (collision.gameObject.CompareTag("Shop"))
        // {
        //     isNearShop = true;
        //     print("플레이어가 상점 근처로 들어옴");
        // }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            StartCoroutine(DelayedLadderExit());
        }

        // if (other.gameObject.CompareTag("Shop"))  
        // {
        //     isNearShop = false;
        //     print("플레이어가 상점 밖으로 나감");
        // }
    }

    IEnumerator DelayedLadderExit()
    {
        yield return new WaitForSeconds(0.2f);

        // 0.2초 동안 다시 사다리에 안 들어왔으면 벗어난 것으로 간주
        if (!Physics2D.OverlapBox(ladderCheck.bounds.center, ladderCheck.bounds.size, 0f, LayerMask.GetMask("Ladder")))
        {
            nearLadder = false;
            isClimbing = false;
            AlignedLadder = false;
            rigid.gravityScale = gravS;
            anim.SetBool("isClimbing", false);
        }
    }

    public void OnDamaged(Vector2 targetPos)
    {
        if (!isInvincible)
        {
            health.TakeDamage(!isTutorial? 1 : 0);
            isInvincible = true; // 일정시간 무적 설정
            if (health.CurrentHP <= 0) // 사망에 대한 처리를 PlayerHealth 부분으로 옮겨주시면 좋을듯합니다!
            {
                Debug.Log("Game Over!");
                anim.SetTrigger("Dead");
                gameObject.layer = 8;
                rigid.linearVelocity = Vector2.zero;
                return;
            }

            //gameObject.layer = 8;
            //spriteRenderer.color = new Color(1, 1, 1, 0.4f);
            anim.SetTrigger("Damaged");
            int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
            rigid.linearVelocity = Vector2.zero;
            rigid.AddForce(new Vector2(dirc * 7, 7), ForceMode2D.Impulse);
            Invoke("StopKnockBack", 0.2f);
            Invoke("OffDamaged", 1);
        }
    }

    void StartClimbing() // 사다리 감지해서 사다리타기 시작 (중앙으로)
    {
        if (AlignedLadder) return;
        anim.SetBool("isClimbing", true);

        Collider2D[] colliders = Physics2D.OverlapBoxAll(ladderCheck.bounds.center, ladderCheck.bounds.size, 0);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Ladder"))
            {
                Tilemap tilemap = collider.GetComponent<Tilemap>();
                if (tilemap != null)
                {
                    Vector3 worldPos = transform.position;
                    Vector3Int cellPos = tilemap.WorldToCell(worldPos + Vector3.up * 0.5f); // 보정값 추가
                    Vector3 cellCenter = tilemap.GetCellCenterWorld(cellPos);

                    transform.position = new Vector2(cellCenter.x, transform.position.y);
                }

                rigid.linearVelocity = Vector2.zero;
                rigid.gravityScale = 0f;
                break;
            }
        }
        AlignedLadder = true;
    }

    IEnumerator Dash()
    {
        isDashing = true;
        rigid.linearVelocity = new Vector2(40 * (spriteRenderer.flipX ? -1 : 1), rigid.linearVelocity.y);
        if (dashAudio == null)
        {
            Debug.LogWarning("attackSFX is null!");
        }
        else
        {
            Debug.Log($"attackSFX name: {dashAudio}");
            SoundManager.inst.PlaySFX(dashAudio);
        }
        yield return new WaitForSeconds(0.15f);
        isDashing = false;
    }

    public void StopKnockBack()
    {
        if (horizontal_input == 0)
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.normalized.x * 0.00001f, rigid.linearVelocity.y);
        avoid = false;
    }

    public void OffDamaged()
    {
        gameObject.layer = 7;
        spriteRenderer.color = new Color(1, 1, 1, 1);
        isInvincible = false; // 무적 해제
    }

    // void OnGUI()
    // {
    //     GUI.Label(new Rect(10, 10, 300, 20), $"isGrounded: {isGrounded}");
    //     GUI.Label(new Rect(10, 30, 300, 20), $"isClimbing: {isClimbing}");
    //     GUI.Label(new Rect(10, 50, 300, 20), $"PlayerState: {currentState}");
    //     GUI.Label(new Rect(10, 70, 200, 20), $"nearLadder: {nearLadder}");
    //     GUI.Label(new Rect(10, 90, 200, 20), $"nearLadder: {AlignedLadder}");
    // }

    public void AddSpeedFactor(float fac)
    {
        speedFactor += fac;
    }
}