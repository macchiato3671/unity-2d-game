using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public WeaponData weaponData; // 이 오브젝트가 선택될 시 넘겨줄 무기
    private bool playerInRange = false;
    private WeaponSelectManager selectManager;

    void Start()
    {
        selectManager = FindAnyObjectByType<WeaponSelectManager>();
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            selectManager.SelectWeapon(weaponData);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}