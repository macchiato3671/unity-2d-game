using System.Collections;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UI;

public class DamageUI : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private RectTransform rect;

    Vector2 startPos, nextPos;

    public void Init(int damage)
    {
        text.text = $"{damage}!";
        startPos = rect.position;
        nextPos = new Vector2(rect.position.x+Random.Range(-0.5f,0.5f), rect.position.y+1.2f);

        StartCoroutine(Destruction());
    }

    IEnumerator Destruction()
    {
        float curTime = 0f, maxTime = 0.6f;

        while (curTime < maxTime)
        {
            curTime += Time.fixedDeltaTime;
            rect.position = Vector2.Lerp(startPos, nextPos, curTime / maxTime);
            yield return new WaitForFixedUpdate();
        }

        gameObject.SetActive(false);
    }
}
