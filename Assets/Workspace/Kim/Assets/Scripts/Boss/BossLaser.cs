using UnityEngine;

public class BossLaser : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerMove player = other.GetComponent<PlayerMove>();
            if (player != null)
            {
                player.OnDamaged(Vector2.zero);
            }
        }
    }
}
