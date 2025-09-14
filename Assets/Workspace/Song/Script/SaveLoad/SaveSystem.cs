using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class SaveSystem
{
    public static SaveData dataToLoad = null;

    public static void Save(SaveData data, string fileName)
    {
        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, fileName + ".json");
        File.WriteAllText(path, json);
        Debug.Log("SaveSystem에서 Save 수행 완료");
    }

    public static SaveData Load(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName + ".json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            Debug.LogWarning("세이브 파일이 존재하지 않습니다.");
            return null;
        }
    }

    public static void DeleteSave(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName + ".json");
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("세이브 파일이 삭제되었습니다.");
        }
        else
        {
            Debug.LogWarning("세이브 파일이 존재하지 않아서 삭제할 수 없습니다.");
        }
    }

    public static List<string> GetSaveFiles()
    {
        string path = Application.persistentDataPath;
        return Directory.GetFiles(path, "save_*.json").Select(Path.GetFileNameWithoutExtension).ToList();
    }    

    public static void SaveGame()
    {
        SaveData data = new SaveData();
        EquipmentData[] eDatas = GameManager.inst.equipmentManager?.equipDatas;

        data.saveName = "save_1";
        data.resource = ResourceManager.have_resource_amount;
        data.clearPercent = GameManager.inst.ClearPercent;
        data.deathPercent = GameManager.inst.DeathPercent;
        for(int i = 0; i < 2; i++)
            data.equippedWeaponType[i] = GameManager.inst.equippedWeapons[i].type;
        for (int i = 0; i < data.equipDatas.Length; i++)
        {
            if (GameManager.inst.equipmentManager == null) break;
            data.equipDatas[i].equipID = eDatas[i].equipID;
            data.equipDatas[i].currentLevel = eDatas[i].currentLevel;
            data.equipDatas[i].requiredResource = eDatas[i].requiredResource;
        }
        data.equippedHackName = GameManager.inst.equippedHackSkill?.skillName;

        // HackSkill 저장 (Resources/HackSkills)
        HackSkillData[] allHacks = Resources.LoadAll<HackSkillData>("HackSkills");
        foreach (var hack in allHacks)
        {
            if (hack.isget)
                data.unlockedHackSkills.Add(hack.skillName);
        }

        // Accessory 저장 (모든 폴더 합쳐서)
        string[] folders = { "AccessoryData/GravityAccessory", "AccessoryData/LaserAccessory", "AccessoryData/MeleeAccessory", "AccessoryData/ThrowingAccessory" };
        foreach (string folder in folders)
        {
            AccessoryData[] accessories = Resources.LoadAll<AccessoryData>(folder);
            foreach (var acc in accessories)
            {
                if (acc.isget)
                    data.unlockedAccessories.Add(acc.accessoryID);
            }
        }

        // Weapon 저장 (Resources/WeaponData)
        WeaponData[] allWeapons = Resources.LoadAll<WeaponData>("WeaponData");
        foreach (var weapon in allWeapons)
        {
            if (weapon.isget)
                data.unlockedWeapons.Add(weapon.weaponID);
        }
        Save(data, data.saveName);
    }
}
