using UnityEngine;

[CreateAssetMenu(menuName = "Hack/Skill/Lure")]
public class HackLureSkill : HackSkillData
{
    public override void Execute(EnemyHackable target)
    {
        if (target != null)
            target.ApplyLure(duration);
    }
}