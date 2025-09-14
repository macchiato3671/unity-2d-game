using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthUI : MonoBehaviour
{
    [SerializeField] private TMP_Text bossNameText;
    [SerializeField] private Slider bossHealthSlider;

    private Enemy targetEnemy;

    void OnGUI()
    {
        if (targetEnemy == null) return;
        bossHealthSlider.value = (float)targetEnemy.CurrentHealth / targetEnemy.MaxHealth;
    }

    public void Init(GameObject enemy, string bossName)
    {
        targetEnemy = enemy.GetComponent<Enemy>();
        bossNameText.text = bossName;
    }
}
