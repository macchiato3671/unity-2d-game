using UnityEngine;
using System.Collections;

public class ReflectSkillPattern : MonoBehaviour
{
    public BossController bossController;
    public float reflectChance = 0.3f;
    private bool isHardMode = false;

    public void Init(Difficulty difficulty)
    {
        if (difficulty == Difficulty.Hard)
        {
            reflectChance = 0.7f;
            isHardMode = true;
        }
        else
        {
            reflectChance = 0.3f;
            isHardMode = false;
        }
    }

    void OnEnable()
    {
        StartCoroutine(ReflectRoutine());
    }

    IEnumerator ReflectRoutine()
    {
        Debug.Log($"Reflect Skill 시작 | HardMode: {isHardMode}");

        yield return new WaitForSeconds(2f); // 지속 시간

        bossController.EndPattern();
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }
}
