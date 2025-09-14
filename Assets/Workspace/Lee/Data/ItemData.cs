using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string itemType;
    public string itemDescription;
    public Sprite itemImage;
    public int itemPrice;

    public ScriptableObject originalData;
}
