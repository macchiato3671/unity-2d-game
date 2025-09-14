using UnityEngine;

[CreateAssetMenu(fileName = "GravLaserData", menuName = "Skills/GravLaser")]
public class GravLaserData : SkillData
{
    [Header("스킬 정보")]
    public float damage = 50f;
    public float width = 0.3f;
    public float range = 20f;
    public float duration = 1.5f;

    public override void GenerateDescription(WeaponData weapon)
    {
        float finalDamage = damage;
        float finalWidth = width;
        float finalRange = range;
        float finalDuration = duration;
        float finalCooldown = cooldown;

        if (weapon.accessoryData1 is GravityEAccessory acc1)
        {
            finalDamage = Mathf.RoundToInt(finalDamage * acc1.damageMult);
            finalWidth *= acc1.widthMult;
            finalRange *= acc1.rangeMult;
        }
        if (weapon.accessoryData2 is GravityEAccessory acc2)
        {
            finalDamage = Mathf.RoundToInt(finalDamage * acc2.damageMult);
            finalWidth *= acc2.widthMult;
            finalRange *= acc2.rangeMult;
        }

        description = $"{skillName}\n" +
                    $"- 전방으로 폭 {finalWidth:F1}m, 사거리 {finalRange:F0}m의 강력한 레이저를 발사합니다.\n" +
                    $"- 레이저는 {finalDuration:F1}초 동안 유지되며, 지나가는 적에게\n" +
                    $"  지속적으로 총 {finalDamage}의 피해를 줍니다.\n" +
                    $"- 쿨타임: {finalCooldown:F1}초";
    }
}
