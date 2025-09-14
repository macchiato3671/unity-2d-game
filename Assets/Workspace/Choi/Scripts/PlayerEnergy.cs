using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEnergy : MonoBehaviour
{
    public int maxEnergy = 100;
    public int currentEnergy = 100;
    public float regenInterval = 1f; // 몇 초마다 회복
    public int regenAmount = 10;

    [Header("UI")]
    public List<Image> energyUnits = new List<Image>(); // 에너지 칸들
    public Sprite fullSprite;
    public Sprite emptySprite;

    private float regenTimer = 0f;

    void Update()
    {
        regenTimer += Time.deltaTime;
        if (regenTimer >= regenInterval)
        {
            regenTimer = 0f;
            RestoreEnergy(regenAmount);
        }

        UpdateUI();
    }

    public bool Consume(int amount)
    {
        if (currentEnergy >= amount)
        {
            currentEnergy -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    public void RestoreEnergy(int amount)
    {
        currentEnergy = Mathf.Min(currentEnergy + amount, maxEnergy);
    }

    private void UpdateUI()
    {
        int totalUnits = maxEnergy / 10;
        int activeUnits = currentEnergy / 10;

        for (int i = 0; i < energyUnits.Count; i++)
        {
            energyUnits[i].sprite = (i < activeUnits) ? fullSprite : emptySprite;
        }
    }
}
