using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BasePlayer : MonoBehaviour
{
    BasePlayer player;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D capsuleCollider;

    public PlayerAttack playerAttack;
    public Player.PlayerState currentState;

    public float horizontal_input;
    public float vertical_input;
    public float move_speed = 10f;
    public float climb_speed = 5f;
    public float jump_power = 10f;
    public float gravS = 5f;
    bool isDashing = false;
    bool isNearShop = false;


    public GameObject shopUI;  // 상점 UI 오브젝트
    public ShopUIManager shopuimanager;

    void Awake()
    {
        if (GameManager.inst != null) GameManager.inst.basePlayer = this;

        currentState = Player.PlayerState.Default;
        playerAttack = GetComponent<PlayerAttack>();
        player = GetComponent<BasePlayer>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {
        if (GameManager.inst.IsPaused || GameManager.inst.IsOnInventory) return;

        horizontal_input = Input.GetAxisRaw("Horizontal");
        vertical_input = Input.GetAxisRaw("Vertical");

        //Stop Speed
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.normalized.x * 0.00001f, rigid.linearVelocity.y);
            anim.SetBool("isRun", false);
        }

        //Direction
        if (currentState == Player.PlayerState.Default)
        {
            if (horizontal_input != 0) anim.SetBool("isRun", true);

            if (horizontal_input < 0)
                spriteRenderer.flipX = true;
            else if (horizontal_input > 0)
                spriteRenderer.flipX = false;
        }
        //Avoid
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
            StartCoroutine(Dash());

        if (isNearShop && Input.GetKeyDown(KeyCode.G))
        {
            ToggleShopUI();
        }
    }

    void FixedUpdate()
    {
        //Move Speed
        if (!isDashing)
            rigid.linearVelocity = new Vector2(horizontal_input * move_speed, rigid.linearVelocity.y);

        //Landing Platform -> isGrounded라는 변수 새로 설정
        Vector2 origin = new Vector2(rigid.position.x, capsuleCollider.bounds.min.y - 0.05f); // 발바닥 기준
        Debug.DrawRay(origin, Vector2.down * 0.1f, new Color(0, 1, 0));

        RaycastHit2D rayHit = Physics2D.Raycast(origin, Vector2.down, 0.1f, LayerMask.GetMask("Platform", "Ladder"));

        //Run Animation
        // if (rigid.linearVelocityX != 0f && rigid.linearVelocityY == 0)
        //     anim.SetBool("isRun", true);
        // else anim.SetBool("isRun", false);
    }

    IEnumerator Dash()
    {
        isDashing = true;
        rigid.linearVelocity = new Vector2(40 * (spriteRenderer.flipX ? -1 : 1), rigid.linearVelocity.y);
        yield return new WaitForSeconds(0.1f);
        isDashing = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Shop"))
        {
            isNearShop = true;
            print("플레이어가 상점 근처로 들어옴");
        }
    }
    
    void ToggleShopUI()
    {
        bool isActive = shopUI.activeSelf;
        if (!isActive)
        {
            Time.timeScale = 0f;
            shopuimanager.PopulateShop();  // 상점 UI 내용 업데이트
            shopUI.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            shopUI.SetActive(false);
        }
    }
}
