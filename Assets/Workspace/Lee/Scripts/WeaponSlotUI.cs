using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour
{
    public Button iconButton;          // 버튼 
    public GameObject highlightBorder; // 테두리 오브젝트 (자식)

    [HideInInspector]
    public WeaponData weaponData;

    private InventoryManager inventoryManager;

    private Image iconImage; // 버튼에 붙은 Image 컴포넌트

    void Awake()
    {
        if (iconButton != null)
            iconImage = iconButton.GetComponent<Image>();
    }

    public void Init(WeaponData data, InventoryManager manager)
    {
        weaponData = data;
        inventoryManager = manager;

        if (iconImage != null && data.icon != null)
            iconImage.sprite = data.icon;

        SetHighlight(false);

        if (iconButton != null)
        {
            iconButton.onClick.RemoveAllListeners();
            iconButton.onClick.AddListener(OnClick);
        }
    }

    public void OnClick()
    {
        inventoryManager.OnClickWeaponSlot(weaponData);
    }

    public void SetHighlight(bool isSelected)
    {
        if (highlightBorder != null)
            highlightBorder.SetActive(isSelected);
    }
}