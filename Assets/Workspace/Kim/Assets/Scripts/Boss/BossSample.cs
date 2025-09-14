using UnityEngine;
using System.Collections;

public class BossSample : MonoBehaviour
{
    public float moveSpeed = 40f;
    public float dropDelay = 0.5f;
    public float minX = 40f;
    public float maxX = 75f;

    float fixedY;
    bool dropFinished;

    private void Start()
    {
        fixedY = 5f;
        StartCoroutine(MoveAndDrop());
        dropFinished = false;
    }

    void Update() {
        // 폭탄 떨구기 완수하면 다시 실행
        if(dropFinished) {
            dropFinished = false;
            StartCoroutine(MoveAndDrop());
        }
    }

    IEnumerator MoveAndDrop()
    {
        // 맵 좌우 랜덤 좌표 설정
        float targetX = Random.Range(minX, maxX);
        Vector3 targetPos = new Vector3(targetX, fixedY, transform.position.z);

        // 해당 위치로 이동
        while (Vector2.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // 도착 후 딜레이
        yield return new WaitForSeconds(dropDelay);

        // 폭탄 투하
        DropBomb();

        // 이동 코루틴 가능하게
        dropFinished = true;
    }

    void DropBomb()
    {
        GameObject bomb = GameManager.inst.pool.GetRange(5); // bomb은 pool element 5번
        if (bomb != null)
        {
            bomb.transform.position = transform.position + Vector3.down * 0.5f; // 약간 아래에서 투하
            bomb.SetActive(true);
        }
    }
}