using UnityEngine;
using System.Collections;

public class EnemySkill : MonoBehaviour
{
    public GameObject player; // �÷��̾� ����  
    public GameObject bulletPrefab; // Bullet ������  

    public float reflectSpeed = 10f; // �ݻ� �Ѿ� �ӵ�  
    public float shootSpeed = 8f; // ���� �߻� �ӵ�  
    public float teleportDistance = 3f; // �����̵� �� ���� �Ÿ�  
    public float bulletDestroyTime = 5f; // Bullet�� �ִ� ����  

    public int surroundBulletCount = 8; // �� ��° ��ų: ������ �Ѿ� ����  
    public float surroundRadius = 3f; // �� ��° ��ų: ������ �Ѿ� ������  
    public float surroundDelay = 2f; // �� ��° ��ų: �Ѿ� ���� �� �÷��̾ ���� ���������� ��� �ð�  

    private float secondSkillCooldown = 5f; // �� ��° ��ų ��Ÿ��  
    private float thirdSkillCooldown = 10f; // �� ��° ��ų ��Ÿ��  

    private bool canUseSecondSkill = true; // �� ��° ��ų ��� ���� ����  
    private bool canUseThirdSkill = true; // �� ��° ��ų ��� ���� ����  

    /*
     * 1. �÷��̾��� ��ų �ݻ�(enemy parrying) (�Ѿ˹ݻ�α���)
     * 2. �÷��̾� ��ġ ���� �������� �Ѿ� ���� �� ���� (Song>prefab>range1 �Ѿ˷� ���)
     * 3. �÷��̾� ��ó ��ġ�� �����̵� �� 1�ʵ� ����
     */

