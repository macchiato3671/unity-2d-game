using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Screen Overlay UI를 담당하는 스크립트
public class ScreenUI : MonoBehaviour
{
    // UI의 각 요소 중 다른 오브젝트에서 GameManager를 통해서 접근해야 하면 등록
    // 다른 경우에도 가능
    [SerializeField] private Image blinder; // 페이드아웃 효과를 주기 위한 이미지
    [SerializeField] private GameObject moveRoomUI; // 방 이동을 위한 UI
    [SerializeField] private GameObject hackSkillUI;
    [SerializeField] private GameObject playerHealthUI;
    [SerializeField] private GameObject playerEnergyUI;
    [SerializeField] private GameObject weaponSelectUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject clearConditionUI;

    [SerializeField] private GameObject selectWorldUI;
    [SerializeField] private GameObject deathConditionUI;
    [SerializeField] private GameObject clearUI;
    [SerializeField] private GameObject eventUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject settingUI;
    [SerializeField] private GameObject endUI;
    [SerializeField] private GameObject resourceUI;
    [SerializeField] private BossHealthUI bossHealthUI;
    [SerializeField] private GameObject shopUI;
    [SerializeField] private GameObject tutoSkip;
    public GameObject subUIManager;

    public Coroutine coroutineToStop;

    void Awake()
    {
        if (GameManager.inst != null) GameManager.inst.screenUI = this;
    }

