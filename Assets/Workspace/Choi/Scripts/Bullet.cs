using System.Collections;
using UnityEngine;

public class Bullet2 : MonoBehaviour
{
    public float lifeTime = 3f;   // 자동 제거 타이머
    public int damage = 1;        // 총알이 입히는 데미지

    private void Start()
    {
        StartCoroutine(Remove());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player2 player = collision.GetComponent<Player2>();
            if (player != null && player.Health != null)
            {
                player.Health.TakeDamage(damage);
            }

            Debug.Log("Player Hit by Bullet!");
            gameObject.SetActive(false);
        }

        // 벽이나 다른 것에 부딪혀도 제거 가능
        else if (collision.CompareTag("Wall"))
        {
            gameObject.SetActive(false);
        }
    }

    IEnumerator Remove()
    {
        yield return new WaitForSeconds(lifeTime);
        gameObject.SetActive(false);
    }
}
