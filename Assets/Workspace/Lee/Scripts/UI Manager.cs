using UnityEngine;

public class UIManager : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public equipUIManager equipUIManager;

    float savedTimeScale;

    void Update()
    {
        if (GameManager.inst.IsPaused) return;
        if (Input.GetKeyDown(KeyCode.Tab) && inventoryManager.can_open_inven)
        {
            if (inventoryManager.InventoryPanel.activeSelf)
            {
                int result = inventoryManager.OnCloseInventory();

                if (result == 1)
                {
                    GameManager.inst.IsOnInventory = false;
                    Time.timeScale = savedTimeScale;
                }
            }
            else
            {
                if (GameManager.inst.IsOnInventory) return;
                GameManager.inst.IsOnInventory = true;
                savedTimeScale = Time.timeScale;
                Time.timeScale = 0f;
                inventoryManager.OpenInventory(); // ���� ������� �޼���
            }
        }

        // if (Input.GetKeyDown(KeyCode.P))
        // {
        //     if (equipUIManager)
        //     {
        //         equipUIManager.ToggleEquipUIPanel();
        //         equipUIManager.UpdateEquipUIPanel();
        //     }
        // }
    }

    private void Start()
    {
        if (inventoryManager != null)
            inventoryManager.gameObject.SetActive(false);

        if (equipUIManager != null)
            equipUIManager.gameObject.SetActive(false);
    }

    public void SetEquipUI()
    {
        if (equipUIManager)
        {
            equipUIManager.ToggleEquipUIPanel();
            equipUIManager.UpdateEquipUIPanel();
        }
    }
}
