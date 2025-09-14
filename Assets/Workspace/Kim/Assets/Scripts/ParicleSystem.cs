using UnityEngine;

public class ParticleAutoDisable : MonoBehaviour
{
    private ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void OnEnable()
    {
        if (ps != null)
        {
            ps.Clear();   // 이전 입자 제거
            ps.Play();    // 재생
        }
    }

    void Update()
    {
        if (ps != null && !ps.IsAlive())
        {
            gameObject.SetActive(false); // 파티클 끝나면 자동 비활성화 (풀로 되돌리기)
        }
    }
}