using UnityEngine;

public class AutoDisable : MonoBehaviour
{
    public float lifeTime = 0.2f;

    public GameObject target = null;
    private Vector3 offset;

    public void Init(GameObject followTarget, Vector3 followOffset)
    {
        target = followTarget;
        offset = followOffset;
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            transform.position = target.transform.position + offset;
        }
    }

    private void OnEnable()
    {
        CancelInvoke(); // �ߺ� ����
        Invoke(nameof(Disable), lifeTime);
    }

    void Disable()
    {
        gameObject.SetActive(false);
    }
}