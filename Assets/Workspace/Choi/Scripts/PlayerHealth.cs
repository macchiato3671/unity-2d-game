using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private CinemachineBasicMultiChannelPerlin camNoise;
    [Header("HP 설정")]
    public int maxHP = 7;
    public int currentHP;

    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;

    public System.Action<int, int> OnHealthChanged; // (현재HP, 최대HP)
    public System.Action OnPlayerDie;

    float defaultFixedDeltaTime;

    [SerializeField] private AudioClip hitSound;


    public void Init()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        if(hitSound) SoundManager.inst.PlaySFX(hitSound);

        Debug.Log($"플레이어 피해: -{amount} → 현재 HP: {currentHP}/{maxHP}");

        OnHealthChanged?.Invoke(currentHP, maxHP);

        StartCoroutine(CameraShake());

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
        OnHealthChanged?.Invoke(currentHP, maxHP);
    }

    private void Die()
    {
        Debug.Log("플레이어 사망!");
        OnPlayerDie?.Invoke();
        // TODO: 사망 애니메이션, 리스폰 등 처리
        GameManager.inst.AddDeathPercent(0.25f);
    }

    // timescale 느려지는 버그때문에 timescale 건드리는 건 제거했습니다
    IEnumerator CameraShake(){
        camNoise.AmplitudeGain = 3.5f;

        yield return new WaitForSeconds(0.2f);

        camNoise.AmplitudeGain = 0f;
    }
}
