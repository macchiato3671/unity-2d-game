using UnityEngine;
using System.Collections;

public class GrenadeSkill : MonoBehaviour
{
    public GameObject grenadePrefab;
    public Transform throwPoint;
    public float throwForce = 10f;
    private Difficulty difficulty;

    public void Init(Difficulty diff)
    {
        difficulty = diff;
    }

    void OnEnable()
    {
        StartCoroutine(ThrowGrenade());
    }

    IEnumerator ThrowGrenade()
    {
        yield return new WaitForSeconds(0.5f); // 약간 딜레이 후 던지기

        GameObject grenade = Instantiate(grenadePrefab, throwPoint.position, Quaternion.identity);
        Rigidbody2D rb = grenade.GetComponent<Rigidbody2D>();

        Vector2 throwDirection = (GameManager.inst.player.transform.position - throwPoint.position).normalized;
        rb.AddForce(new Vector2(throwDirection.x, 1f) * throwForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(2f); // 폭발 대기
        GetComponentInParent<BossController>().EndPattern();
    }
}
