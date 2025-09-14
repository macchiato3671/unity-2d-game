using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private Image blinder;
    [SerializeField] private Text caution;
    [SerializeField] private Text saveInfo;

    [SerializeField] private Button[] buttons;
    [SerializeField] private Text[] texts;

    [SerializeField] private GameObject settingUI;

    [SerializeField] private Texture2D cursor;

    List<String> files;
    int clickCnt = 0;

    Coroutine toStop;

    void Start()
    {
        files = SaveSystem.GetSaveFiles();
        if (files.Count != 0)
        {
            saveInfo.text = "마지막 저장\n" +
                            File.GetLastWriteTime(Path.Combine(Application.persistentDataPath, files[0] + ".json")).ToString("yyyy-MM-dd HH:mm");
        }
        toStop = StartCoroutine(SetSceneLoadedUI());
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
    }

    public void OnButtonClick(int buttonIdx)
    {
        switch (buttonIdx)
        {
            case 0: // 새로 시작 버튼
                if (files.Count != 0 && clickCnt++ == 0) caution.enabled = true;
                else StartGame(false);
                break;
            case 1: // 이어서 하기 버튼
                StartGame(true);
                break;
            case 2: // 설정 버튼
                SetSettingUI(true);
                break;
            case 3: // 게임 종료 버튼
                ExitGame();
                break;
        }
    }

    void StartGame(bool load)
    {
        if (!load)
        {
            SaveSystem.DeleteSave("save_1");
            StartCoroutine(ChangeSceneCoroutine("Tutorial"));
        }
        else
        {
            if (files.Count != 0)
            {
                SaveData data = SaveSystem.Load(files[0]);
                if (data != null)
                {
                    SaveSystem.dataToLoad = data;
                    StartCoroutine(ChangeSceneCoroutine("ExploreBase"));
                }
                else
                {
                    Debug.LogWarning("세이브파일이 존재하나 실제 데이터를 가져오는데 실패");
                }
            }
            else Debug.LogWarning("세이브파일 없음");
        }
    }

    void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    IEnumerator SetSceneChangeUI()
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
    }

    public IEnumerator SetSceneLoadedUI()
    {
        float curTime = 0f;
        float maxTime = 1f;
        Color startColor = new Color(0, 0, 0, 1);
        Color endColor = new Color(0, 0, 0, 0);

        ShowUI(blinder.gameObject);
        blinder.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        yield return new WaitForSeconds(0.75f);

        while (curTime <= maxTime)
        {
            curTime += Time.fixedDeltaTime;
            blinder.color = Color.Lerp(startColor, endColor, curTime / maxTime);
            yield return new WaitForFixedUpdate();
        }
        HideUI(blinder.gameObject);
    }

    IEnumerator ChangeSceneCoroutine(string sceneName)
    {
        if(toStop != null) StopCoroutine(toStop);
        yield return StartCoroutine(SetSceneChangeUI());
        SceneManager.LoadScene(sceneName);
    }

    public void SetSettingUI(bool flag)
    {
        SettingUI ui = settingUI.GetComponent<SettingUI>();
        ui.Init();
        if (flag && ui.isActive) return;
        StartCoroutine(UIAnimation(settingUI,flag));
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
            target.GetComponent<SettingUI>()?.SetGroupActive(true);
        }
        else
        {
            savedScale.x = 0f;
            target.transform.localScale = savedScale;
            HideUI(target);
        }
    }



    void ShowUI(GameObject ui)
    {
        if (ui == null) return;
        CanvasGroup cg = ui.GetComponent<CanvasGroup>();
        if (cg == null) return;
        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }
    
    void HideUI(GameObject ui)
    {
        if (ui == null) return;
        CanvasGroup cg = ui.GetComponent<CanvasGroup>();
        if (cg == null) return;
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }
}
