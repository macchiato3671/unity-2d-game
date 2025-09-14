using UnityEngine;

[CreateAssetMenu(fileName = "EMPSlashData", menuName = "Skills/EMPSlash")]
public class EMPSlashData : SkillData
{
    [Header("스킬 정보")]
    public int damage = 10;
    public float slowDuration = 1f;
    public float slowAmount = 0.8f;
    public float maxDistance = 15f;

    public override void GenerateDescription(WeaponData weapon)
    {
        int finalDamage = damage;
        float finalMaxDistance = maxDistance;
        float finalSlowAmount = slowAmount;
        float finalSlowDuration = slowDuration;

        if (weapon.accessoryData1 is MeleeQAccessory acc1)
        {
            finalDamage = Mathf.RoundToInt(damage * acc1.damageMult);
            finalMaxDistance *= acc1.maxDistanceMult;
            finalSlowAmount *= acc1.slowAmountMult;
            finalSlowDuration *= acc1.slowDurationMult;
        }
        if (weapon.accessoryData2 is MeleeQAccessory acc2)
        {
            finalDamage = Mathf.RoundToInt(damage * acc2.damageMult);
            finalMaxDistance *= acc2.maxDistanceMult;
            finalSlowAmount *= acc2.slowAmountMult;
            finalSlowDuration *= acc2.slowDurationMult;
        }

        float slowPercent = (1f - finalSlowAmount) * 100f;

        description = $"{skillName}\n" +
                    $"- 전방 최대 {finalMaxDistance:F1}m까지 EMP Slash를 날립니다.\n" +
                    $"- 적중한 적에게 {finalDamage}의 피해를 입히고,\n" +
                    $"  이동속도를 {slowPercent:F0}% 감소시킵니다.\n" +
                    $"- 감속 지속 시간: {finalSlowDuration:F1}초\n" +
                    $"- 쿨타임: {cooldown:F1}초";
    }
}
