using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 3f; // 투사체가 존재하는 시간  

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Bullet");
    }


    private void OnEnable()
    {
        // 일정 시간이 지나면 투사체 파괴  <- 파괴 처리는 그냥 오브젝트 비활성화 해주시면 됩니다.
        gameObject.layer = LayerMask.NameToLayer("Bullet");
        StartCoroutine(Remove());

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어를 맞췄을 때  
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player Hit!");
            gameObject.SetActive(false);
            // .. (미구현)
        }
    }

    public void Init(Vector2 dir){
        transform.rotation = Quaternion.FromToRotation(Vector2.right,dir);
    }

    IEnumerator Remove(){
        yield return new WaitForSeconds(lifeTime);
        gameObject.SetActive(false);
    }
}