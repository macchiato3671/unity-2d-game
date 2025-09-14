using UnityEngine;

[CreateAssetMenu(fileName = "AccessoryData", menuName = "Accessory/AccessoryData")]
public class AccessoryData : ScriptableObject
{
    [Header("기본 정보")]
    public string accessoryName;
    public int accessoryID;
    public WeaponType compatibleWeapon;
    public SkillSlot slotType;
    public bool isget;

    [Header("아이콘")]
    public Sprite icon;

    [TextArea]
    public string description;
}
