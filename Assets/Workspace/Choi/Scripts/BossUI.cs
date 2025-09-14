using UnityEngine;

public class BossUI : MonoBehaviour
{
    void Start()
    {
        if (GameManager.inst != null) GameManager.inst.bossUI = this;
    }
    public BossHealthUI GetBossUI(GameObject enemy, string bossName, int maxHp)
    {
        BossHealthUI newBossUI = GameManager.inst.pool.GetUI(2).GetComponent<BossHealthUI>();
        newBossUI.transform.SetParent(transform, false);
        newBossUI.Init(enemy, bossName);
        return newBossUI;
    }
}
