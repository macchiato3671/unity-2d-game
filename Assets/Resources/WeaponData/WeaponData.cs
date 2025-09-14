using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapon/WeaponData")]
public abstract class WeaponData : ScriptableObject
{
    [Header("기본 정보")]
    public int weaponID;
    public string weaponName;
    public bool isget;
    public int requiredResource;
    [TextArea]
    public string description;       // 무기 설명
    public Sprite icon;              // 인벤토리/UI 아이콘
    public WeaponType type;

    [Header("공통 스탯")]
    public int baseDamage;           // 기본 데미지
    public float attackSpeed;        // 공격 속도

    [Header("스킬")]
    public SkillData qSkillData;         // 스킬 Q
    public SkillData eSkillData;         // 스킬 E

    [Header("착용 장신구")]
    public AccessoryData accessoryData1; // 착용 장신구1
    public AccessoryData accessoryData2; // 착용 장신구2

    [Header("효과음")]
    public AudioClip attackSFX; //효과음

    public abstract void GenerateDescription();
}