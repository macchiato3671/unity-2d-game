using UnityEngine;

[CreateAssetMenu(fileName = "ThrowingWeaponData", menuName = "Weapon/Throwing")]
public class ThrowingWeaponData : WeaponData
{
    [Header("탄 정보")]
    public int speed = 60;
    public int maxKnifeCount = 4;
    public float maxRange = 12f;

    public override void GenerateDescription()
    {
        float finalRange = maxRange;
        int finalKnifeCount = maxKnifeCount;
        int finalDamage = baseDamage;

        if (accessoryData1 is ThrowingNormalAccessory acc1)
        {
            finalRange *= acc1.maxRangeMult;
            finalKnifeCount += acc1.maxKnifeCountAdd;
            finalDamage = Mathf.RoundToInt(finalDamage * acc1.damageMult);
        }
        if (accessoryData2 is ThrowingNormalAccessory acc2)
        {
            finalRange *= acc2.maxRangeMult;
            finalKnifeCount += acc2.maxKnifeCountAdd;
            finalDamage = Mathf.RoundToInt(finalDamage * acc2.damageMult);
        }

        description = $"{weaponName}\n" +
                    $"- 칼을 던져 적을 관통시키며, {finalRange:F1}m 거리까지 날아갑니다.\n" +
                    $"- 적중 시 {finalDamage}의 피해를 입힙니다.\n" +
                    $"- 동시에 최대 {finalKnifeCount}개의 칼을 사용할 수 있으며,\n" +
                    $"  모두 소모 시 스킬을 사용해 회수해야 다시 사용할 수 있습니다.\n";
    }
}
