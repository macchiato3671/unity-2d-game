using UnityEngine;

[CreateAssetMenu(menuName = "Hack/Skill/StunAll")]
public class HackStunAllSkill : HackSkillData
{
    public override void Execute(EnemyHackable target = null)
    {
        if (target == null) return; // 적 하나 클릭한 뒤에만 발동

        foreach (var enemy in GameObject.FindObjectsOfType<EnemyHackable>())
        {
            enemy.ApplyStun(duration);
        }
        Debug.Log("[HACK] 전체 적 기절 발동");
    }
}
