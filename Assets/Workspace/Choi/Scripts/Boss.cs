using System.Collections;
using UnityEngine;

public class Boss : Enemy
{
    /*private BossController bossController;
    private BossHealthUI bossHealthUI;

    protected override void Start()
    {
        base.Start();

        bossController = GetComponent<BossController>();
        bossHealthUI = GameManager.inst.worldUI.GetBossUI(); // 보스 체력바 가져오기

        if (bossHealthUI != null)
        {
            bossHealthUI.Init("Boss Name", maxHealth); // 보스 이름과 최대 체력 설정
        }
    }

    protected override void FixedUpdate()
    {
        if (isDead) return;

        rigid.gravityScale = 0f; // 보스는 공중 부양

        if (bossController != null && !bossController.isActiveAndEnabled)
        {
            bossController.enabled = true;
        }

        MoveTowardPlayer();
    }

    private void MoveTowardPlayer()
    {
        if (target == null) return;

        float direction = target.position.x >= transform.position.x ? 1f : -1f;
        rigid.linearVelocity = new Vector2(direction * chaseSpeed, 0);
        sprite.flipX = (direction < 0);
    }

    public override void Damaged(int damage, Vector2 dir)
    {
        if (isDead || isInvincible) return;

        currentHealth -= damage;
        isInvincible = true;
        anim.SetTrigger("Hurt");

        if (bossHealthUI != null)
        {
            bossHealthUI.UpdateHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            StartCoroutine(Dead());
            return;
        }
    }

    protected override IEnumerator Dead()
    {
        isDead = true;
        rigid.linearVelocity = Vector2.zero;
        transform.tag = "Untagged";
        gameObject.layer = LayerMask.NameToLayer("Default");

        anim.SetTrigger("Death");

        if (bossHealthUI != null)
        {
            bossHealthUI.Hide();
        }

        TryDropResource(transform.position);

        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }*/
}
