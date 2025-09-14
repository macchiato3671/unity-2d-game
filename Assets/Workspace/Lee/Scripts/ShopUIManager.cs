using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    public GameObject itemButtonPrefab;    // ��ǰ ��ư ������
    public Transform itemListContainer;    // ��ǰ ����� ���� ����
    public TextMeshProUGUI itemNameText;              // ��ǰ�� �ؽ�Ʈ
    public TextMeshProUGUI itemDescriptionText;       // ��ǰ ���� �ؽ�Ʈ
    public Image itemImage;                // ��ǰ �̹���
    public TextMeshProUGUI itemTypeText;

    public ItemData[] items;                   // ������ ������ �迭

    public GameObject itemDetailPanel;
    public Image itemDetailImage;
    public TextMeshProUGUI itembuytext;
    public Button itembuybutton;
    public ResourceUIManager resourceUIManager;

    private Dictionary<ItemData, GameObject> itemButtonMap = new Dictionary<ItemData, GameObject>();

    float savedTimeScale;

    // ������ ����� UI�� ǥ���ϴ� �Լ�

    void Awake()
    {
        if (GameManager.inst != null) GameManager.inst.shopUIManager = this;
    }

    public void SetShop(bool flag)
    {
        if (flag)
        {
            gameObject.SetActive(true);
            GameManager.inst.IsOnInventory = true;
            savedTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }
        else
        {
            GameManager.inst.IsOnInventory = false;
            Time.timeScale = savedTimeScale;
            gameObject.SetActive(false);
        }
    }

    public void PopulateShop()
    {
        itemDetailPanel.SetActive(false);
        SoundManager.inst.PlaySFX(Resources.Load<AudioClip>("Audio/Puzzle_start"), 1);

        foreach (Transform child in itemListContainer)
        {
            Destroy(child.gameObject);
        }

        // ������ �����͸� �������� ��ư�� ����
        foreach (var item in items)
        {
            GameObject newButton = Instantiate(itemButtonPrefab, itemListContainer);
            //Button button = newButton.GetComponent<Button>();
            //Text buttonText = button.GetComponent<Text>();
            //buttonText.text = item.itemPrice.ToString();
            newButton.GetComponentInChildren<Image>().sprite = item.itemImage;
            newButton.GetComponent<Button>().onClick.AddListener(() => ShowItemDetails(item));

            //Button childButton = newButton.transform.Find("Button").GetComponent<Button>();  // �ڽ� ��ư �̸��� "ChildButton"�̶�� ����
            //childButton.onClick.AddListener(() => TryBuyItem(item));

            itemButtonMap[item] = newButton;
        }

    }

    public void GenerateRandomShopItems()
    {
        List<ItemData> pool = new List<ItemData>();

        HackSkillData[] hacks = Resources.LoadAll<HackSkillData>("HackSkillData");
        foreach (var h in hacks)
        {
            if (!h.isget)
            {
                pool.Add(new ItemData
                {
                    itemName = h.skillName,
                    itemType = "해킹",
                    itemDescription = h.description,
                    itemImage = h.icon,
                    itemPrice = Random.Range(10, 21), // 💡 10~20 랜덤 가격
                    originalData = h
                });
            }
        }

        string[] folders = { "AccessoryData/GravityAccessory", "AccessoryData/LaserAccessory", "AccessoryData/MeleeAccessory", "AccessoryData/ThrowingAccessory" };
        foreach (string folder in folders)
        {
            AccessoryData[] accs = Resources.LoadAll<AccessoryData>(folder);
            foreach (var a in accs)
            {
                if (!a.isget)
                {
                    pool.Add(new ItemData
                    {
                        itemName = a.accessoryName,
                        itemType = "악세사리",
                        itemDescription = a.description,
                        itemImage = a.icon,
                        itemPrice = Random.Range(10, 21),
                        originalData = a
                    });
                }
            }
        }

        // 랜덤 3~5개 추출
        int count = Random.Range(3, 6);
        List<ItemData> selected = new List<ItemData>();
        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int idx = Random.Range(0, pool.Count);
            selected.Add(pool[idx]);
            pool.RemoveAt(idx);
        }

        items = selected.ToArray();
    }

    // ������ Ŭ�� �� ���� ���� ǥ��
    void ShowItemDetails(ItemData item)
    {
        itemDetailPanel.SetActive(true);
        itemDetailImage.sprite = item.itemImage;
        itemNameText.text = item.itemName;
        itemTypeText.text = item.itemType;
        itemDescriptionText.text = item.itemDescription;
        itembuytext.text = item.itemPrice.ToString() ;

        itembuybutton.onClick.RemoveAllListeners();
        itembuybutton.onClick.AddListener(() => TryBuyItem(item));

    }

    void TryBuyItem(ItemData item)
    {
        if (ResourceManager.have_resource_amount >= item.itemPrice)
        {
            ResourceManager.UseResource(item.itemPrice);
            Debug.Log($"[구매 완료] {item.itemName} ({item.itemType})");
            SoundManager.inst.PlaySFX(Resources.Load<AudioClip>("Audio/buy"), 1);

            if (item.originalData is HackSkillData hack)
            {
                hack.isget = true;
            }
            else if (item.originalData is AccessoryData acc)
            {
                acc.isget = true;
            }

            if (itemButtonMap.TryGetValue(item, out GameObject btn))
            {
                Image img = btn.GetComponentInChildren<Image>();
                if (img != null)
                    img.color = Color.gray;

                Button btnComp = btn.GetComponent<Button>();
                if (btnComp != null)
                    btnComp.interactable = false;
            }

            itemDetailPanel.SetActive(false);
            resourceUIManager?.updateResourceText();
        }
        else
        {
            Debug.Log("자원이 부족합니다.");
        }
    }
    /*
    void UpdateItemUI(ItemData item)
    {
        if (!itemButtonMap.ContainsKey(item)) return;

        GameObject btn = itemButtonMap[item];
        Image img = btn.GetComponentInChildren<Image>();
        img.color = Color.gray;

        // 클릭도 막고 싶다면
        Button childButton = btn.transform.Find("Button").GetComponent<Button>();
        childButton.interactable = false;
    }*/

    public void CloseItemDetailPanel()
    {
        itemDetailPanel.SetActive(false);
    }

    public void Speak()
    {
        SoundManager.inst.PlaySFX(Resources.Load<AudioClip>("Audio/Puzzle_start"), 1);
    }
}