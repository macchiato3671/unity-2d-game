using UnityEngine;

public class EndUI : MonoBehaviour
{
    [SerializeField] private GameObject clearGroup;
    [SerializeField] private GameObject gameoverGroup;

    public void Init(bool flag)
    {
        CanvasGroup cg;
        if (flag)
        {
            cg = clearGroup.GetComponent<CanvasGroup>();
            if (cg == null) return;
        }
        else
        {
            cg = gameoverGroup.GetComponent<CanvasGroup>();
            if (cg == null) return;
        }
        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }
}
