using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CombatEffectManager : MonoBehaviour
{
    public static CombatEffectManager Instance;

    private bool isHitStopping = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Hit Stop
    public void DoHitStop(float duration)
    {
        if (!isHitStopping)
            StartCoroutine(HitStopCoroutine(duration));
    }

    private IEnumerator HitStopCoroutine(float duration)
    {
        float originalTimeScale = 1;

        isHitStopping = true;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = originalTimeScale;
        isHitStopping = false;
    }

    // Ease Hit Stop
    public void EasedHitStop(float dipValue = 0.2f, float dipDuration = 0.05f, float recoverDuration = 0.08f)
    {
        StartCoroutine(HitStopEasedRoutine(dipValue, dipDuration, recoverDuration));
    }

    private IEnumerator HitStopEasedRoutine(float dipValue, float dipDuration, float recoverDuration)
    {
        // 1. 타임스케일 감소
        Time.timeScale = dipValue;
        yield return new WaitForSecondsRealtime(dipDuration);

        // 2. 복귀 애니메이션 (부드럽게)
        float t = 0f;
        float start = dipValue;
        while (t < recoverDuration)
        {
            t += Time.unscaledDeltaTime;
            float normalized = t / recoverDuration;

            // Ease-out cubic: 빠르게 회복되다가 느리게 멈춤
            float scale = Mathf.Lerp(start, 1f, 1f - Mathf.Pow(1f - normalized, 3f));
            Time.timeScale = scale;
            yield return null;
        }

        Time.timeScale = 1f;
    }

    private float EaseOutCubic(float t) => 1f - Mathf.Pow(1f - t, 3);
    private float EaseInCubic(float t) => Mathf.Pow(t, 3);

    // Camera Shake
    public void DoCameraShake(float duration, float shakeAmountX, float shakeAmountY)
    {
        StartCoroutine(CameraShakeCoroutine(duration, shakeAmountX, shakeAmountY));
    }

    private IEnumerator CameraShakeCoroutine(float duration, float shakeAmountX, float shakeAmountY)
    {
        Transform cam = Camera.main.transform;
        Vector3 originalPos = cam.position;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeAmountX;
            float offsetY = Random.Range(-1f, 1f) * shakeAmountY;

            cam.position = new Vector3(originalPos.x + offsetX, originalPos.y + offsetY, originalPos.z);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        cam.position = originalPos;
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        GameManager.inst.combatEffectManager = this;
    }
}