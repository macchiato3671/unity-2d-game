using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class equipUIManager : MonoBehaviour
{ // �� ������ UI ��ҵ��� �̸� ���ǵ� �迭�� ����
    public TextMeshProUGUI[] equipNameTexts;          // �� ������ �̸��� ǥ���� �ؽ�Ʈ (4��)
    public TextMeshProUGUI[] equipDescriptionTexts;   // �� ������ ������ ǥ���� �ؽ�Ʈ (4��)
    public Image[] equipImageUI;           // �� ������ �̹����� ǥ���� �̹��� (4��)
    public TextMeshProUGUI[] currentLevelTexts;      // �� ������ ������ ǥ���� �ؽ�Ʈ (4��)
    public Slider[] resourceBars;         // �� �������� �ڿ� �ٸ� ǥ���� �����̴� (4��)
    public Button[] updateButtons;        // �� �������� ������ ��ư (4��)
    public TextMeshProUGUI[] resourceTexts;

    public GameObject itemUIPanel;        // ������ UI Panel (�̹� ����Ƽ �����Ϳ��� ������� ���·� �Ҵ��)

    public EquipmentManager equipManager;       // ������ ������ �����ϴ� ItemManager

    // void Start()
    // {
    //     itemUIPanel.SetActive(false);  // ���� �� �г��� ��Ȱ��ȭ
    // }

    // ������ UI �г��� ���̰ų� �����
   public void ToggleEquipUIPanel()
    {
        SoundManager.inst.PlaySFX(Resources.Load<AudioClip>("Audio/Interact_Pickup"), 1);
        bool isActive = itemUIPanel.activeSelf;
        itemUIPanel.SetActive(!isActive);  // ���� Ȱ��ȭ ���¿� �ݴ�� ���
    }

    // UI �г��� ������Ʈ�ϴ� �Լ� (������ ���� ������Ʈ)
    public void UpdateEquipUIPanel()
    {
        for (int i = 0; i < equipManager.equipDatas.Length; i++)
        {
            // �� �������� UI ��ҵ��� ����
            equipNameTexts[i].text = equipManager.equipDatas[i].equipName;
            equipDescriptionTexts[i].text = equipManager.equipDatas[i].equipDescription;
            equipImageUI[i].sprite = equipManager.equipDatas[i].equipImage;
            currentLevelTexts[i].text = equipManager.equipDatas[i].currentLevel.ToString();

            // �ڿ� �� ������Ʈ
            int currentResourceAmount = ResourceManager.have_resource_amount;
            float value = (float)currentResourceAmount / (float)equipManager.equipDatas[i].requiredResource;
            if (value >= 1) value = 1;
            resourceBars[i].value = value;
            Debug.Log("Resource Percentage: " + value);

            if (resourceTexts != null && i < resourceTexts.Length)
                resourceTexts[i].text = $"{currentResourceAmount} / {equipManager.equipDatas[i].requiredResource}";

            // ������ ��ư Ȱ��ȭ ����
            updateButtons[i].interactable = resourceBars[i].value >= 1;

            // ��ư Ŭ�� �̺�Ʈ ����
            updateButtons[i].onClick.RemoveAllListeners();
            int equipID = equipManager.equipDatas[i].equipID;
            updateButtons[i].onClick.AddListener(() => OnUpgradeButtonClicked(equipID));
        }
    }

    // ������ ��ư Ŭ�� ��
    public void OnUpgradeButtonClicked(int equipID)
    {
        equipManager.Upgrade_equip(equipID);
        UpdateEquipUIPanel();  // UI ����
    }

}
