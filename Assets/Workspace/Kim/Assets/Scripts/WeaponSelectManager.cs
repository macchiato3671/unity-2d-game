using UnityEngine;

public class WeaponSelectManager : MonoBehaviour
{
    public WeaponData defaultWeapon1;
    public WeaponData defaultWeapon2;

    public WeaponData[] equippedWeapons = new WeaponData[2]; // 장착된 무기 2개
    public string[] generatedWeaponDescriptions = new string[2];
    public string[] generatedQSkillDescriptions = new string[2];
    public string[] generatedESkillDescriptions = new string[2];

    void Awake()
    {
        if (GameManager.inst != null) GameManager.inst.weaponSelectManager = this;
    }

    public void Init()
    {
        equippedWeapons[0] = defaultWeapon1;
        equippedWeapons[1] = defaultWeapon2;

        Debug.Log("기본 무기 자동 장착됨: " + defaultWeapon1.weaponName + ", " + defaultWeapon2.weaponName);

        FinalizeSelection(); // 한번 저장해줬습니다.
    }

    public void SelectWeapon(WeaponData newWeapon)
    {
        // 이미 장착되어 있으면 무시
        if (equippedWeapons[0] == newWeapon || equippedWeapons[1] == newWeapon)
        {
            Debug.Log("이미 장착된 무기입니다.");
            return;
        }

        // 큐처럼 밀기
        equippedWeapons[0] = equippedWeapons[1];
        equippedWeapons[1] = newWeapon;

        Debug.Log($"무기 장착됨: 1번 = {(equippedWeapons[0] ? equippedWeapons[0].weaponName : "없음")}, 2번 = {equippedWeapons[1].weaponName}");
        FinalizeSelection();
    }

    public void LoadWeapon()
    {
        equippedWeapons[0] = GameManager.inst.equippedWeapons[0];
        equippedWeapons[1] = GameManager.inst.equippedWeapons[1];

        GameManager.inst.screenUI.SetWeaponSelectUI(0);

        Debug.Log("무기 정보 WeaponSelectManager에 로드 완료!");
    }

    public void FinalizeSelection()
    {
        GameManager.inst.equippedWeapons[0] = equippedWeapons[0];
        GameManager.inst.equippedWeapons[1] = equippedWeapons[1];

        GameManager.inst.screenUI.SetWeaponSelectUI(0);

        Debug.Log("무기 정보 GameManager에 저장 완료!");
        GenerateDescriptions();
    }

    public void GenerateDescriptions()
    {
        for (int i = 0; i < equippedWeapons.Length; i++)
        {
            WeaponData weapon = equippedWeapons[i];

            if (weapon != null)
            {
                // 무기 설명 생성
                weapon.GenerateDescription();
                generatedWeaponDescriptions[i] = weapon.description;

                // Q 스킬 설명 생성
                if (weapon.qSkillData != null)
                {
                    weapon.qSkillData.GenerateDescription(weapon);
                    generatedQSkillDescriptions[i] = weapon.qSkillData.description;
                }
                else
                {
                    generatedQSkillDescriptions[i] = "스킬 Q 없음";
                }

                // E 스킬 설명 생성
                if (weapon.eSkillData != null)
                {
                    weapon.eSkillData.GenerateDescription(weapon);
                    generatedESkillDescriptions[i] = weapon.eSkillData.description;
                }
                else
                {
                    generatedESkillDescriptions[i] = "스킬 E 없음";
                }

                Debug.Log($"[{weapon.weaponName}]");
                Debug.Log($"- 무기 설명: {weapon.description}");
                Debug.Log($"- Q: {generatedQSkillDescriptions[i]}");
                Debug.Log($"- E: {generatedESkillDescriptions[i]}");
            }
            else
            {
                generatedWeaponDescriptions[i] = "장착된 무기가 없습니다.";
                generatedQSkillDescriptions[i] = "";
                generatedESkillDescriptions[i] = "";
            }
        }
    }
}
