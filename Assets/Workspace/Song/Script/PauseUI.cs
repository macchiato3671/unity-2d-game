using System.Collections;
using System.Data;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private GameObject group;
    [SerializeField] private Button[] buttons;
    [SerializeField] private Text[] texts;

    public bool isActive = false;

    float savedTimeScale;

    public void SetGroupActive(bool flag)
    {
        CanvasGroup cg = group.GetComponent<CanvasGroup>();
        if (cg == null) return;

        if (flag)
        {
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;

            savedTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            GameManager.inst.IsPaused = true;
        }
        else
        {
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;

            Time.timeScale = savedTimeScale;
            GameManager.inst.IsPaused = false;
        }
        isActive = flag;
    }

    public void OnButtonClick(int buttonIdx)
    {
        switch (buttonIdx)
        {
            case 0: // 저장 버튼
                SaveGame();
                break;
            case 1: // 설정 버튼
                GameManager.inst.screenUI.SetSettingUI(true);
                break;
            case 2: // 종료 버튼
                ExitGame();
                break;
            case 3: // 탐사 포기 버튼
                GameManager.inst.screenUI.SetPauseUI(false);
                GameManager.inst.player.DealPlayer(10000);
                break;
        }
    }

    void SaveGame()
    {
        SaveSystem.SaveGame();
        StartCoroutine(SetSaveButton());
    }

    IEnumerator SetSaveButton()
    {
        texts[0].text = "저장 완료!";
        buttons[0].interactable = false;
        yield return new WaitForSecondsRealtime(1f);
        texts[0].text = "저장";
        buttons[0].interactable = true;
    }

    void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
