using UnityEngine;
using System.Collections;

public class LaserPattern : MonoBehaviour
{
    public BossController boss;
    public float enterXLeft = 42f;
    public float enterXRight = 70f;
    public int bossY = 2;
    public int yMin = -3;
    public int yMax = 5;
    int laserCount;
    float warningDuration;
    float attackDuration;
    
    public void Init(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                laserCount = 1;
                warningDuration = 2.5f;
                attackDuration = 2.5f;
                break;

            case Difficulty.Hard:
                laserCount = 2;
                warningDuration = 1.5f;
                attackDuration = 3.5f;
                break;
        }
    }

    void OnEnable()
    {
        Debug.Log("Laser Pattern");
        StartCoroutine(FireLaserTwice());
    }

    IEnumerator FireLaserTwice()
    {
        int repeatCount = 2;
        bool fromLeft = Random.value < 0.5f;

        // 패턴 시작 시 두 번 반복
        for (int i = 0; i < repeatCount; i++)
        {
            yield return StartCoroutine(FireLaser(fromLeft));

            // 반복 사이 간격
            yield return new WaitForSeconds(1f);
        }

        boss.EndPattern();
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    // IEnumerator FireLaser(bool fromLeft)
    // {
    //     float startX = fromLeft ? enterXLeft : enterXRight;
    //     int y1 = Random.Range((int)yMin, (int)yMax + 1);
    //     int y2;
    //     do
    //     {
    //         y2 = Random.Range((int)yMin, (int)yMax + 1);
    //     } while (y2 == y1);
    //     float laserY1 = (float)y1;
    //     float laserY2 = (float)y2;
    //     boss.transform.position = new Vector3(startX, bossY, 0f);

    //     Vector2 dir = fromLeft ? Vector2.right : Vector2.left;
    //     float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    //     float range = 25f;
    //     float width = 1f;

    //     // LineRenderer 경고선 표시
    //     GameObject warning1 = CreateWarningLine(startX, laserY1, dir, range, width);
    //     GameObject warning2 = CreateWarningLine(startX, laserY2, dir, range, width);

    //     yield return new WaitForSeconds(2f);

    //     Destroy(warning1);
    //     Destroy(warning2);

    //     // 레이저 발사
    //     GameObject laser1 = FireLaserAt(startX, laserY1, angle, dir, range, width);
    //     GameObject laser2 = FireLaserAt(startX, laserY2, angle, dir, range, width);

    //     yield return new WaitForSeconds(attackDuration);

    //     if (laser1 != null) laser1.SetActive(false);
    //     if (laser2 != null) laser2.SetActive(false);
    // }

    IEnumerator FireLaser(bool fromLeft)
    {
        float startX = fromLeft ? enterXLeft : enterXRight;
        boss.transform.position = new Vector3(startX, bossY, 0f);
        Vector2 dir = fromLeft ? Vector2.right : Vector2.left;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float range = 25f;
        float width = 1f;

        int[] yValues = new int[laserCount];
        for (int i = 0; i < laserCount; i++)
        {
            int y;
            do {
                y = Random.Range(yMin, yMax + 1);
            } while (System.Array.Exists(yValues, val => val == y));
            yValues[i] = y;
        }

        GameObject[] warnings = new GameObject[laserCount];
        for (int i = 0; i < laserCount; i++)
        {
            float y = yValues[i];
            warnings[i] = CreateWarningLine(startX, y, dir, range, width);
        }

        yield return new WaitForSeconds(warningDuration);

        foreach (var w in warnings) Destroy(w);

        GameObject[] lasers = new GameObject[laserCount];
        for (int i = 0; i < laserCount; i++)
        {
            float y = yValues[i];
            lasers[i] = FireLaserAt(startX, y, angle, dir, range, width);
        }

        yield return new WaitForSeconds(attackDuration);

        foreach (var laser in lasers)
            if (laser != null) laser.SetActive(false);
    }

    // 경고선 표시
    GameObject CreateWarningLine(float startX, float y, Vector2 dir, float range, float width)
    {
        GameObject obj = new GameObject("LaserWarning");
        LineRenderer line = obj.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = new Color(1, 0, 0, 0.5f);
        line.endColor = new Color(1, 0, 0, 0.5f);
        line.startWidth = width;
        line.endWidth = width;
        line.useWorldSpace = true;
        line.positionCount = 2;

        Vector3 start = new Vector3(startX, y, 0f);
        Vector3 end = start + (Vector3)(dir * range);

        line.SetPosition(0, start);
        line.SetPosition(1, end);
        return obj;
    }

    // 레이저 발사
    GameObject FireLaserAt(float startX, float y, float angle, Vector2 dir, float range, float width)
    {
        GameObject laser = GameManager.inst.pool.GetRange(12);
        if (laser != null)
        {
            laser.transform.position = new Vector3(startX, y, 0f);
            laser.transform.rotation = Quaternion.Euler(0, 0, angle);

            SpriteRenderer sr = laser.GetComponent<SpriteRenderer>();
            BoxCollider2D col = laser.GetComponent<BoxCollider2D>();

            if (sr != null && sr.sprite != null && col != null)
            {
                Vector2 spriteSize = sr.sprite.bounds.size;
                float scaleX = range / spriteSize.x;
                float scaleY = width / spriteSize.y;

                laser.transform.localScale = new Vector3(scaleX, scaleY, 1f);
                col.size = spriteSize;
                col.offset = new Vector2(spriteSize.x / 2f, 0f);
            }

            laser.SetActive(true);
        }

        return laser;
    }
}