using UnityEngine;

public abstract class HackSkillData : ScriptableObject
{
    public HackType type;
    public string skillName;
    public bool isget;
    public float duration;
    public Sprite icon;
    public GameObject effect;
    [TextArea]
    public string description;   // 설명 추가
    public int requiredResource;

    public abstract void Execute(EnemyHackable target = null);
}

