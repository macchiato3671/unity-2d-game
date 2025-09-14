using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] Text text;

    [TextArea(5,10)]
    [SerializeField] private string fullText;

    CanvasGroup canvasGroup;
    [SerializeField] GameManager gameManager;

    bool skip = false;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (GameManager.inst != null) gameManager = GameManager.inst;
        StartCoroutine(TypeText());
    }

    void Update()
    {
        if (Input.anyKeyDown) skip = true;
    }

    IEnumerator TypeText()
    {
        float curTime = 0f, maxTime = 1f;

        gameManager.IsPaused = true;

        yield return new WaitForSecondsRealtime(1f);
        for (int i = 0; i < fullText.Length; i++)
        {
            text.text += fullText[i];
            yield return new WaitForSecondsRealtime(skip ? 0f : 0.04f);
        }
        yield return new WaitForSecondsRealtime(5f);

        while (curTime < maxTime)
        {
            curTime += Time.unscaledDeltaTime;
            canvasGroup.alpha = 1 - curTime / maxTime;
            yield return new WaitForSecondsRealtime(0);
        }
        gameManager.IsPaused = false;
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        
    }
}
