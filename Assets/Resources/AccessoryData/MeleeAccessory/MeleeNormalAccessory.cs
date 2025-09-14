using UnityEngine;

[CreateAssetMenu(fileName = "Melee_Normal_Accessory", menuName = "Accessory/Melee/NormalAttack")]
public class MeleeNormalAccessory : AccessoryData
{
    public float damageMult;
    public float hitboxWidthMult;
    public float hitboxHeightMult;
}