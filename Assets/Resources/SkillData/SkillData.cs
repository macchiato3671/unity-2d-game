using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Skills/SkillData")]
public abstract class SkillData : ScriptableObject
{
    public string skillName;
    public Sprite icon;
    public float cooldown; //실제적용
    public float baseCooldown;

    [TextArea]
    public string description;

    [Header("효과음")]
    public AudioClip attackSFX; //효과음

    public virtual void ApplyCooldownModifier(float multiplier)
    {
        cooldown = baseCooldown * multiplier;
    }

    public abstract void GenerateDescription(WeaponData weapon);
}