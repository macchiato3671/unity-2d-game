using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string saveName;
    public int resource;
    public ClearData[] clearPercent;
    public float deathPercent;
    public WeaponType[] equippedWeaponType;
    public SavedEquipData[] equipDatas;
    public string equippedHackName;
    public List<string> unlockedHackSkills = new List<string>();  // skillName ����
    public List<int> unlockedAccessories = new List<int>();       // accessoryID ����
    public List<int> unlockedWeapons = new List<int>();           // weaponID ����

    public SaveData()
    {
        clearPercent = new ClearData[GameManager.worldCount];
        equippedWeaponType = new WeaponType[2];
        equipDatas = new SavedEquipData[4];
        for (int i = 0; i < equipDatas.Length; i++)
            equipDatas[i] = new SavedEquipData();
    }
}

[System.Serializable]
public class SavedEquipData
{
    public int equipID;
    public int currentLevel;
    public int requiredResource;
}