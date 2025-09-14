using UnityEngine;

[CreateAssetMenu(menuName = "Hack/Skill/Turncoat")]
public class HackTurncoatSkill : HackSkillData
{
    public override void Execute(EnemyHackable target)
    {
        if (target != null)
            target.ApplyTurncoat(duration);
    }
}