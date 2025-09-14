using UnityEngine;

[CreateAssetMenu(menuName = "Hack/Skill/Suicide")]
public class HackSuicideSkill : HackSkillData
{
    public override void Execute(EnemyHackable target)
    {
        if (target != null)
            target.ApplySuicide();
    }
}
