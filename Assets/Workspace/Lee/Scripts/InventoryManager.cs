using System.Collections.Generic;
using TMPro;
using Unity.Multiplayer.Center.Common;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    // 전체 보유 아이템들
    public List<WeaponData> allWeapons;
    public List<HackSkillData> allHacks;
    public List<AccessoryData> allAccessories;


    public WeaponSelectManager weaponSelectManager;


    public GameManager gameManager;
    // 현재 장착된 무기 (2개)
    public WeaponData primaryWeapon;
    public WeaponData secondaryWeapon;

    // 현재 선택된 해킹
    public HackSkillData selectedHack;

    // 장착된 악세사리 (최대 2개)
    public List<AccessoryData> primaryAccessories = new List<AccessoryData>(2);
    public List<AccessoryData> secondaryAccessories = new List<AccessoryData>(2);

    // 해킹 아이콘 표시용 Image (1개)
    public Image selectedHackIcon;

    // 악세사리 아이콘 표시용 Image 배열 (최대 2개)
    public Image[] accessoryIcons = new Image[2];
    public GameObject InventoryPanel;


    // 중앙 UI 출력용 (무기 정보)
    public Image weaponSprite;
    public TextMeshProUGUI weaponDescription;

    // 해킹, 악세사리 상세 패널 UI
    public GameObject detailPanel;
    public TextMeshProUGUI detailText;
    public Image detailImg;

    // 무기 버튼 4개 (씬에서 연결)
    public WeaponSlotUI[] weaponButtons = new WeaponSlotUI[4];

    //무기 skill ui 출력용
    public GameObject WeaponskillPanel;
    public Button skillshowButton;
    public Image weaponSkillSprite1;
    public Image weaponSkillSprite2;
    public TextMeshProUGUI Skill1des;
    public TextMeshProUGUI Skill2des;

    //무기 사기 가능
    public TextMeshProUGUI wp_price_text;
    public Button wp_buy_button;

    // 해킹 & 악세사리 슬롯 및 부모 (스크롤뷰)
    public GameObject hackSlotPrefab;
    public GameObject accessorySlotPrefab;
    public Transform hackSlotParent;
    public Transform accessorySlotParent;

    //panel
    public GameObject WarningPanel;
  //  public GameObject WarningPanel2;
    public GameObject WarningPanel3;
   // public GameObject WeaponbuyPanel;

    public List<HackSlotUI> hackSlots = new List<HackSlotUI>();
    public List<AccessorySlotUI> accessorySlots = new List<AccessorySlotUI>();

    public bool weaponCanChange = true;
    public bool can_open_inven = true;
    public int curWeaponIndex = 1;

    public bool isTutorial = false;

    void Start()
    {
        if (gameManager == null) gameManager = GameManager.inst;

        if (weaponSelectManager.equippedWeapons.Length >= 1 && weaponSelectManager.equippedWeapons[0] != null)
        {
            primaryWeapon = weaponSelectManager.equippedWeapons[0];
        }
        if (weaponSelectManager.equippedWeapons.Length >= 2 && weaponSelectManager.equippedWeapons[1] != null)
        {
            secondaryWeapon = weaponSelectManager.equippedWeapons[1];
        }

        if (gameManager == null)
        {
            Debug.LogWarning("gameManager is NULL!!");
        }
        else if (gameManager.equippedHackSkill == null)
        {
            Debug.LogWarning("gameManager.equippedHackSkill is NULL!!");
        }
        else
        {
            selectedHack = gameManager.equippedHackSkill;
            Debug.Log($" selectedHack loaded: {selectedHack.skillName}");
        }
        UpdateSelectedHackIcon();

        // 무기 버튼 초기화
        for (int i = 0; i < weaponButtons.Length; i++)
        {
            if (i < allWeapons.Count)
                weaponButtons[i].Init(allWeapons[i], this);
            else
                weaponButtons[i].gameObject.SetActive(false);
        }

        InitHackSlots();
        InitAccessorySlots();
        UpdateEquippedAccessoryIcons();
        ShowWeaponDetail(1);
        UpdateWeaponHighlights();

        // 상세패널은 처음 숨김
        detailPanel.SetActive(false);
        WeaponskillPanel.SetActive(false);
        WarningPanel.SetActive(false);
//        WarningPanel2.SetActive(false);
        WarningPanel3.SetActive(false);
 //       WeaponbuyPanel.SetActive(false);
    }


    public void OpenInventory()
    {
        SoundManager.inst.PlaySFX(Resources.Load<AudioClip>("Audio/Inventory_Open"), 1);
        if (gameManager.equippedHackSkill != null) selectedHack = gameManager.equippedHackSkill;
        InitHackSlots();
        InitAccessorySlots();
        UpdateSelectedHackIcon();
        InitEquippedAccessories();
        UpdateEquippedAccessoryIcons();

        for (int i = 0; i < weaponButtons.Length; i++)
        {
            if (i < allWeapons.Count)
                weaponButtons[i].Init(allWeapons[i], this);
            else
                weaponButtons[i].gameObject.SetActive(false);
        }

        UpdateWeaponHighlights();
        InventoryPanel.SetActive(true);
    }

    public void UpdateWeaponHighlights()
    {
        foreach (var slot in weaponButtons)
        {
            bool isEquipped = (slot.weaponData == primaryWeapon) || (slot.weaponData == secondaryWeapon);
            slot.SetHighlight(isEquipped);
        }
    }

    // 무기 장착/해제 및 UI 업데이트 
    public void OnClickWeaponSlot(WeaponData selected)
    {

        if (!weaponCanChange)
        {
            Debug.Log("무기 변경은 현재 불가능합니다.");
            return;
        }

        if (!isTutorial)
        {
            // 동일 무기 중복 장착 방지
            if (selected == primaryWeapon || selected == secondaryWeapon)
            {
                // 이미 장착된 무기라면 해당 위치에서 해제
                if (selected == primaryWeapon)
                {
                    primaryWeapon = null;
                    curWeaponIndex = 1;
                   
                }
                else
                {
                    secondaryWeapon = null;
                    curWeaponIndex = 2;
                    
                }
            }
            else if (primaryWeapon == null)
            {
                primaryWeapon = selected;
                curWeaponIndex = 1;
                ;
            }
            else if (secondaryWeapon == null)
            {
                secondaryWeapon = selected;
                curWeaponIndex = 2;
            }
            InitEquippedAccessories();
            UpdateWeaponUI();
            UpdateEquippedAccessoryIcons();
            ShowWeaponDetail(curWeaponIndex);
        }
    }
        void UpdateWeaponDataAccessories()
    {
        if (primaryWeapon != null)
        {
            primaryWeapon.accessoryData1 = (primaryAccessories.Count > 0) ? primaryAccessories[0] : null;
            primaryWeapon.accessoryData2 = (primaryAccessories.Count > 1) ? primaryAccessories[1] : null;
        }

        if (secondaryWeapon != null)
        {
            secondaryWeapon.accessoryData1 = (secondaryAccessories.Count > 0) ? secondaryAccessories[0] : null;
            secondaryWeapon.accessoryData2 = (secondaryAccessories.Count > 1) ? secondaryAccessories[1] : null;
        }
    }
    /*
        public void TryBuyWeapon(WeaponData selected)
        {
            int required = selected.requiredResource;
            if (ResourceManager.UseResource(required)) { selected.isget = true; WeaponbuyPanel.gameObject.SetActive(false); }
            else WarningPanel2.SetActive(true);
        }
    */
    private void UpdateWeaponUI()
    {
        for (int i = 0; i < weaponButtons.Length; i++)
        {
            bool isEquipped = (weaponButtons[i].weaponData == primaryWeapon) || (weaponButtons[i].weaponData == secondaryWeapon);
            weaponButtons[i].SetHighlight(isEquipped);
        }
    }

    public void ShowWeaponDetail(int slotIndex)
    {
        if (slotIndex == 1 && primaryWeapon != null)
        {
            curWeaponIndex = 1;
            weaponSprite.sprite = primaryWeapon.icon;
            weaponDescription.text = primaryWeapon.description;
            weaponSprite.color = new Color(1f, 1f, 1f, 1f);

            skillshowButton.gameObject.SetActive(true);
            skillshowButton.onClick.RemoveAllListeners(); // 기존 리스너 제거
            skillshowButton.onClick.AddListener(() => WeaponskillPanel.SetActive(true));
            skillshowButton.onClick.AddListener(() => ShowWeaponSkillPanel(1));
            UpdateEquippedAccessoryIcons();
        }
        else if (slotIndex == 2 && secondaryWeapon != null)
        {
            curWeaponIndex = 2;
            weaponSprite.sprite = secondaryWeapon.icon;
            weaponDescription.text = secondaryWeapon.description;
            weaponSprite.color = new Color(1f, 1f, 1f, 1f);

            skillshowButton.gameObject.SetActive(true);
            skillshowButton.onClick.RemoveAllListeners(); // 기존 리스너 제거
            skillshowButton.onClick.AddListener(() => WeaponskillPanel.SetActive(true));
            skillshowButton.onClick.AddListener(() => ShowWeaponSkillPanel(2));
            UpdateEquippedAccessoryIcons();
        }
        else
        {
            weaponSprite.sprite = null;
            weaponDescription.text = "";
            weaponSprite.color = new Color(1f, 1f, 1f, 0f);

            skillshowButton.gameObject.SetActive(false);
        }
    }

    public void ShowWeaponSkillPanel(int slotIndex)
    {
        if (slotIndex == 1)
        {
            weaponSkillSprite1.sprite = primaryWeapon.qSkillData.icon;
            weaponSkillSprite2.sprite = primaryWeapon.eSkillData.icon;
            Skill1des.text = primaryWeapon.qSkillData.description;
            Skill2des.text = primaryWeapon.eSkillData.description;
        } else if (slotIndex == 2)
        {
            weaponSkillSprite1.sprite = secondaryWeapon.qSkillData.icon;
            weaponSkillSprite2.sprite = secondaryWeapon.eSkillData.icon;
            Skill1des.text = secondaryWeapon.qSkillData.description;
            Skill2des.text = secondaryWeapon.eSkillData.description;
        }
    }

    // 해킹 슬롯 생성 및 초기화
    void InitHackSlots()
    {

        foreach (Transform child in hackSlotParent)
        {
            Destroy(child.gameObject);
        }
        hackSlots.Clear();

        HackSkillData[] allHackData = Resources.LoadAll<HackSkillData>("HackSkillData");

        foreach (var hack in allHackData)
        {
            if (hack.isget)
            {
                GameObject obj = Instantiate(hackSlotPrefab, hackSlotParent);
                HackSlotUI slot = obj.GetComponent<HackSlotUI>();
                slot.Init(hack, this);
                if (hack == selectedHack)
                {
                    TextMeshProUGUI tmpText = obj.GetComponentInChildren<TextMeshProUGUI>();
                    if (tmpText != null)
                        tmpText.text = "장착";
                    else
                        Debug.LogWarning("TextMeshPro 텍스트를 찾을 수 없습니다.");
                }
                hackSlots.Add(slot);
            }
        }
    }
        // 악세사리 슬롯 생성 및 초기화
        void InitAccessorySlots()
        {

        foreach (Transform child in accessorySlotParent)
        {
            Destroy(child.gameObject);
        }
        accessorySlots.Clear();
        AccessoryData acc1 = primaryWeapon != null ? primaryWeapon.accessoryData1 : null;
        AccessoryData acc2 = primaryWeapon != null ? primaryWeapon.accessoryData2 : null;
        AccessoryData acc3 = secondaryWeapon != null ? secondaryWeapon.accessoryData1 : null;
        AccessoryData acc4 = secondaryWeapon != null ? secondaryWeapon.accessoryData2 : null;


        string[] accessoryFolders = { "GravityAccessory", "LaserAccessory", "MeleeAccessory", "ThrowingAccessory" };

            foreach (string folder in accessoryFolders)
            {
                AccessoryData[] accessories = Resources.LoadAll<AccessoryData>("AccessoryData/" + folder);

                foreach (var acc in accessories)
                {
                    if (acc.isget)
                    {
                        GameObject obj = Instantiate(accessorySlotPrefab, accessorySlotParent);
                        AccessorySlotUI slot = obj.GetComponent<AccessorySlotUI>();
                        slot.Init(acc, this);

                    if (acc == acc1 || acc == acc2 || acc == acc3 || acc == acc4)
                    {
                        TextMeshProUGUI tmpText = obj.GetComponentInChildren<TextMeshProUGUI>();
                        if (tmpText != null)
                            tmpText.text = "장착";
                        else
                            Debug.LogWarning("TextMeshPro 텍스트를 찾을 수 없습니다.");
                    }

                    accessorySlots.Add(slot);
                    }
                }
            }
        }

    public void InitEquippedAccessories()
    {
        primaryAccessories.Clear();
        secondaryAccessories.Clear();

        if (primaryWeapon != null)
        {
            if (primaryWeapon.accessoryData1 != null) primaryAccessories.Add(primaryWeapon.accessoryData1);
            if (primaryWeapon.accessoryData2 != null) primaryAccessories.Add(primaryWeapon.accessoryData2);
        }

        if (secondaryWeapon != null)
        {
            if (secondaryWeapon.accessoryData1 != null) secondaryAccessories.Add(secondaryWeapon.accessoryData1);
            if (secondaryWeapon.accessoryData2 != null) secondaryAccessories.Add(secondaryWeapon.accessoryData2);
        }
    }

    // 해킹 장착/해제 버튼 클릭 처리
    public void ToggleHack(HackSkillData data, HackSlotUI slotUI)
    {
        if (!weaponCanChange)
        {
            Debug.Log("무기 변경은 현재 불가능합니다.");
            return;
        }

        if (selectedHack == data)
        {
            // 해제
            selectedHack = null;
            slotUI.SetButtonText("장착");
            detailPanel.SetActive(false);
            UpdateSelectedHackIcon();
        }
        else
        {
            selectedHack = data;
            foreach (var slot in hackSlots)
            {
                if (slot.hackSkillData == selectedHack)
                    slotUI.SetButtonText("해제");
                else
                    slotUI.SetButtonText("장착");
            }

            // 상세패널 활성화 및 정보 출력
            //ShowDetailPanel(data.skillName);
            UpdateSelectedHackIcon();
        }
    }

    // 악세사리 장착/해제 버튼 클릭 처리
    public void ToggleAccessory(AccessoryData data, AccessorySlotUI slotUI)
    {
        if (!weaponCanChange)
        {
            Debug.Log("무기 변경은 현재 불가능합니다.");
            return;
        }

        WeaponData currentWeapon = (curWeaponIndex == 1) ? primaryWeapon : secondaryWeapon;
        List<AccessoryData> currentList = (curWeaponIndex == 1) ? primaryAccessories : secondaryAccessories;

        if (data.compatibleWeapon != currentWeapon.type)
        {
            Debug.Log("해당 무기에 장착할 수 없는 악세사리입니다.");
            WarningPanel3.SetActive(true); // 원하면 별도 패널로 분리해도 됨
            return;
        }

        if (currentList.Contains(data))
        {
            currentList.Remove(data);
            slotUI.SetButtonText("장착");
            detailPanel.SetActive(false);
            UpdateEquippedAccessoryIcons();
            UpdateWeaponDataAccessories();
        }
        else
        {
            if (currentList.Count >= 2)
            {
                Debug.Log("해당 무기에는 악세사리를 최대 2개까지만 장착할 수 있습니다.");
                return;
            }
            currentList.Add(data);
            slotUI.SetButtonText("해제");
            UpdateEquippedAccessoryIcons();
            UpdateWeaponDataAccessories();
        }
    }

    // 상세패널 표시
    public void ShowDetailPanel(string name, Sprite icon)
    {
        detailPanel.SetActive(true);
        detailText.text = name;
        if (icon != null) detailImg.sprite = icon;
    }

    // 상세패널 닫기 (UI 버튼 등에 연결)
    public void CloseDetailPanel()
    {
        detailPanel.SetActive(false);
    }

    void UpdateSelectedHackIcon()
    {

        if (selectedHack != null && selectedHack.icon != null)
        {
            Debug.Log("해킹 아이콘 활성화");
            selectedHackIcon.enabled = true;
            selectedHackIcon.sprite = selectedHack.icon;
            selectedHackIcon.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            Debug.Log("해킹 아이콘 비활성화");
            selectedHackIcon.enabled = false;
            selectedHackIcon.sprite = null;
            selectedHackIcon.color = new Color(1f, 1f, 1f, 0f);
        }

        //hackSlot
        
            foreach (var slot in hackSlots)
            {
                if (slot.hackSkillData == selectedHack)
                    slot.SetButtonText("해제");
                else
                    slot.SetButtonText("장착");
            }
  
    }

    void UpdateEquippedAccessoryIcons()
    {
        List<AccessoryData> currentList = (curWeaponIndex == 1) ? primaryAccessories : secondaryAccessories;

        for (int i = 0; i < accessoryIcons.Length; i++)
        {
            if (i < currentList.Count && currentList[i].icon != null)
            {
                accessoryIcons[i].enabled = true;
                accessoryIcons[i].sprite = currentList[i].icon;
                accessoryIcons[i].color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                accessoryIcons[i].enabled = false;
                accessoryIcons[i].sprite = null;
                accessoryIcons[i].color = new Color(1f, 1f, 1f, 0f);
            }
        }

        foreach (var slot in accessorySlots)
        {
            bool isEquipped = (curWeaponIndex == 1 && primaryAccessories.Contains(slot.accessoryData)) ||
                              (curWeaponIndex == 2 && secondaryAccessories.Contains(slot.accessoryData));

            slot.SetButtonText(isEquipped ? "해제" : "장착");
        }
    }

    public int OnCloseInventory()
    {
        if (primaryWeapon == null || secondaryWeapon == null)
        {
            WarningPanel.SetActive(true);
            return 0;
        }
        //악세사리 저장
        if (primaryAccessories.Count > 0)
            primaryWeapon.accessoryData1 = primaryAccessories[0];
        if (primaryAccessories.Count > 1)
            primaryWeapon.accessoryData2 = primaryAccessories[1];

        if (secondaryAccessories.Count > 0)
            secondaryWeapon.accessoryData1 = secondaryAccessories[0];
        if (secondaryAccessories.Count > 1)
            secondaryWeapon.accessoryData2 = secondaryAccessories[1];

        // hackModeManager에 selectedSkill 반영
        if (gameManager != null)
        {
            gameManager.equippedHackSkill = selectedHack;
            Debug.Log("HackModeManager selectedSkill updated.");
        }

        // weaponSelectManager에 equippedWeapons 반영
        if (weaponSelectManager != null)
        {
            weaponSelectManager.equippedWeapons[0] = primaryWeapon;
            weaponSelectManager.equippedWeapons[1] = secondaryWeapon;
            weaponSelectManager.FinalizeSelection();
            Debug.Log("WeaponSelectManager equippedWeapons updated.");
        }

        if(GameManager.inst.player) GameManager.inst.player.UpdateEquippedWeapon();

        // 인벤토리 UI 닫기
        SoundManager.inst.PlaySFX(Resources.Load<AudioClip>("Audio/Inventory_Close"), 1);
        InventoryPanel.SetActive(false);
        return 1;
    }

    public void ClickSound()
    {
        SoundManager.inst.PlaySFX(Resources.Load<AudioClip>("Audio/Inventory_Equip"), 1);
    }
}
