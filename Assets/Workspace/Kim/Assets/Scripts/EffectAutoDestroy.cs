using UnityEngine;
using System.Collections;

public class EffectAutoDestroy : MonoBehaviour
{
    public void Deactivate()
    {
        gameObject.SetActive(false); // 애니메이션 이벤트에서 호출
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void InitFollowAndDestroy(Transform followTarget, float duration)
    {
        transform.SetParent(followTarget); // 적에 붙이기
        StartCoroutine(DestroyAfter(duration));
    }

    IEnumerator DestroyAfter(float time)
    {
        yield return new WaitForSeconds(time);
        transform.SetParent(null); // 부모 제거
        gameObject.SetActive(false);
    }
}