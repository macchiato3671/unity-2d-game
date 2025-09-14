using UnityEngine;
using System.Collections;

public class BombEnemySpawner : MonoBehaviour
{
    float spawnInterval;
    public Vector3 spawnOffset = Vector3.up * 1.5f;

    private float timer = 0f;
    private float delayTimer = 0f;
    private float initialDelay = 3f;

    private bool canSpawn = false;

    private EnemyManager enemyManager;

    int spawnCount = 0;
    public int maxSpawnCount = 10;

    private bool isTurncoat = false; // ⭐ 박스가 전향되었는지 여부

    public void Init(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                spawnInterval = 5f;
                break;
            case Difficulty.Hard:
                spawnInterval = 1.5f;
                break;
        }
    }

    void Start()
    {
        enemyManager = FindAnyObjectByType<EnemyManager>();
        if (enemyManager == null)
            Debug.LogError("EnemyManager not found");

        Destroy(gameObject, 10f);
    }

    void Update()
    {
        if (!canSpawn)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer >= initialDelay)
            {
                canSpawn = true;
                timer = 0f;
            }
            return;
        }

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnBombEnemy();
        }
    }

    void SpawnBombEnemy()
    {
        if (enemyManager == null) return;
        if (spawnCount >= maxSpawnCount) return;

        Vector3 spawnPos = transform.position + spawnOffset;
        GameObject bombEnemy = enemyManager.SpawnAuto(4, spawnPos);
        spawnCount++;

        if (isTurncoat && bombEnemy != null)
        {
            EnemyHackable hackable = bombEnemy.GetComponent<EnemyHackable>();
            if (hackable != null)
            {
                hackable.ApplyTurncoat(10f); // 10초 동안 Turncoat 상태
            }
        }

        spawnCount++;
    }

    public void DestroySpawner()
    {
        Destroy(gameObject);
    }
    public void SetTurncoat()
    {
        isTurncoat = true;
    }
}
