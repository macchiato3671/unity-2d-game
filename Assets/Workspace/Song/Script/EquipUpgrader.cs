using UnityEngine;

public class EquipUpgrader : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject indicator;
    [SerializeField] private Sprite[] indiSprites;
    SpriteRenderer indSprite;
    public bool isActive { get; set; }

    void Awake()
    {
        indSprite = indicator.GetComponent<SpriteRenderer>();
        isActive = false;
        indSprite.sprite = indiSprites[1];
    }

    public void Interact()
    {
        ChangeActive(!isActive);
        //GameManager.inst.screenUI.SetSelectWorldUI(isActive);
    }

    public void Cancel()
    {
        ChangeActive(false);
        //GameManager.inst.screenUI.SetSelectWorldUI(isActive);
    }

    void ChangeActive(bool flag)
    {
        if (isActive == flag && flag == false) return;
        isActive = flag;
        GameManager.inst.screenUI.subUIManager.GetComponent<UIManager>().SetEquipUI();
        if (isActive) indSprite.sprite = indiSprites[0];
        else indSprite.sprite = indiSprites[1];
    }
}
