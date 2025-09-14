using System.Collections;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using static RoomManager.RoomType; // 룸매니저의 룸타입 enum 편하게 사용

public class Portal : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Light2D portalLight;
    [SerializeField] private ParticleSystem particle;

    public bool isUsable;
    bool isUsed = false;

    public bool isTutorial = false;

    void Awake()
    {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.4f);
    }

    void OnTriggerStay2D(Collider2D coll)
    {
        if (isUsed) return;

        if (coll.CompareTag("Player") && isUsable)
        {
            if (!isTutorial)
            {
                isUsed = true;
                if (GameManager.inst.roomManager.roomCount != 5) // 보스룸의 포탈 아닐시
                    GameManager.inst.screenUI.EnableMoveRoomUI();
                else
                { // 보스룸의 포탈일시
                    GameManager.inst.screenUI.EnableClearUI();
                }
            }
            else
            {
                EndTutorial();
            }
        }

    }

    public void SetUsable(bool flag)
    {
        isUsable = flag;
        if (isUsable)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.8f);
            particle.gameObject.SetActive(true);
            StartCoroutine(SetLight());
        }
        else
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.4f);
            portalLight.intensity = 0f;
            particle.gameObject.SetActive(false);
        }
    }

    IEnumerator SetLight()
    {
        float curTime = 0, maxTime = 4f;
        float preservedIntensity = 5f;

        while (curTime < maxTime)
        {
            curTime += Time.fixedDeltaTime;
            portalLight.intensity = preservedIntensity * (curTime / maxTime);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator ChangeSceneCoroutine(string sceneName)
    {
        yield return StartCoroutine(GameManager.inst.screenUI.SetSceneChangeUI());
        Destroy(GameManager.inst.gameObject);
        SceneManager.LoadScene(sceneName);
    }

    public void EndTutorial()
    {
        if (isUsed) return;
        isUsed = true;
        StartCoroutine(ChangeSceneCoroutine("ExploreBase"));
    }
}
