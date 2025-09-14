using System.Collections;
using UnityEngine;

public class PlayerAttack2 : MonoBehaviour
{
    Player2 player;
    SpriteRenderer sprite;
    Animator anim;
    [SerializeField] private GameObject hitbox;

    bool onAttack; // 근접 공격이 작동중일시 true
    bool canShot; // 원거리 공격 가능시 true
    

    void Awake()
    {
        player = GetComponent<Player2>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        onAttack = false;
        canShot = true;
    }

    void Update()
    {
        // 마우스 좌클릭에 따라 플레이어의 애니메이션 설정 / 우클릭에 따라 원거리 공격
        if(Input.GetMouseButtonDown(0)){
            player.State = Player2.PlayerState.Attack;
            anim.SetTrigger("Attack1");
            if(!onAttack)
                StartCoroutine(ActivateHitbox());
        }else if(Input.GetMouseButtonUp(0)){
            player.State = Player2.PlayerState.Default;
        }else if(Input.GetMouseButtonDown(1) && canShot){
            StartCoroutine(Shot());
        }
    }

    void LateUpdate()
    {
        // 공격중에만, 마우스 위치 계산을 통한 플레이어의 바라보는 방향 처리
        if(player.State == Player2.PlayerState.Attack){
            Vector3 mouseVec = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            if(mouseVec.x < 0){
                sprite.flipX = true;
                hitbox.transform.localPosition = new Vector2(-1.5f, 0);
            }
            else{
                sprite.flipX = false;
                hitbox.transform.localPosition = new Vector2(1.5f, 0);
            }
        }
    }


    IEnumerator Shot(){
        // PoolManager를 통해 투사체를 생성하고, 위치, 회전, 공격 방향 설정 후 공격 쿨타임 적용
        Transform proj = GameManager.inst.pool.GetRange(0).transform;
        Range projComp = proj.GetComponent<Range>();
        Vector3 mouseVec = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        mouseVec.z = 0;

        proj.localPosition = transform.position;
        proj.localRotation = Quaternion.identity;
        proj.rotation = Quaternion.FromToRotation(Vector3.up,mouseVec);
        proj.Rotate(90f * Vector3.forward);
        proj.Translate(mouseVec.normalized * 1f, Space.World);

        //projComp.Init(mouseVec.normalized, 20);

        canShot = false;
        StartCoroutine(ShotCoolDown());
        yield return null;
    }

    IEnumerator ShotCoolDown(){
        yield return new WaitForSeconds(0.5f);
        canShot = true;
    }

    IEnumerator ActivateHitbox()
    {
        onAttack = true;
        hitbox.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        hitbox.SetActive(false);
        onAttack = false;
    }
}
