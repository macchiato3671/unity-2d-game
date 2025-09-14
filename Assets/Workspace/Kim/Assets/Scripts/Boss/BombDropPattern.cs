using UnityEngine;
using System.Collections;

public class BombDropPattern : MonoBehaviour
{
    public BossController boss;
    float moveSpeed;
    float dropDelay;
    public float minX = 40f;
    public float maxX = 75f;
    public float patternDuration = 10f; // 몇 초 동안 반복할지

    public void Init(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                moveSpeed = 25f;
                dropDelay = 0.8f;
                break;

            case Difficulty.Hard:
                moveSpeed = 40f;
                dropDelay = 0.5f;
                break;
        }
    }

    void OnEnable()
    {
        Debug.Log("Drop Bomb Pattern");
        StartCoroutine(RepeatDropForSeconds());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator RepeatDropForSeconds()
    {
        float elapsed = 0f;

        while (elapsed < patternDuration)
        {
            float startTime = Time.time;

            // 이동 후 폭탄 떨구기
            yield return StartCoroutine(MoveAndDrop());

            // 다음 반복까지 소요된 시간 계산
            elapsed += Time.time - startTime;
        }

        boss.EndPattern();
    }

    void DropBomb()
    {
        GameObject bomb = GameManager.inst.pool.GetRange(11);
        if (bomb != null)
        {
            bomb.transform.position = transform.position + Vector3.down * 0.5f;
            bomb.SetActive(true);
        }
    }

    IEnumerator MoveAndDrop()
    {
        float fixedY = 5f;

        // 플레이어 위치 가져오기
        Vector3 playerPos = GameManager.inst.player.transform.position;

        // 플레이어 주변 랜덤 오프셋 (조금 앞뒤로 움직이게)
        float offsetX = Random.Range(-3f, 3f); // -3~+3 범위 안에서 플레이어 주변 랜덤
        float targetX = Mathf.Clamp(playerPos.x + offsetX, minX, maxX); // 맵 범위 넘지 않게 Clamp

        Vector3 targetPos = new Vector3(targetX, fixedY, boss.transform.position.z);

        // 이동
        while (Vector2.Distance(boss.transform.position, targetPos) > 0.1f)
        {
            boss.transform.position = Vector2.MoveTowards(boss.transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // 도착 후 약간 기다리고 폭탄 떨구기
        yield return new WaitForSeconds(dropDelay);
        DropBomb();
    }

}