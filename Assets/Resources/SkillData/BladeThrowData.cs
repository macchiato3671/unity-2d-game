using UnityEngine;

[CreateAssetMenu(fileName = "BladeThrowData", menuName = "Skills/BladeThrow")]
public class BladeThrowData : SkillData
{
    [Header("스킬 정보")]
    public float spreadAngle = 30f;

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
                    $"- 전방 {spreadAngle:F0}° 범위로 칼을 동시에 던집니다.\n" +
                    $"- 각 칼은 적을 관통하며 {finalDamage}의 피해를 입힙니다.\n" +
                    $"- 사용 시 남아 있는 칼 수만큼만 던져지며, 이후 스킬로 회수해야 재사용할 수 있습니다.\n" +
                    $"- 쿨타임: {cooldown:F1}초";
    }
}
