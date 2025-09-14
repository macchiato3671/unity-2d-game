using UnityEngine;

[CreateAssetMenu(fileName = "MeleeWeaponData", menuName = "Weapon/Melee")]
public class MeleeWeaponData : WeaponData
{
    [Header("히트박스 크기")]
    public float hitboxWidth = 3.0f;
    public float hitboxHeight = 2.0f;

    public override void GenerateDescription()
    {
        float finalDamage = baseDamage;
        float finalWidth = hitboxWidth;
        float finalHeight = hitboxHeight;

        if (accessoryData1 is MeleeNormalAccessory acc1)
        {
            finalDamage = Mathf.RoundToInt(baseDamage * acc1.damageMult);
            finalWidth *= acc1.hitboxWidthMult;
            finalHeight *= acc1.hitboxHeightMult;
        }
        if (accessoryData2 is MeleeNormalAccessory acc2)
        {
            finalDamage = Mathf.RoundToInt(baseDamage * acc2.damageMult);
            finalWidth *= acc2.hitboxWidthMult;
            finalHeight *= acc2.hitboxHeightMult;
        }

        description = $"{weaponName}\n" +
                    $"- 전방에 칼을 휘둘러 {finalWidth:F1} × {finalHeight:F1} 범위 내의 적에게 {finalDamage}의 피해를 줍니다.\n";
    }
}
