using UnityEngine;

[CreateAssetMenu(fileName = "RecallBladeData", menuName = "Skills/RecallBlade")]
public class RecallBladeData : SkillData
{
    public override void GenerateDescription(WeaponData weapon)
    {
        int finalDamage = weapon.baseDamage;

        if (weapon.accessoryData1 is ThrowingNormalAccessory acc1)
        {
            finalDamage = Mathf.RoundToInt(finalDamage * acc1.damageMult);
        }
        if (weapon.accessoryData2 is ThrowingNormalAccessory acc2)
        {
            finalDamage = Mathf.RoundToInt(finalDamage * acc2.damageMult);
        }

        description = $"{skillName}\n" +
                    $"- 맵에 남아있는 모든 칼을 회수하여 탄창을 즉시 복구합니다.\n" +
                    $"- 회수되는 칼은 경로상의 적에게 {finalDamage}의 피해를 입힙니다.\n" +
                    $"- 쿨타임: {cooldown:F1}초";
    }
}
