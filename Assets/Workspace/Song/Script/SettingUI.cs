using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [SerializeField] private GameObject group;
    [SerializeField] private Button exitButton;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    public bool isActive = false;

    public void Init()
    {
        Debug.Log("세팅 UI 초기화됨");
        float bgmValue = PlayerPrefs.GetFloat("BGMVolume",0.6f);
        float sfxValue = PlayerPrefs.GetFloat("SFXVolume",0.6f);

        bgmSlider.value = bgmValue;
        sfxSlider.value = sfxValue;

        SetBGMVolume(bgmValue);
        SetSFXVolume(sfxValue);

        exitButton.onClick.RemoveAllListeners();
        bgmSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();

        exitButton.onClick.AddListener(() => ExitButton());
        bgmSlider.onValueChanged.AddListener((float val) => SetBGMVolume(val));
        sfxSlider.onValueChanged.AddListener((float val) => SetSFXVolume(val));
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
        isActive = flag;
    }

    void ExitButton()
    {
        transform.GetComponentInParent<TitleUI>()?.SetSettingUI(false);
        transform.GetComponentInParent<ScreenUI>()?.SetSettingUI(false);
    }

    void SetBGMVolume(float value)
    {
        SoundManager.inst.SetBGMVolume(value);
    }

    void SetSFXVolume(float value)
    {
        SoundManager.inst.SetSFXVolume(value);
    }
}
