using UnityEngine;

public class AccessoryInitializer : MonoBehaviour
{
    void Start()
    {
        InitializeAccessories();
        InitializeWeapons();
        Debug.Log("ExploreBase 초기화 완료");
    }

    void InitializeAccessories()
    {
        string[] accessoryFolders = { "GravityAccessory", "LaserAccessory", "MeleeAccessory", "ThrowingAccessory" };
        foreach (string folder in accessoryFolders)
        {
            AccessoryData[] accessories = Resources.LoadAll<AccessoryData>("AccessoryData/" + folder);
            foreach (var acc in accessories)
            {
                acc.isget = false;
            }
        }
    }

    void InitializeWeapons()
    {
        WeaponData[] weapons = Resources.LoadAll<WeaponData>("WeaponData");
        foreach (var weapon in weapons)
        {
            weapon.accessoryData1 = null;
            weapon.accessoryData2 = null;
        }
    }
}
