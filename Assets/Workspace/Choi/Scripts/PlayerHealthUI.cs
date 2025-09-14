using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup hackIcon;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public Sprite redheart;

    [SerializeField] private GameObject hpPrefab;
    private PlayerHealth health;

    List<Image> hpList = new List<Image>();

    void Start()
    {
        health = GameManager.inst.player.Health;

        if (health != null)
        {
            health.OnHealthChanged += UpdateHearts;
            health.OnPlayerDie += ShowGameOver;
        }

        float curX = -11f, curY = -1.98f;
        for (int i = 0; i < health.maxHP; i++)
        {
            RectTransform rect = Instantiate(hpPrefab, transform).GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(curX, curY);
            hpList.Add(rect.GetComponent<Image>());
            curX += 9f;

        }
        //gameOverPanel.SetActive(false); // 시작 시 비활성화
    }

    void UpdateHearts(int currentHP, int maxHP)
    {
        StartCoroutine(ChangeHearts(currentHP, maxHP));
    }

    IEnumerator ChangeHearts(int currentHP, int maxHP)
    {
        for (int i = 0; i < hpList.Count; i++)
            hpList[i].sprite = redheart;

        yield return new WaitForSeconds(0.15f);

        for (int i = 0; i < hpList.Count; i++)
            hpList[i].sprite = i < currentHP ? fullHeart : emptyHeart;
    }

    void ShowGameOver()
    {
        Debug.Log("Game Over UI 표시");
        GameManager.inst.screenUI.EnableGameOverUI();
    }

    public void SetHackIcon(bool flag)
    {
        hackIcon.alpha = flag ? 1 : 0.2f;
    }
}
