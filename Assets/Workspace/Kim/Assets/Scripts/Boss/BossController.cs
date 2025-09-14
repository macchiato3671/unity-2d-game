using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("패턴 프리셋")]
    public MonoBehaviour[] skillSetA; // 100, 102번 보스용 (여러 패턴 OK)
    public MonoBehaviour[] skillSetB; // 101, 103번 보스용 (2개 스킬만 교대)

    [Header("현재 사용중인 패턴")]
    private MonoBehaviour[] patterns;

    private MonoBehaviour currentPattern;
    private int currentPatternIndex = 0;
    private float patternDelay = 3f;

    public Difficulty currentDifficulty = Difficulty.Easy;

    private int bossID;

    void Start()
    {
        bossID = GetComponent<Enemy>().enemyID;

        // 시작하자마자 보스 하위 모든 MonoBehaviour 스킬 끄기
        foreach (var skill in GetComponents<MonoBehaviour>())
        {
            // Enemy나 BossController 자기 자신은 끄면 안되니까 제외
            if (skill != this && !(skill is Enemy))
            {
                skill.enabled = false;
            }
        }

        if (bossID == 10 || bossID == 100 || bossID == 102)
        {
            patterns = skillSetA;
        }
        else if (bossID == 101 || bossID == 103 || bossID == 104)
        {
            patterns = skillSetB;
        }
        else
        {
            Debug.LogWarning($"알 수 없는 Boss ID: {bossID}, 기본 스킬 세트 사용");
            patterns = skillSetA;
        }

        StartCoroutine(StartFirstPatternAfterDelay(3f));
    }


    private void DisableSkillSet(MonoBehaviour[] skillSet)
    {
        if (skillSet == null) return;

        foreach (var skill in skillSet)
        {
            if (skill != null)
                skill.enabled = false;
        }
    }

    IEnumerator StartFirstPatternAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PickNextPattern();
    }

    public void PickNextPattern()
    {
        if (currentPattern != null)
        {
            Debug.Log($"[BossController] 비활성화: {currentPattern.GetType().Name}");
            currentPattern.enabled = false;
        }

        currentPattern = patterns[currentPatternIndex];


        Debug.Log($"[BossController] 스킬 사용: {currentPattern.GetType().Name}");

        var initMethod = currentPattern.GetType().GetMethod("Init");
        if (initMethod != null)
        {
            initMethod.Invoke(currentPattern, new object[] { currentDifficulty });
        }

        currentPattern.enabled = true;

        currentPatternIndex = (currentPatternIndex + 1) % patterns.Length;
    }

    public void EndPattern()
    {
        Debug.Log($"[BossController] 패턴 종료됨: {currentPattern.GetType().Name}");
        StartCoroutine(WaitAndPickNextPattern());
    }


    IEnumerator WaitAndPickNextPattern()
    {
        yield return new WaitForSeconds(patternDelay); // 패턴 간 대기
        PickNextPattern();
    }
}
