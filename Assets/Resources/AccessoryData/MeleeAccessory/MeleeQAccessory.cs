using UnityEngine;

[CreateAssetMenu(fileName = "Melee_Q_Accessory", menuName = "Accessory/Melee/Q")]
public class MeleeQAccessory : AccessoryData
{ 
    public float damageMult;
    public float slowDurationMult;
    public float slowAmountMult;
    public float maxDistanceMult;
}