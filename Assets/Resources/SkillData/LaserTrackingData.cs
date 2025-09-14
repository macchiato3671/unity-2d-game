using UnityEngine;

[CreateAssetMenu(fileName = "LaserTrackingData", menuName = "Skills/LaserTracking")]
public class LaserTrackingData : SkillData
{
    [Header("스킬 정보")]
    public int trackEnemyNum = 3;
    public float trackingRange = 5.0f;
    public float buffDuration = 5.0f;

    public override void GenerateDescription(WeaponData weapon)
    {
        float finalDuration = buffDuration;
        float finalRange = trackingRange;
        int finalTrackNum = trackEnemyNum;

        if (weapon.accessoryData1 is LaserQAccessory acc1)
        {
            finalDuration *= acc1.buffDurationMult;
            finalRange *= acc1.trackingRangeMult;
            finalTrackNum += acc1.trackEnemyNumAdd;
        }
        if (weapon.accessoryData2 is LaserQAccessory acc2)
        {
            finalDuration *= acc2.buffDurationMult;
            finalRange *= acc2.trackingRangeMult;
            finalTrackNum += acc2.trackEnemyNumAdd;
        }

        description = $"{skillName}\n" +
                    $"- {finalDuration:F1}초 동안 발사하는 레이저가 적을 자동으로 추적합니다.\n" +
                    $"- 첫 적중 지점을 기준으로 반경 {finalRange:F1}m 내의 적 {finalTrackNum}명을 추가로 꿰뚫습니다.\n" +
                    $"- 쿨타임: {cooldown:F1}초";
    }
}
