using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Object/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("# 설명")]
    public string description;
    [Header("# 주요 값")]
    public int enemyID;
    public float defaultSpeed = 2f; // 기본 이동속도
    public float chaseSpeed = 4f; // 플레이어 추적 이동속도
    public float detectionRange = 5f; // 플레이어 탐지 범위
    public int attackPower = 1; // 플레이어 대상 공격력
    public int maxHealth = 100; // 최대 체력
    public float knockbackForce = 4f; // 피격시 자신이 넉백되는 정도
    public Vector2 size; // Scale 크기

    [Header("# 추가 값")]
    public float attackCooldown = 0f;
    public float bulletSpeed = 0f;
    public Vector3 firePoint;

    [Header("# 애니메이션 및 콜라이더")]
    public RuntimeAnimatorController animationController; // 에너미에 해당하는 애니메이션 컨트롤러
    public Vector2 colliderOffset; // 콜라이더 오프셋
    public Vector2 colliderSize; // 콜라이더 크기

    [Header("# 효과음")]
    public AudioClip[] audioClips;
}
