using UnityEngine;

[CreateAssetMenu(fileName = "Throwing_Normal_Accessory", menuName = "Accessory/Throwing/NormalAttack")]
public class ThrowingNormalAccessory : AccessoryData
{
    public float damageMult;
    public float speedMult;
    public int maxKnifeCountAdd;
    public float maxRangeMult;
}