using UnityEngine;
using System.Collections;

public class SurroundShootSkillPattern : MonoBehaviour
{
    public BossController bossController;
    public GameObject bulletPrefab;
    public Transform player;

    public int bulletCount = 8;
    public float surroundRadius = 3f;
    public float surroundDelay = 2f;
    public float shootSpeed = 8f;
    public float bulletDestroyTime = 5f;
    private bool isHardMode = false;

    public void Init(Difficulty difficulty)
    {
        if (difficulty == Difficulty.Hard)
        {
            bulletCount = 14;      // 총알 수 증가
            shootSpeed = 12f;      // 더 빠르게
            surroundDelay = 1.5f;    // 대기시간 감소
            isHardMode = true;
        }
        else
        {
            bulletCount = 8;
            shootSpeed = 8f;
            surroundDelay = 2f;
            isHardMode = false;
        }
    }

    void OnEnable()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(SurroundShootRoutine());
    }

    IEnumerator SurroundShootRoutine()
    {
        Debug.Log($"Surround Shoot Skill 시작 | HardMode: {isHardMode}");

        Vector2 playerPosition = player.position;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * (360f / bulletCount);
            Vector2 spawnPos = playerPosition + new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * surroundRadius;

            Vector2 direction = (playerPosition - spawnPos).normalized;
            Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

            GameObject bullet = Instantiate(bulletPrefab, spawnPos, rotation);
            bullet.tag = "EnemyBullet";
            int bulletLayer = LayerMask.NameToLayer("Bullet");
            if (bulletLayer == -1)
            {
                Debug.LogError("Bullet 레이어가 존재하지 않습니다! Layers 설정을 확인하세요.");
            }
            SetLayerRecursively(bullet, bulletLayer);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = Vector2.zero;

            Destroy(bullet, bulletDestroyTime);
        }

        yield return new WaitForSeconds(surroundDelay);

        GameObject[] bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        foreach (GameObject bullet in bullets)
        {
            if (bullet != null)
            {
                Vector2 dir = (player.position - bullet.transform.position).normalized;
                bullet.GetComponent<Rigidbody2D>().linearVelocity = dir * shootSpeed;
            }
        }

        yield return new WaitForSeconds(2f);

        bossController.EndPattern();
    }
    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null)
            return;

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (child == null) continue;
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }


    void OnDisable()
    {
        StopAllCoroutines();
    }
}
