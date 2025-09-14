using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentData", menuName = "Scriptable Object/EquipmentData")]
public class EquipmentData : ScriptableObject
{
    public int equipID;
    public string equipName;
    public string equipDescription;
    public Sprite equipImage;
    public int currentLevel;
    public int requiredResource; //레벨업에 필요한 자원수
}
