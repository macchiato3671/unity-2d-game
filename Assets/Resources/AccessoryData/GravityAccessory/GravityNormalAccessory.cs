using UnityEngine;

[CreateAssetMenu(fileName = "Gravity_Normal_Accessory", menuName = "Accessory/Gravity/NormalAttack")]
public class GravityNormalAccessory : AccessoryData
{ 
    public float pullRadiusMult;
    public float pullForceMult;
    public float durationMult;

    public float totalTickDamageMult;
}