using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove2 : MonoBehaviour
{
    Player2 player;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D capsuleCollider;
    public GameObject hitbox;
    
    public int horizontal_input;
    public float max_speed;
    public float jump_power;

    bool avoid = false;

    void Awake()
    {
        player = GetComponent<Player2>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {
        //JUMP
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJump")) 
        {
            rigid.AddForce(Vector2.up * jump_power, ForceMode2D.Impulse);
            anim.SetBool("isJump", true);
        }

        //Move
        if (Input.GetKey(KeyCode.A))
            horizontal_input = -1;
        else if (Input.GetKey(KeyCode.D))
            horizontal_input = 1;
        else
            horizontal_input = 0;

        //Stop Speed
        if(Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) 
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.normalized.x * 0.00001f, rigid.linearVelocity.y);


        //Direction <- 공격중이지 않을때만 처리하게 했습니다. 공격중 방향은 PlayerAttack에서 처리합니다.
        if(player.State == Player2.PlayerState.Default){
            if (horizontal_input < 0) {
                spriteRenderer.flipX = true;
                hitbox.transform.localPosition = new Vector2(-1.5f, 0);
            }
            else if (horizontal_input > 0) {
                spriteRenderer.flipX = false;
                hitbox.transform.localPosition = new Vector2(1.5f, 0);
            }
        } 

        //Attack <- PlayerAttack으로 이동했습니다!
        // if (Input.GetMouseButtonDown(0))
        // {
        //     anim.SetTrigger("Attack1");
        //     StartCoroutine(ActivateHitbox());
        // }

        //Avoid
        if (Input.GetKeyDown(KeyCode.LeftShift) && !avoid)
        {
            StartCoroutine(AvoidCoroutine());
        }

        //Run Animation
        if (Mathf.Abs(rigid.linearVelocity.x) > 0.5f && !avoid)
            anim.SetBool("isRun", true);
        else
            anim.SetBool("isRun", false);

    }

    void FixedUpdate() 
    {
        //Move Speed
        rigid.AddForce(Vector2.right * horizontal_input, ForceMode2D.Impulse);

        //Max Speed
        if(rigid.linearVelocity.x > max_speed) //Right Max Speed
            rigid.linearVelocity = new Vector2(max_speed, rigid.linearVelocity.y);
        else if (rigid.linearVelocity.x < max_speed*(-1)) //Left Max Speed
            rigid.linearVelocity = new Vector2(max_speed * (-1), rigid.linearVelocity.y);

        //Landing Platform
        if(rigid.linearVelocity.y < 0) {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if(rayHit.collider != null) {
                if(rayHit.distance < 1.0f) {
                    anim.SetBool("isJump", false);
                    anim.SetBool("isRun", false);
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy") {
            //Damaged
            OnDamaged(collision.transform.position);
        }
    }

    void OnDamaged(Vector2 targetPos)
    {
        // Change Layer (Immortal Active)
        gameObject.layer = 8;

        //View Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // Reaction Force
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.linearVelocity = Vector2.zero;
        rigid.AddForce(new Vector2(dirc * 7, 7), ForceMode2D.Impulse);

        // Animation
        anim.SetTrigger("Damaged");

        Invoke("StopKnockBack", 0.2f);

        Invoke("OffDamaged", 1);
    }

    IEnumerator AvoidCoroutine()
    {
        avoid = true;
        anim.SetTrigger("Avoid");
        gameObject.layer = 8;

        if (spriteRenderer.flipX)
            rigid.AddForce(new Vector2(-1.0f, -1.0f) * max_speed, ForceMode2D.Impulse);
        else
            rigid.AddForce(new Vector2(1.0f, -1.0f) * max_speed, ForceMode2D.Impulse);

        //Avoid time
        yield return new WaitForSeconds(0.4f);

        StopKnockBack();
        OffDamaged();

        avoid = false;
    }

    //Hitbox Activation <- PlayerAttack으로 이동했습니다!! (로직도 추가)
    // IEnumerator ActivateHitbox()
    // {
    //     hitbox.SetActive(true);
    //     yield return new WaitForSeconds(0.6f);
    //     hitbox.SetActive(false);
    // }

    void StopKnockBack()
    {
        if (horizontal_input == 0) 
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.normalized.x * 0.00001f, rigid.linearVelocity.y);
        avoid = false;
    }

    void OffDamaged()
    {
        gameObject.layer = 7;

        spriteRenderer.color = new Color(1, 1, 1, 1);
    }
}
