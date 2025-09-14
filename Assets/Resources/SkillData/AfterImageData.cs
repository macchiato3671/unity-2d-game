using UnityEngine;

[CreateAssetMenu(fileName = "AfterImageData", menuName = "Skills/AfterImage")]
public class AfterImageData : SkillData
{
    [Header("스킬쓰는는 효과음")]
    public AudioClip buffSFX; //효과음

    [Header("스킬 정보")]
    public int damage = 10;
    public float speed = 30f;
    public float maxDistance = 5f;
    public float buffDuration = 5.0f;

    public override void GenerateDescription(WeaponData weapon)
    {
        int finalDamage = damage;
        float finalSpeed = speed;
        float finalMaxDistance = maxDistance;
        float finalBuffDuration = buffDuration;

        if (weapon.accessoryData1 is MeleeEAccessory acc1)
        {
            finalDamage = Mathf.RoundToInt(damage * acc1.damageMult);
            finalSpeed *= acc1.speedMult;
            finalMaxDistance *= acc1.maxDistanceMult;
            finalBuffDuration *= acc1.buffDurationMult;
        }
        if (weapon.accessoryData2 is MeleeEAccessory acc2)
        {
            finalDamage = Mathf.RoundToInt(damage * acc2.damageMult);
            finalSpeed *= acc2.speedMult;
            finalMaxDistance *= acc2.maxDistanceMult;
            finalBuffDuration *= acc2.buffDurationMult;
        }

        description = $"{skillName}\n" +
                    $"- 스킬 사용 후 일정 시간 동안 기본 공격에 잔상이 생성됩니다.\n" +
                    $"- 잔상은 {finalSpeed} 속도로 최대 {finalMaxDistance}m까지 공격하며,\n" +
                    $"  적에게 {finalDamage}의 추가 피해를 줍니다.\n" +
                    $"- 잔상 효과 지속 시간: {finalBuffDuration:F1}초\n" +
                    $"- 쿨타임: {cooldown:F1}초";
    }
}
