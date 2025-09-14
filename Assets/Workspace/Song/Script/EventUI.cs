using UnityEngine;
using UnityEngine.UI;

public class EventUI : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private Button button;

    EventInteract buttonTarget;

    public void SetState(string txt)
    {
        if (txt == null) return;
        text.text = txt;
    }

    public void SetTextActive(bool flag)
    {
        CanvasGroup cg = text.GetComponent<CanvasGroup>();
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

    public void SetButtonActive(bool flag, EventInteract target = null)
    {
        CanvasGroup cg = button.GetComponent<CanvasGroup>();
        if (cg == null) return;

        buttonTarget = target;
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

    public void OnButtonClick()
    {
        if(buttonTarget != null) buttonTarget.ButtonClick();
    }
}