    // 공통
    public void HideUI(GameObject ui)
    {
        if (ui == null) return;
        CanvasGroup cg = ui.GetComponent<CanvasGroup>();
        if (cg == null) return;
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    public void HideUI()
    {
        HideUI(hackSkillUI);
        HideUI(playerHealthUI);
        HideUI(playerEnergyUI);
        HideUI(weaponSelectUI);
        HideUI(clearConditionUI);
        HideUI(deathConditionUI);
        HideUI(resourceUI);
        HideUI(shopUI);
        HideUI(tutoSkip);
    }

    public void ShowUI(GameObject ui)
    {
        if (ui == null) return;
        CanvasGroup cg = ui.GetComponent<CanvasGroup>();
        if (cg == null) return;
        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    public void ShowUI()
    {
        ShowUI(hackSkillUI);
        ShowUI(playerHealthUI);
        ShowUI(playerEnergyUI);
        ShowUI(weaponSelectUI);
        ShowUI(clearConditionUI);
        ShowUI(deathConditionUI);
        ShowUI(resourceUI);
        ShowUI(shopUI);
        ShowUI(tutoSkip);
    }


    // 개별
    public GameObject GetShopUI()
    {
        return shopUI;
    }

    public void EnableMoveRoomUI()
    {
        HideUI();
        HideUI(eventUI);
        if (coroutineToStop != null) StopCoroutine(coroutineToStop);
        coroutineToStop = StartCoroutine(FadeoutAtStart(moveRoomUI.gameObject));
    }

    public void DisableMoveRoomUI()
    {
        ShowUI();
        if (coroutineToStop != null) StopCoroutine(coroutineToStop);
        coroutineToStop = StartCoroutine(FadeoutAtEnd());
    }

    public void EnableGameOverUI()
    {
        ShowUI(gameOverUI);
    }

    public void EnableClearUI()
    {
        ShowUI(clearUI);
    }

    public void SetEventUI(bool flag, string txt, bool buttonActive, EventInteract target)
    {
        EventUI ui = eventUI.GetComponent<EventUI>();
        ui.SetState(txt);
        ui.SetTextActive(false);
        ui.SetButtonActive(buttonActive, target);
        StartCoroutine(EventUIAnimation(flag));
    }

    public void SetClearConditionUI(int iconIdx, string txt)
    {
        if (clearConditionUI == null) return;
        clearConditionUI.GetComponent<ClearConditionUI>().SetState(iconIdx, txt);
    }

    public void SetDeathConditionUI(string txt)
    {
        if (deathConditionUI == null) return;
        deathConditionUI.GetComponent<DeathConditionUI>().SetState(txt);
    }

    public void SetWeaponSelectUI(int weaponIdx)
    {
        weaponSelectUI.GetComponent<WeaponSelectUI>().UpdateUI(weaponIdx);
    }

    public void SetSelectWorldUI(bool flag)
    {
        SelectWorldUI ui = selectWorldUI.GetComponent<SelectWorldUI>();
        ui.SetClearPercent();
        StartCoroutine(UIAnimation(selectWorldUI, flag));
    }

    public void SetPauseUI(bool flag)
    {
        PauseUI ui = pauseUI.GetComponent<PauseUI>();
        StartCoroutine(UIAnimation(pauseUI, flag));

        if (!flag)
            if (settingUI.GetComponent<SettingUI>().isActive)
                StartCoroutine(UIAnimation(settingUI, false));
    }

    public void SetPauseUI()
    {
        PauseUI ui = pauseUI.GetComponent<PauseUI>();
        if (GameManager.inst.IsPaused && !ui.isActive) return;
        if (ui.isActive)
            if (settingUI.GetComponent<SettingUI>().isActive)
                StartCoroutine(UIAnimation(settingUI, false));

        StartCoroutine(UIAnimation(pauseUI, !ui.isActive));
    }

    public void SetSettingUI(bool flag)
    {
        SettingUI ui = settingUI.GetComponent<SettingUI>();
        ui.Init();
        if (flag && ui.isActive) return;
        StartCoroutine(UIAnimation(settingUI, flag));
    }

    public void SetEndUI(bool flag)
    {
        EndUI ui = endUI.GetComponent<EndUI>();
        ui.Init(flag);
        ShowUI(endUI);
        //HideUI(blinder.gameObject);
    }

    public IEnumerator SetSceneChangeUI()
    {
        float curTime = 0f;
        float maxTime = 0.5f;
        Color startColor = new Color(0, 0, 0, 0);
        Color endColor = new Color(0, 0, 0, 1);

        ShowUI(blinder.gameObject);
        HideUI();
        HideUI(selectWorldUI);
        HideUI(clearUI);
        HideUI(gameOverUI);
        HideUI(eventUI);

        while (curTime <= maxTime)
        {
            curTime += Time.fixedDeltaTime;
            blinder.color = Color.Lerp(startColor, endColor, curTime / maxTime);
            yield return new WaitForFixedUpdate();
        }
    }


    public IEnumerator SetSceneLoadedUI()
    { // 씬 로드되었을때, GameManager에서 작동
        float curTime = 0f;
        float maxTime = 1f;
        Color startColor = new Color(0, 0, 0, 1);
        Color endColor = new Color(0, 0, 0, 0);
        bool isShowed = false;

        HideUI();
        ShowUI(blinder.gameObject);
        yield return new WaitForSeconds(0.75f);

        while (curTime <= maxTime)
        {
            curTime += Time.fixedDeltaTime;
            blinder.color = Color.Lerp(startColor, endColor, curTime / maxTime);
            if (curTime > maxTime - 1f && !isShowed)
            {
                ShowUI();
                isShowed = true;
            }
            yield return new WaitForFixedUpdate();
        }
        ShowUI();
        HideUI(blinder.gameObject);
    }

    IEnumerator FadeoutAtStart(GameObject target)
    {
        float curTime = 0f;
        float maxTime = 0.5f;
        Color startColor = new Color(0, 0, 0, 0);
        Color endColor = new Color(0, 0, 0, 1);

        ShowUI(blinder.gameObject);

        while (curTime <= maxTime)
        {
            curTime += Time.fixedDeltaTime;
            blinder.color = Color.Lerp(startColor, endColor, curTime / maxTime);
            yield return new WaitForFixedUpdate();
        }

        target.SetActive(true);
        Time.timeScale = 0;
        GameManager.inst.IsPaused = true;
    }

    IEnumerator FadeoutAtEnd()
    {
        float curTime = 0f;
        float maxTime = 1f;
        Color startColor = new Color(0, 0, 0, 1);
        Color endColor = new Color(0, 0, 0, 0);

        moveRoomUI.SetActive(false);
        Time.timeScale = 1;
        GameManager.inst.IsPaused = false;

        yield return new WaitForSeconds(0.75f);

        while (curTime <= maxTime)
        {
            curTime += Time.fixedDeltaTime;
            blinder.color = Color.Lerp(startColor, endColor, curTime / maxTime);
            yield return new WaitForFixedUpdate();
        }
        HideUI(blinder.gameObject);
    }

    IEnumerator EventUIAnimation(bool flag)
    {
        Vector3 savedScale = eventUI.transform.localScale;
        float curTime = 0f, maxTime = 0.15f;

        if (flag)
        {
            savedScale.x = 0f;
            eventUI.transform.localScale = savedScale;
            ShowUI(eventUI);
        }
        else
        {
            savedScale.x = 1f;
            eventUI.transform.localScale = savedScale;
        }


        while (curTime < maxTime)
        {
            if (flag) savedScale.x = curTime / maxTime;
            else savedScale.x = 1 - curTime / maxTime;
            eventUI.transform.localScale = savedScale;
            curTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (flag)
        {
            savedScale.x = 1f;
            eventUI.transform.localScale = savedScale;
            eventUI.GetComponent<EventUI>().SetTextActive(true);
        }
        else
        {
            savedScale.x = 0f;
            eventUI.transform.localScale = savedScale;
            HideUI(eventUI);
        }
    }

    IEnumerator UIAnimation(GameObject target, bool flag)
    {
        Vector3 savedScale = target.transform.localScale;
        float curTime = 0f, maxTime = 0.15f;

        if (flag)
        {
            savedScale.x = 0f;
            target.transform.localScale = savedScale;
            ShowUI(target);
        }
        else
        {
            savedScale.x = 1f;
            target.transform.localScale = savedScale;
            target.GetComponent<SelectWorldUI>()?.SetGroupActive(false);
            target.GetComponent<PauseUI>()?.SetGroupActive(false);
            target.GetComponent<SettingUI>()?.SetGroupActive(false);
        }


        while (curTime < maxTime)
        {
            if (flag) savedScale.x = curTime / maxTime;
            else savedScale.x = 1 - curTime / maxTime;
            target.transform.localScale = savedScale;
            curTime += Time.unscaledDeltaTime;
            yield return new WaitForSecondsRealtime(0);
        }

        if (flag)
        {
            savedScale.x = 1f;
            target.transform.localScale = savedScale;
            target.GetComponent<SelectWorldUI>()?.SetGroupActive(true);
            target.GetComponent<PauseUI>()?.SetGroupActive(true);
            target.GetComponent<SettingUI>()?.SetGroupActive(true);
        }
        else
        {
            savedScale.x = 0f;
            target.transform.localScale = savedScale;
            HideUI(target);
        }
    }

    public void SetHackUI(int weaponIdx)
    {
        if (hackSkillUI) hackSkillUI.GetComponent<HackSkillUI>().UpdateUI(weaponIdx);
    }

    public void UpdateHackUI(int slotIndex)
    {
        hackSkillUI.GetComponent<HackSkillUI>().TriggerSkill(slotIndex);
    }

    public void SetHackIcon(bool flag)
    {
        playerHealthUI.GetComponent<PlayerHealthUI>().SetHackIcon(flag);
    }

    public BossHealthUI GetBossUI()
    {
        return bossHealthUI;
    }
}
