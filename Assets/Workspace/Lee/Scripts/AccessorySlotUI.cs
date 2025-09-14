using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccessorySlotUI : MonoBehaviour
{
    public Image iconImage;             // ������ ������
    public Button toggleButton;         // ����/���� ��ư
    public TextMeshProUGUI toggleButtonText;       // ��ư �� �ؽ�Ʈ
    public Button parentButton;         // ���� Ŭ�� �� ���г� ǥ�ÿ�

    [HideInInspector]
    public AccessoryData accessoryData;

    private InventoryManager inventoryManager;

    public void Init(AccessoryData data, InventoryManager manager)
    {
        accessoryData = data;
        inventoryManager = manager;

        iconImage.sprite = data.icon;
        SetButtonText("����");

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