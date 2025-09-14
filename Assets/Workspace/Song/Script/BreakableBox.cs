using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class BreakableBox : MonoBehaviour
{
    [SerializeField] private Collider2D coll;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private GameObject boxParticle;
    [SerializeField] private GameObject beforeEvent;
    [SerializeField] private GameObject eventObj;
    SpriteRenderer sprite;

    int currentHealth = 5;

    void Awake(){
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.V)) Damaged(1); // 테스트   
    }

    public void Damaged(int damage) // 데미지값 무시
    {
        currentHealth -= 1;
        boxParticle.GetComponent<Animator>().SetTrigger("Start");
        if (currentHealth <= 0)
        {
            coll.enabled = false;
            beforeEvent.SetActive(false);
            eventObj.SetActive(true);
            return;
        }

        sprite.sprite = sprites[5 - currentHealth];
    }
}
