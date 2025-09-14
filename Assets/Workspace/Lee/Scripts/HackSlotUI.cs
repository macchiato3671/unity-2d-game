using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HackSlotUI : MonoBehaviour
{
    public Image iconImage;             // ������ ������
    public Button toggleButton;         // ����/���� ��ư
    public TextMeshProUGUI toggleButtonText;       // ��ư �� �ؽ�Ʈ
    public Button parentButton;         // ���� ��ü Ŭ�� �� ���г� ǥ�ÿ�

    [HideInInspector]
    public HackSkillData hackSkillData;

    private InventoryManager inventoryManager;

    public void Init(HackSkillData data, InventoryManager manager)
    {
        hackSkillData = data;
        inventoryManager = manager;

        iconImage.sprite = data.icon;
        SetButtonText("����");

        toggleButton.onClick.RemoveAllListeners();
        toggleButton.onClick.AddListener(() => inventoryManager.ToggleHack(hackSkillData, this));

        parentButton.onClick.RemoveAllListeners();
        parentButton.onClick.AddListener(() => inventoryManager.ShowDetailPanel(hackSkillData.description, hackSkillData.icon));
    }

    public void SetButtonText(string text)
    {
        toggleButtonText.text = text;
    }
}