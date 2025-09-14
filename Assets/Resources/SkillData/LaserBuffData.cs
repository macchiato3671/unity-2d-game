using UnityEngine;

[CreateAssetMenu(fileName = "LaserBuffData", menuName = "Skills/LaserBuff")]
public class LaserBuffData : SkillData
{
    [Header("스킬 정보")]
    public float attackSpeedRate = 0.5f;
    public float buffDuration = 5.0f;

    public override void GenerateDescription(WeaponData weapon)
    {
        float finalBuffDuration = buffDuration;
        float finalAttackSpeedRate = attackSpeedRate;

        if (weapon.accessoryData1 is LaserEAccessory acc1)
        {
            finalBuffDuration *= acc1.buffDurationMult;
            finalAttackSpeedRate *= acc1.attackSpeedRateMult;
        }
        if (weapon.accessoryData2 is LaserEAccessory acc2)
        {
            finalBuffDuration *= acc2.buffDurationMult;
            finalAttackSpeedRate *= acc2.attackSpeedRateMult;
        }

        float percent = finalAttackSpeedRate * 100f;

        description = $"{skillName}\n" +
                    $"- {finalBuffDuration:F1}초 동안 공격 속도가 {percent:F0}% 빨라집니다.\n" +
                    $"- 쿨타임: {cooldown:F1}초";
    }
}
