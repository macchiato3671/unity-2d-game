using UnityEngine;

[CreateAssetMenu(fileName = "LaserWeaponData", menuName = "Weapon/Laser")]
public class LaserWeaponData : WeaponData
{
    [Header("레이저 정보")]
    public float laserLength = 15f;
    public float lifeTime = 0.1f;

    public override void GenerateDescription()
    {
        float finalLength = laserLength;
        float finalLifeTime = lifeTime;
        int finalDamage = baseDamage;

        if (accessoryData1 is LaserNormalAccessory acc1)
        {
            finalLength *= acc1.laserLengthMult;
            finalLifeTime *= acc1.lifeTimeMult;
            finalDamage = Mathf.RoundToInt(finalDamage * acc1.damageMult);
        }
        if (accessoryData2 is LaserNormalAccessory acc2)
        {
            finalLength *= acc2.laserLengthMult;
            finalLifeTime *= acc2.lifeTimeMult;
            finalDamage = Mathf.RoundToInt(finalDamage * acc2.damageMult);
        }

        description = $"{weaponName}\n" +
                    $"- 마우스 방향으로 {finalLength:F1}m 길이의 레이저를 발사합니다.\n" +
                    $"- 레이저는 {finalLifeTime:F2}초 동안 유지되며, 이 시간 동안 적에게 초당 {finalDamage}의 피해를 줍니다.";
    }
}
