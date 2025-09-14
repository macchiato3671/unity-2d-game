using UnityEngine;
using System.Collections;

public class TeleportDashSkillPattern : MonoBehaviour
{
    public BossController bossController;
    public Rigidbody2D bossRigidbody;
    public Transform player;

    public float teleportDistance = 3f;
    public float dashSpeed = 10f;
    private bool isHardMode = false;

    public void Init(Difficulty difficulty)
    {
        if (difficulty == Difficulty.Hard)
        {
            dashSpeed = 15f;  // 초고속 돌진
            isHardMode = true;
        }
        else
        {
            dashSpeed = 10f;
            isHardMode = false;
        }
    }

    void OnEnable()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(TeleportDashRoutine());
    }

    IEnumerator TeleportDashRoutine()
    {
        Debug.Log($"Teleport + Dash Skill 시작 | HardMode: {isHardMode}");

        Vector2 playerPosition = player.position;
        int teleportCount = isHardMode ? 2 : 1;  // 하드모드면 두 번 순간이동 후 돌진
        Vector2 teleportPosition = Vector2.zero;

        for (int t = 0; t < teleportCount; t++)
        {
            bool foundValidPosition = false;
            for (int i = 0; i < 10; i++)
            {
                Vector2 candidate = playerPosition + Random.insideUnitCircle * teleportDistance;
                RaycastHit2D groundHit = Physics2D.Raycast(candidate, Vector2.down, 1.5f, LayerMask.GetMask("Platform"));
                Collider2D overlap = Physics2D.OverlapCircle(candidate, 0.4f, LayerMask.GetMask("Platform"));

                if (groundHit.collider != null && overlap == null)
                {
                    teleportPosition = candidate;
                    foundValidPosition = true;
                    break;
                }
            }

            if (foundValidPosition)
            {
                bossRigidbody.position = teleportPosition;
                yield return new WaitForSeconds(0.5f);  // Hard면 짧게 기다림
            }
            else
            {
                Debug.LogWarning("유효한 순간이동 위치를 찾지 못했습니다.");
                bossController.EndPattern();
                yield break;
            }
        }

        Vector2 dashDirection = (playerPosition - teleportPosition).normalized;
        bossRigidbody.linearVelocity = dashDirection * dashSpeed;

        yield return new WaitForSeconds(2f);

        bossController.EndPattern();
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }
}
