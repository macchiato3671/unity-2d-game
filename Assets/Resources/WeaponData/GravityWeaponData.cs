using UnityEngine;

[CreateAssetMenu(fileName = "GravityWeaponData", menuName = "Weapon/Gravity")]
public class GravityWeaponData : WeaponData
{
    [Header("끌어당기는 효과음")]
    public AudioClip pullSFX; //효과음

    [Header("끌어당김")]
    public float pullRadius = 5f;
    public float pullForce = 0.1f;
    public float duration = 2f;

    [Header("틱 데미지")]
    public float totalTickDamage = 10f;
    public float tickInterval = 0.2f;

    [Header("기타")]
    public float maxLifetime = 10f;

    public override void GenerateDescription()
    {
        float finalPullRadius = pullRadius;
        float finalPullForce = pullForce;
        float finalDuration = duration;
        float finalTickDamage = totalTickDamage;

        if (accessoryData1 is GravityNormalAccessory acc1)
        {
            finalPullRadius *= acc1.pullRadiusMult;
            finalPullForce *= acc1.pullForceMult;
            finalDuration *= acc1.durationMult;
            finalTickDamage *= acc1.totalTickDamageMult;
        }
        if (accessoryData2 is GravityNormalAccessory acc2)
        {
            finalPullRadius *= acc2.pullRadiusMult;
            finalPullForce *= acc2.pullForceMult;
            finalDuration *= acc2.durationMult;
            finalTickDamage *= acc2.totalTickDamageMult;
        }

        description = $"{weaponName}\n" +
            $"- 플랫폼에 부착된 중력자탄은 {finalPullRadius:F1}m 내 적을 {finalDuration:F1}초간 {finalPullForce:F1} 힘으로 끌어당깁니다.\n" +
            $"- {tickInterval:F1}초마다 피해를 주며, 총 {finalTickDamage}의 틱 데미지를 줍니다.";
    }
}
