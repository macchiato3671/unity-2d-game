using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

public class BaseManager : MonoBehaviour
{
    [SerializeField] private CinemachineConfiner2D cineCam;
    [SerializeField] private CinemachineConfiner2D cutsceneCam;

    public Room curRoom;

    void Awake()
    {
        if (GameManager.inst != null) GameManager.inst.baseManager = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.inst.screenUI.SetPauseUI();
        }
    }

    public void Init()
    {
        GameManager.inst.basePlayer.transform.position = curRoom.GetSpawnPoint();
        cineCam.BoundingShape2D = curRoom.GetCameraConfiner();
        if (cutsceneCam) cutsceneCam.BoundingShape2D = curRoom.GetCameraConfiner();

        GameManager.inst.screenUI.SetClearConditionUI(2, "탐사 기지");
        GameManager.inst.screenUI.SetDeathConditionUI(String.Format("악성코드 추적도\n{0:F2}%", GameManager.inst.DeathPercent * 100));
        if (GameManager.inst.clear) StartCoroutine(EndGame(true));
        if(GameManager.inst.gameOver) StartCoroutine(EndGame(false));
    }

    IEnumerator EndGame(bool flag)
    {
        GameManager.inst.screenUI.SetEndUI(flag);
        GameManager.inst.IsPaused = true;
        yield return new WaitForSecondsRealtime(5f);
        yield return StartCoroutine(GameManager.inst.screenUI.SetSceneChangeUI());
        if (flag)
        {
            Destroy(GameManager.inst.gameObject);
            SceneManager.LoadScene("Title");
        }
        else
        {
            SaveSystem.DeleteSave("save_1");
            Destroy(GameManager.inst.gameObject);
            SceneManager.LoadScene("Title");
        }
    }
}
