using UnityEngine;

public class WorldUI : MonoBehaviour
{
    void Awake()
    {
        if (GameManager.inst != null) GameManager.inst.worldUI = this;
    }

    public EnemyHealthUI GetEnemyUI(GameObject enemy)
    {
        EnemyHealthUI newEnemyUI = GameManager.inst.pool.GetUI(0).GetComponent<EnemyHealthUI>();
        newEnemyUI.gameObject.SetActive(true);
        newEnemyUI.transform.SetParent(transform, false);
        newEnemyUI.Init(enemy);

        return newEnemyUI;
    }

    public void GetDamageUI(Vector3 position, int damage)
    {
        DamageUI newDamageUI = GameManager.inst.pool.GetUI(1).GetComponent<DamageUI>();
        newDamageUI.gameObject.SetActive(true);
        newDamageUI.transform.SetParent(transform, false);
        newDamageUI.GetComponent<RectTransform>().position = position;
        newDamageUI.Init(damage);
    }
}
