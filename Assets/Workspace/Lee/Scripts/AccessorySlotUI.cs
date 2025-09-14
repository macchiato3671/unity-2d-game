using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccessorySlotUI : MonoBehaviour
{
    public Image iconImage;             // 아이템 아이콘
    public Button toggleButton;         // 장착/해제 버튼
    public TextMeshProUGUI toggleButtonText;       // 버튼 내 텍스트
    public Button parentButton;         // 슬롯 클릭 시 상세패널 표시용

    [HideInInspector]
    public AccessoryData accessoryData;

    private InventoryManager inventoryManager;

    public void Init(AccessoryData data, InventoryManager manager)
    {
        accessoryData = data;
        inventoryManager = manager;

        iconImage.sprite = data.icon;
        SetButtonText("장착");

        toggleButton.onClick.RemoveAllListeners();
        toggleButton.onClick.AddListener(() => inventoryManager.ToggleAccessory(accessoryData, this));

        parentButton.onClick.RemoveAllListeners();
        parentButton.onClick.AddListener(() => inventoryManager.ShowDetailPanel(accessoryData.accessoryName, accessoryData.icon));
    }

    public void SetButtonText(string text)
    {
        toggleButtonText.text = text;
    }
}