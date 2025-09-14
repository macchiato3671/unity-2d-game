using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float airHitDamage = 15f;
    public float groundExplosionDamage = 15f;
    public LayerMask playerLayer;
    public float explosionRadius = 3f;
    private bool hasExploded = false;

    void Awake()
    {
        playerLayer = LayerMask.GetMask("Player");
        transform.localScale = new Vector3(2f, 2f, 1f);
    }

    void OnEnable()
    {
        hasExploded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasExploded) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            // 공중에서 부딪힌 경우
            // PlayerMove player = collision.gameObject.GetComponent<PlayerMove>();
            // if (player != null) {
            //     player.OnDamaged(transform.position);
            // }
            Explode();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            // Layer가 Platform일 경우에만 폭발
            Explode();
        }
    }

    void Explode()
    {
        hasExploded = true;

        GameObject effect = GameManager.inst.pool.GetRange(9);
        if (effect != null) {
            effect.transform.position = transform.position;

            float baseRadius = 1f;
            float scale = explosionRadius / baseRadius;
            effect.transform.localScale = new Vector3(scale, scale, 1f);

            effect.SetActive(true);
        }

        // 주변 범위 데미지
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, playerLayer);
        foreach (var hit in hits)
        {
            // 풀레이어 데미지
            PlayerMove player = hit.GetComponent<PlayerMove>();
            if (player != null)
            {
                Debug.Log("explosion damage!");
                player.OnDamaged(transform.position);
            }
        }

        gameObject.SetActive(false);
    }
}