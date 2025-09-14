using UnityEngine;

public class EnemyExplosion : MonoBehaviour
{
    [SerializeField] private Animator anim;

    void FixedUpdate(){
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if(info.normalizedTime >= 1f) gameObject.SetActive(false);
    }
}
