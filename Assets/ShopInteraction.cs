using UnityEngine;

public class ShopInteraction : MonoBehaviour
{
    public GameObject shopUI;  // ���� UI ������Ʈ
    public ShopUIManager shopuimanager;

    private bool isNearShop = false;

    void Update()
    {
        // if (isNearShop && Input.GetKeyDown(KeyCode.G))
        // {
        //     ToggleShopUI();
        // }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))  // �浹�� ��ü�� �÷��̾���
        {
            isNearShop = true;
            print("�÷��̾ ���� ��ó�� ����");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))  // �浹�� ������ ��
        {
            isNearShop = false;
        }
    }


    void ToggleShopUI()
    {
        bool isActive = shopUI.activeSelf;
        shopUI.SetActive(isActive);

        if (!isActive)
        {
            shopuimanager.PopulateShop();
        }
    }
}
