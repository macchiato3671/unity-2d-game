using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour
{
    public Button iconButton;          // ��ư 
    public GameObject highlightBorder; // �׵θ� ������Ʈ (�ڽ�)

    [HideInInspector]
    public WeaponData weaponData;

    private InventoryManager inventoryManager;

    private Image iconImage; // ��ư�� ���� Image ������Ʈ

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