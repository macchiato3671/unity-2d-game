using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float explosionDelay = 2f;
    public float explosionRadius = 2f;
    public int explosionDamage = 1;
    public GameObject explosionEffectPrefab;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Invoke(nameof(Explode), explosionDelay);
    }

    void Explode()
    {
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(transform.position, explosionRadius, LayerMask.GetMask("Player"));
        foreach (Collider2D target in hitTargets)
        {
            if (target.CompareTag("Player"))
            {
                target.GetComponent<Player>().DealPlayer(explosionDamage);
            }
        }

        Destroy(gameObject); // 수류탄 본체 삭제
    }

    void OnDrawGizmosSelected()
    {
        // 디버그용: 폭발 반경 표시
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
