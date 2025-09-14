using UnityEngine;

public class StartDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject indicator;
    [SerializeField] private Sprite[] indiSprites;
    SpriteRenderer indSprite;
    public bool isActive { get; set; }

    void Awake()
    {
        indSprite = indicator.GetComponent<SpriteRenderer>();
        ChangeActive(false);
    }

    public void Interact()
    {
        ChangeActive(!isActive);
        GameManager.inst.screenUI.SetSelectWorldUI(isActive);
    }

    public void Cancel()
    {
        ChangeActive(false);
        GameManager.inst.screenUI.SetSelectWorldUI(isActive);
    }

    void ChangeActive(bool flag)
    {
        isActive = flag;
        if (isActive) indSprite.sprite = indiSprites[0];
        else indSprite.sprite = indiSprites[1];
    }
}
