using UnityEngine;
using System.Collections;

public class SpawnEnemyPattern : MonoBehaviour
{
    public BossController boss;
    public GameObject spawnerPrefab;
    public float patternDuration = 10f;
    int xMin = 30;
    int xMax = 55;
    int yMin = 0;
    int yMax = 3;

    void OnEnable()
    {
        StartCoroutine(RunSpawnPattern());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator RunSpawnPattern()
    {
        Debug.Log("Spawn Pattern Start");
        float bossY = boss.transform.position.y;
        float bossX = boss.transform.position.x; // 위치 고정 시 참고

        float randX = Random.Range(xMin, xMax);
        float randY = Random.Range(yMin, yMax);
        Vector3 spawnPos = new Vector3(randX, randY, 0f);

        Debug.Log($"Spawner 위치: {spawnPos}");

        var spawner = Instantiate(spawnerPrefab, spawnPos, Quaternion.identity);
        spawner.transform.localScale = Vector3.one * 2f;

        yield return new WaitForSeconds(patternDuration);
        boss.EndPattern();
    }
}