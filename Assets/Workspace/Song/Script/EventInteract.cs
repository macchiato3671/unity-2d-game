using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static EventInteract.EventType;

public class EventInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject indicator;
    [SerializeField] private Sprite[] indiSprites;
    [SerializeField] private Transform[] posArr;
    SpriteRenderer indSprite;

    public bool isActive { get; set; }

    public enum EventType { Repair, GetResource, RandomBox, SelfHarm, SelfHarmAfter, Buff, SpeedBuff, SpeedDebuff, Tutorial, Shop, ShopAfter }
    [SerializeField] private EventType eventType;

    bool isInteractable = true;
    bool isSelectable = false;

    float[] randomBoxWeight = { 0.7f, 0.3f }; // 랜덤박스 비중(순서대로 부품 보상, 몬스터 출현)
    float[] buffWeight = { 0.5f, 0.5f }; // 버프 비중(순서대로 속도 버프, 속도 디버프)

    [SerializeField] private AudioClip positiveSound;
    [SerializeField] private AudioClip negativeSound;

    void Awake()
    {
        indSprite = indicator.GetComponent<SpriteRenderer>();
        isActive = false;
        if (isActive) indSprite.sprite = indiSprites[0];
        else if (isInteractable) indSprite.sprite = indiSprites[1];
    }

    public void Interact()
    {
        if (!isInteractable) return;
        ChangeActive(!isActive);
    }

    public void Cancel()
    {
        if (isSelectable) ChangeActive(false);
    }

    void ChangeActive(bool flag)
    {
        string eventText = null;
        isActive = flag;

        if (isActive)
        {
            switch (eventType)
            {
                // 보상 방 
                case Repair:
                    eventText = "로봇이 수리되었다.";
                    GameManager.inst.player.HealPlayer(2);
                    if(positiveSound) SoundManager.inst.PlaySFX(positiveSound);
                    isInteractable = false;
                    isSelectable = false;
                    StartCoroutine(DisableByTime());
                    break;
                case GetResource:
                    eventText = "부품을 대량 획득했다!";
                    ResourceManager.CollectResource(20);
                    if(positiveSound) SoundManager.inst.PlaySFX(positiveSound);
                    isInteractable = false;
                    isSelectable = false;
                    StartCoroutine(DisableByTime());
                    break;

                // 이벤트 방(보통 버튼 누르면 추가 처리 하는 방식)
                case RandomBox:
                    eventText = "수상한 상자다. 뭐가 들어있는지 확인해볼까?";
                    isSelectable = true;
                    break;
                case SelfHarm:
                    eventText = "당신은 스스로를 부숴서 부품을 좀 얻을 수 있을 듯하다...";
                    isSelectable = true;
                    break;
                case SelfHarmAfter:
                    eventText = "체력을 잃은 대신 부품을 얻었다!";
                    GameManager.inst.player.DealPlayer(2);
                    ResourceManager.CollectResource(30);
                    if(positiveSound) SoundManager.inst.PlaySFX(positiveSound);
                    isInteractable = false;
                    isSelectable = false;
                    StartCoroutine(DisableByTime());
                    break;
                case Buff:
                    eventText = "로봇을 일시적으로 개조할 수 있을 듯하다!\n성공할지는 모르겠다...";
                    isSelectable = true;
                    break;
                case SpeedBuff:
                    eventText = "로봇이 빨라졌다!";
                    GameManager.inst.player.AddPlayerSpeedFactor(0.3f);
                    if(positiveSound) SoundManager.inst.PlaySFX(positiveSound);
                    isInteractable = false;
                    isSelectable = false;
                    StartCoroutine(DisableByTime());
                    break;
                case SpeedDebuff:
                    eventText = "로봇이 느려졌다...";
                    GameManager.inst.player.AddPlayerSpeedFactor(-0.2f);
                    if(negativeSound) SoundManager.inst.PlaySFX(negativeSound);
                    isInteractable = false;
                    isSelectable = false;
                    StartCoroutine(DisableByTime());
                    break;
                case Shop:
                    eventText = "아이템 판매점이 있다! 주인은 멀쩡한거 같다..\n물건을 둘러볼까?";
                    isSelectable = true;
                    break;
                case ShopAfter:
                    // 상점 UI 오픈 로직 구현해야 하는 부분입니다!!
                    var shop = GameManager.inst.screenUI.GetShopUI();
                    shop.GetComponent<ShopUIManager>().GenerateRandomShopItems();
                    shop.GetComponent<ShopUIManager>().PopulateShop();
                    //GameManager.inst.screenUI.ShowUI(shop);
                    shop.GetComponent<ShopUIManager>().SetShop(true);


                    //
                    isInteractable = false;
                    isSelectable = false;
                    StartCoroutine(DisableByTime(0f));
                    break;
                // 튜토리얼
                case Tutorial:
                    eventText = "상호작용에 성공했다!";
                    isInteractable = false;
                    isSelectable = false;
                    StartCoroutine(DisableByTime());
                    break;
            }
        }
        GameManager.inst.screenUI.SetEventUI(isActive, eventText, isSelectable, this);

        if (isActive) indSprite.sprite = indiSprites[0];
        else if (isInteractable) indSprite.sprite = indiSprites[1];
    }

    IEnumerator DisableByTime(float time = 2f)
    {
        yield return new WaitForSeconds(time);
        ChangeActive(false);
    }

    int GetWeightedRandom(float[] weight)
    {
        float total = 0f, randWeight, cumWeight = 0f;
        foreach (float w in weight)
            total += w;

        randWeight = Random.Range(0, total);

        for (int i = 0; i < weight.Length; i++)
        {
            cumWeight += weight[i];
            if (randWeight < cumWeight)
                return i;
        }
        return -1; // Error
    }

    public void ButtonClick()
    {
        int rand;
        isInteractable = false;
        // 이벤트별 처리
        switch (eventType)
        {
            case RandomBox:
                rand = GetWeightedRandom(randomBoxWeight);
                switch (rand)
                {
                    case 0:
                        eventType = GetResource;
                        ChangeActive(true);
                        break;
                    case 1:
                        if(negativeSound) SoundManager.inst.PlaySFX(negativeSound);
                        foreach (var pos in posArr)
                            GameManager.inst.enemyManager.SpawnAuto(0, pos.position);
                        ChangeActive(false);
                        break;
                }
                break;
            case SelfHarm:
                eventType = SelfHarmAfter;
                ChangeActive(true);
                break;
            case Buff:
                rand = GetWeightedRandom(buffWeight);
                switch (rand)
                {
                    case 0:
                        eventType = SpeedBuff;
                        ChangeActive(true);
                        break;
                    case 1:
                        eventType = SpeedDebuff;
                        ChangeActive(true);
                        break;
                }
                break;
            case Shop:
                eventType = ShopAfter;
                ChangeActive(true);
                break;
        }
    }
}
