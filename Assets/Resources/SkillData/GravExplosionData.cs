using UnityEngine;

[CreateAssetMenu(fileName = "GravExplosionData", menuName = "Skills/GravExplosion")]
public class GravExplosionData : SkillData
{
    [Header("스킬 정보")]
    public float explosionRadius = 3f;
    public int explosionDamage = 10;

    public override void GenerateDescription(WeaponData weapon)
    {
        float finalRadius = explosionRadius;
        int finalDamage = explosionDamage;

        if (weapon.accessoryData1 is GravityQAccessory acc1)
        {
            finalRadius *= acc1.explosionRadiusMult;
            finalDamage = Mathf.RoundToInt(finalDamage * acc1.explosionDamageMult);
        }
        if (weapon.accessoryData2 is GravityQAccessory acc2)
        {
            finalRadius *= acc2.explosionRadiusMult;
            finalDamage = Mathf.RoundToInt(finalDamage * acc2.explosionDamageMult);
        }

        description = $"{skillName}\n" +
                    $"- 맵에 설치된 중력자탄을 모두 기폭시켜,\n" +
                    $"  반경 {finalRadius:F1}m 내의 적에게 각각 {finalDamage}의 피해를 줍니다.\n" +
                    $"- 여러 개의 중력자탄이 설치되어 있다면 동시에 폭발합니다.\n" +
                    $"- 쿨타임: {cooldown:F1}초";
    }
}
