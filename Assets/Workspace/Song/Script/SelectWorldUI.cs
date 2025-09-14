using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectWorldUI : MonoBehaviour
{
    [SerializeField] private GameObject group;
    [SerializeField] private Button[] worldButtons;
    [SerializeField] private Text[] worldText;

    public void SetClearPercent()
    {
        foreach (var i in GameManager.inst.ClearPercent){
            if (i.worldID == 0) continue;
            worldText[i.worldID - 1].text = String.Format("  탐사율 : {0:F2}%", i.rate*100);
        }
    }

    public void SetGroupActive(bool flag)
    {
        CanvasGroup cg = group.GetComponent<CanvasGroup>();
        if (cg == null) return;

        if (flag)
        {
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }
        else
        {
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
    }

    public void OnButtonClick(int wIdx)
    {
        GameManager.inst.ChangeScene(wIdx);
    }
}