    void Start()
    {
        // �÷��̾ Ȯ��  
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    void Update()
    {
        // �� ��° ��ų: ���� �ð����� �ߵ�  
        if (canUseSecondSkill)
        {
            StartCoroutine(TriggerSecondSkill());
        }

        // �� ��° ��ų: ���� �ð����� �ߵ�  
        if (canUseThirdSkill)
        {
            StartCoroutine(TriggerThirdSkill());
        }
    }

    // ù ��° ��ų: �÷��̾��� ���� �ݻ� (�÷��̾� �ҷ��� ���� ���) 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            float chance = Random.value; // 0.0 ~ 1.0 ������ ���� ��

            if (chance <= 0.3f) // 30% Ȯ���� Reflect
            {
                ReflectSkill(collision.gameObject);
            }
            else
            {
                Destroy(collision.gameObject); // �ݻ�ȵǸ� �����. -> damaged �߰��ʿ�
            }
        }
    }

    public void ReflectSkill(GameObject playerBullet)
    {
        // �÷��̾� �Ѿ��� �ݻ��Ͽ� �ٽ� �÷��̾ ���� ���ư��� ��  
        Vector2 direction = (player.transform.position - playerBullet.transform.position).normalized;

        Range bulletRange = playerBullet.GetComponent<Range>();

        if (bulletRange != null)
        {
            bulletRange.Reflect(direction, 10);
        }

        // �Ѿ� �±׸� �����Ͽ� �� �Ѿ˷� �νĵǰ� �� (�÷��̾ �µ���)  
        playerBullet.tag = "EnemyBullet";
    }

    // �� ��° ��ų: �÷��̾� ��ġ �������� �������� �Ѿ� ���� �� ����  
    public IEnumerator TriggerSecondSkill()
    {
        canUseSecondSkill = false; // ��ų ��� �Ұ�  
        SurroundShootSkill(surroundBulletCount, surroundRadius, surroundDelay); // �� ��° ��ų �ߵ�  
        yield return new WaitForSeconds(secondSkillCooldown); // ��Ÿ�� ���  
        canUseSecondSkill = true; // ��ų ��� ����  
    }

    public void SurroundShootSkill(int bulletCount, float radius, float delay)
    {
        StartCoroutine(SurroundShootCoroutine(bulletCount, radius, delay));
    }

    private IEnumerator SurroundShootCoroutine(int bulletCount, float radius, float delay)
    {
        Vector2 playerPosition = player.transform.position; // �÷��̾��� �ʱ� ��ġ�� ����  

        // �÷��̾ �߽����� �� ���·� �Ѿ� ����  
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * (360f / bulletCount); // �յ��� ����  
            Vector2 spawnPosition = playerPosition + new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;

            Vector2 directionToPlayer = (playerPosition - spawnPosition).normalized;
            float angleZ = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, 0, angleZ);
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, rotation);
            bullet.tag = "EnemyBullet";

            // BoxCollider2D�� ����� ����  
            BoxCollider2D bulletCollider = bullet.GetComponent<BoxCollider2D>();
            if (bulletCollider != null)
            {
                bulletCollider.isTrigger = true; // �浹�� Ʈ���ŷ� �����Ͽ� ���� ������ ������� ��  
            }

            bullet.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero; // �ʱ� �ӵ� ����  

            BulletDirectionMemory memory = bullet.AddComponent<BulletDirectionMemory>();
            memory.targetPosition = playerPosition;

            // Bullet ������ ���� �ڵ� ���� ó��  
            Destroy(bullet, bulletDestroyTime);
        }

        yield return new WaitForSeconds(delay); // ���� �ð� ���  

        // ������ �Ѿ˵��� �÷��̾ ���� �����ϵ��� ����  
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        foreach (GameObject bullet in bullets)
        {
            BulletDirectionMemory memory = bullet.GetComponent<BulletDirectionMemory>();
            if (memory != null)
            {
                Vector2 dir = (memory.targetPosition - (Vector2)bullet.transform.position).normalized;
                bullet.GetComponent<Rigidbody2D>().linearVelocity = dir * shootSpeed;
            }
        }
    }

    // �� ��° ��ų: �����̵� �� ����  
    public IEnumerator TriggerThirdSkill()
    {
        canUseThirdSkill = false; // ��ų ��� �Ұ�  
        yield return StartCoroutine(TeleportAndDashSkill());
        yield return new WaitForSeconds(thirdSkillCooldown); // ��Ÿ�� ���  
        canUseThirdSkill = true; // ��ų ��� ����  
    }

    public IEnumerator TeleportAndDashSkill()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 teleportPosition = Vector2.zero;
        bool validPositionFound = false;

        int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            // �ֺ� ���� ��ġ ����
            Vector2 candidatePosition = playerPosition + (Random.insideUnitCircle * teleportDistance);

            // 1. �Ʒ� �������� Raycast�ؼ� Platform�� �ִ��� Ȯ��
            RaycastHit2D groundHit = Physics2D.Raycast(candidatePosition, Vector2.down, 1.5f, LayerMask.GetMask("Platform"));

            // 2. �� ��ġ�� ���� ���� �ִ��� (��, Platform �ȿ� �Ĺ����� ���) Ȯ��
            Collider2D overlap = Physics2D.OverlapCircle(candidatePosition, 0.4f, LayerMask.GetMask("Platform"));

            // ����: �Ʒ��� Platform �ְ�, ���� ��ġ�� �浹 ���� (����)
            if (groundHit.collider != null && overlap == null)
            {
                teleportPosition = candidatePosition;
                validPositionFound = true;
                break;
            }
        }

        if (!validPositionFound)
        {
            Debug.LogWarning("��ȿ�� �����̵� ��ġ�� ã�� ���� (�÷��� �� or ����).");
            yield break;
        }

        // �����̵�
        transform.position = teleportPosition;

        // 1�� ���
        yield return new WaitForSeconds(1f);

        // ���� ���� ���
        Vector2 dashDirection = (playerPosition - teleportPosition).normalized;

        // ���� ����
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = dashDirection * 10f;
    } //�ʿ��� �ÿ� ���� �� �÷��̾�� �ε����� ���� EnemyMove(script)>knockBack �ڷ�ƾ �߰��ص� ����
}