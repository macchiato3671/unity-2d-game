using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectUI : MonoBehaviour
{
    public Image mainSlotImage;
    public Image subSlotImage;

    private int currentWeaponIndex = 0; // 0 or 1

    public void UpdateUI(int weaponIdx)
    {
        WeaponData[] weaponDatas = GameManager.inst.equippedWeapons;
        if (weaponDatas[0] == null || weaponDatas[1] == null)
        {
            Debug.Log("무기 UI 업데이트에 문제 발생");
            return;
        }
        if (weaponIdx == 0)
        {
            mainSlotImage.sprite = weaponDatas[0].icon;
            subSlotImage.sprite = weaponDatas[1].icon;
        }
        else
        {
            mainSlotImage.sprite = weaponDatas[1].icon;
            subSlotImage.sprite = weaponDatas[0].icon;
        }
    }

    public int GetCurrentWeaponIndex()
    {
        return currentWeaponIndex;
    }
}
