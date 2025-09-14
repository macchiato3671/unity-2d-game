using UnityEngine;

[CreateAssetMenu(menuName = "Hack/Skill/Infection")]
public class HackInfectionSkill : HackSkillData
{
    public override void Execute(EnemyHackable target)
    {
        if (target != null)
            target.ApplyInfection();
    }
}