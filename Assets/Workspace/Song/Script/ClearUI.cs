using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClearUI : MonoBehaviour
{
    [SerializeField] private Button toBaseButton;

    public void OnButtonClick()
    {
        GameManager.inst.AddClearPercent(0.2f);
        GameManager.inst.ChangeScene(0);
    }
}