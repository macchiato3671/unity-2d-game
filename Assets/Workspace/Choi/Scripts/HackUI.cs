using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HackSkillUI : MonoBehaviour
{
    public Image[] cooldownOverlays;           // 쿨다운 진행 UI
    public Text[] cooldownTexts;    // 쿨다운 숫자
    public Image[] skillIcons;                 // Q/E/X 슬롯 아이콘

    // [Header("스킬 아이콘")]
    // public Sprite[] normalIcons; // 스킬 인덱스별 일반 아이콘
    // public Sprite[] hackIcons;   // 스킬 인덱스별 해킹 아이콘

    [Header("쿨타임 설정")]
    // float[] cooldownDurations = { 0f, 0f };
    // private float[] cooldownTimers = { 0f, 0f };
    private float[,] cooldownTimersPerWeapon = new float[2, 2]; // [무기][슬롯(Q=0, E=1)]
    private float[,] cooldownDurationsPerWeapon = new float[2, 2];

    // [Header("UI 토글")]
    // public GameObject[] cooldownOverlaysGO; // 활성/비활성용 오브젝트
    // public GameObject[] cooldownTextsGO;

    // private int[] equippedSkillIndices = new int[2]; // 슬롯별 실제 스킬 인덱스
    // private bool isHackMode = false;

    void Update()
    {
        for (int weapon = 0; weapon < 2; weapon++) // 1번, 2번 무기 모두
        {
            for (int i = 0; i < 2; i++) // Q, E 슬롯
            {
                if (cooldownTimersPerWeapon[weapon, i] > 0f)
                {
                    cooldownTimersPerWeapon[weapon, i] -= Time.deltaTime;
                    cooldownTimersPerWeapon[weapon, i] = Mathf.Max(0f, cooldownTimersPerWeapon[weapon, i]);
                }
            }
        }

        UpdateCurrentWeaponCooldownUI();
    }

    public bool IsSkillReady(int slotIndex)
    {
        return cooldownTimersPerWeapon[GameManager.inst.currentWeaponIndex, slotIndex] <= 0f;
    }

    public void TriggerSkill(int slotIndex)
    {
        cooldownTimersPerWeapon[GameManager.inst.currentWeaponIndex, slotIndex] = cooldownDurationsPerWeapon[GameManager.inst.currentWeaponIndex, slotIndex];
    }

    public void UpdateUI(int weaponIdx)
    {
        WeaponData[] weaponDatas = GameManager.inst.equippedWeapons;
        if (weaponDatas[0] == null)
        {
            Debug.Log("스킬 UI 업데이트에 문제 발생");
            return;
        }
        if (weaponIdx == 0)
        {
            skillIcons[0].sprite = weaponDatas[0].qSkillData.icon;
            skillIcons[1].sprite = weaponDatas[0].eSkillData.icon;
            cooldownDurationsPerWeapon[GameManager.inst.currentWeaponIndex, 0] = weaponDatas[0].qSkillData.cooldown;
            cooldownDurationsPerWeapon[GameManager.inst.currentWeaponIndex, 1] = weaponDatas[0].eSkillData.cooldown;
        }
        else
        {
            skillIcons[0].sprite = weaponDatas[1].qSkillData.icon;
            skillIcons[1].sprite = weaponDatas[1].eSkillData.icon;
            cooldownDurationsPerWeapon[GameManager.inst.currentWeaponIndex, 0] = weaponDatas[1].qSkillData.cooldown;
            cooldownDurationsPerWeapon[GameManager.inst.currentWeaponIndex, 1] = weaponDatas[1].eSkillData.cooldown;
        }

        UpdateCurrentWeaponCooldownUI();
    }

    void UpdateCurrentWeaponCooldownUI()
    {
        int currentWeapon = GameManager.inst.currentWeaponIndex;

        for (int i = 0; i < 2; i++)
        {
            float timer = cooldownTimersPerWeapon[currentWeapon, i];
            float duration = cooldownDurationsPerWeapon[currentWeapon, i];

            if (timer > 0f)
            {
                cooldownOverlays[i].fillAmount = timer / duration;
                cooldownTexts[i].text = Mathf.Ceil(timer).ToString();
                cooldownTexts[i].color = Color.white;
            }
            else
            {
                cooldownOverlays[i].fillAmount = 0f;
                cooldownTexts[i].text = "";
                cooldownTexts[i].color = new Color(1f, 1f, 1f, 0f);
            }
        }
    }

    // public void SetEquippedSkills(int[] skillIndices)
    // {
    //     for (int i = 0; i < 2; i++)
    //     {
    //         equippedSkillIndices[i] = (i < skillIndices.Length) ? skillIndices[i] : -1;

    //         if (equippedSkillIndices[i] == -1)
    //         {
    //             skillIcons[i].sprite = null;
    //         }
    //         else
    //         {
    //             skillIcons[i].sprite = isHackMode ? hackIcons[equippedSkillIndices[i]] : normalIcons[equippedSkillIndices[i]];
    //         }
    //     }
    // }

    // public void SetHackMode(bool isHack)
    // {
    //     isHackMode = isHack;

    //     for (int i = 0; i < 3; i++)
    //     {
    //         int skillIndex = equippedSkillIndices[i];
    //         if (skillIndex != -1)
    //         {
    //             skillIcons[i].sprite = isHackMode ? hackIcons[skillIndex] : normalIcons[skillIndex];
    //         }

    //         // 일반 모드일 때만 쿨다운 표시
    //         cooldownOverlaysGO[i].SetActive(!isHackMode);
    //         cooldownTextsGO[i].SetActive(!isHackMode);
    //     }
    // }
}
