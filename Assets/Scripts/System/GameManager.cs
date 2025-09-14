using System.Collections;
using System.Numerics;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct ClearData
{
    public int worldID;
    public float rate;

    public ClearData(int id, float r)
    {
        worldID = id;
        rate = r;
    }
}

public class GameManager : MonoBehaviour
{
    // 싱글톤 패턴 사용을 위한 static 선언
    public static GameManager inst;

    // 접근 편의성을 위해 스크립트 가져오기
    // 다른 스크립트에서 자주 접근해야하는 것들 등록하고 에디터에서 설정하면 됩니다.
    public ScreenUI screenUI; // Screen Overlay UI 총괄
    public WorldUI worldUI; // World Space UI 총괄
    public BossUI bossUI; // World Space UI 총괄
    public Player player; // 플레이어 오브젝트 총괄
    public BasePlayer basePlayer; // 기지에서의 플레이어 오브젝트
    public PoolManager pool; // 오브젝트 풀링 담당
    public RoomManager roomManager; // 룸 이동 처리 담당
    public EnemyManager enemyManager; // 에너미 소환 및 초기화 담당
    public HackModeManager hackModeManager; // 해킹 관련 처리 담당
    public Light2D globalLight;
    public EquipmentManager equipmentManager;
    public ShopManager shopManager;
    public CombatEffectManager combatEffectManager; //타격감 담당(hitstop이나 camera shake)
    public BaseManager baseManager;
    public WeaponSelectManager weaponSelectManager; // 무기 장착, description 생성
    public ShopUIManager shopUIManager;
    public AccessoryDropManager accessoryDropManager;
    //public BossManager bossManager;


    ClearData[] clearPercent;
    public ClearData[] ClearPercent
    {
        get
        {
            return clearPercent;
        }
    }
    public bool clear = false;

    float deathPercent;
    public float DeathPercent
    {
        get
        {
            return deathPercent;
        }
    }
    public bool gameOver = false;

    public const int worldCount = 2; // 탐사기지 포함 월드 수
    int curWorldID = 0; // 0 : 탐사기지, 이후 순서대로 월드1, 2...

    public WeaponData[] equippedWeapons = new WeaponData[2]; // 현재 장착 무기
    public int currentWeaponIndex = 0;

    public bool IsPaused { get; set; } = false;
    public bool IsOnInventory { get; set; } = false;
    HackSkillData _equippedHackSkill;
    public HackSkillData equippedHackSkill
    {
        get
        {
            return _equippedHackSkill;
        }
        set
        {
            _equippedHackSkill = value;
            if (hackModeManager) hackModeManager.selectedSkill = value;
        }
    } //현재 해킹스킬

    public WeaponData[] defaultWeaponData;
    public EquipmentData[] defaultEquipData;
    public HackSkillData[] defaultHackData;

    void Awake()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(gameObject);

            // 타이틀 씬에서 탐사기지 씬으로 넘어오며 게임 시작->GameManager 객체가 생성될때의 처리
            clearPercent = new ClearData[worldCount];

            LoadData();

            if (baseManager) baseManager.Init();

            screenUI.SetWeaponSelectUI(0);

            int seed = System.Guid.NewGuid().GetHashCode();
            Random.InitState(seed);
        }
        else Destroy(gameObject);
    }

    public void AddClearPercent(float pToAdd)
    {
        clearPercent[curWorldID].rate += pToAdd;
        if (clearPercent[curWorldID].rate >= 0.99f)
        {
            clearPercent[curWorldID].rate = 1f;
            clear = true;
        }
    }

    public void AddDeathPercent(float pToAdd)
    {
        deathPercent += pToAdd;
        if (deathPercent >= 0.99f)
        {
            deathPercent = 1f;
            gameOver = true;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        screenUI.coroutineToStop = StartCoroutine(screenUI.SetSceneLoadedUI());
        weaponSelectManager.LoadWeapon();
        screenUI.SetHackUI(0);
        if (baseManager) baseManager.Init();
        if(player!=null) { player.UpdateEquipOn();
            Debug.Log("플레이어 장비 반영 완료");
        }
    }

    public void ChangeScene(int wIdx)
    {
        SaveSystem.SaveGame();

        // Base Scene일 경우 GameManager에서 장착한 Weapon 저장
        if (weaponSelectManager != null && curWorldID == 0)
        {
            Debug.Log("Base 씬에서 무기 선택 저장됨");
            weaponSelectManager.FinalizeSelection();
        }

        curWorldID = wIdx;

        switch (curWorldID)
        {
            case 0:
                StartCoroutine(ChangeSceneCoroutine("ExploreBase"));
                break;
            case 1:
                StartCoroutine(ChangeSceneCoroutine("Main"));
                break;
        }
    }

    IEnumerator ChangeSceneCoroutine(string sceneName)
    {
        yield return StartCoroutine(screenUI.SetSceneChangeUI());
        SceneManager.LoadScene(sceneName);
    }

    void LoadData()
    {
        SaveData data = SaveSystem.dataToLoad;
        if (data == null)
        {
            for (int i = 0; i < worldCount; i++)
                clearPercent[i] = new ClearData(i, 0f);
            deathPercent = 0f;

            HackSkillData[] allHackData = Resources.LoadAll<HackSkillData>("HackSkillData");
            foreach (var hack in allHackData)
            {
                if (hack.skillName == "Infection" || hack.skillName == "Lure")
                    hack.isget = true;
                else
                    hack.isget = false;
            }

            if (weaponSelectManager) weaponSelectManager.Init();
            else Debug.Log("WeaponSelectManager가 없습니다!!");
            ResourceManager.resourceAmount = 0;
            if (equipmentManager)
            {
                for (int i = 0; i < defaultEquipData.Length; i++)
                    equipmentManager.equipDatas[i] = defaultEquipData[i];
            }
            equipmentManager.Init_equip();
            if (player) player.UpdateEquipOn();
            equippedHackSkill = null;
        }
        else // 로드할 데이터 존재
        {
            ResourceManager.LoadResource(data.resource);
            clearPercent = data.clearPercent;
            deathPercent = data.deathPercent;

            for (int i = 0; i < defaultWeaponData.Length; i++)
            {
                if (data.equippedWeaponType[0] == defaultWeaponData[i].type)
                    equippedWeapons[0] = defaultWeaponData[i];
                if (data.equippedWeaponType[1] == defaultWeaponData[i].type)
                    equippedWeapons[1] = defaultWeaponData[i];
            }
            weaponSelectManager.LoadWeapon();

            if (equipmentManager)
            {
                for (int i = 0; i < defaultEquipData.Length; i++)
                {
                    for (int j = 0; j < data.equipDatas.Length; j++)
                    {
                        if (data.equipDatas[j].equipID == defaultEquipData[i].equipID)
                        {
                            equipmentManager.equipDatas[j] = defaultEquipData[i];
                            equipmentManager.equipDatas[j].currentLevel = data.equipDatas[j].currentLevel;
                            equipmentManager.equipDatas[j].requiredResource = data.equipDatas[j].requiredResource;
                            break;
                        }
                    }
                }
            }

            equipmentManager.UpdateSkillCooldown();
            for (int i = 0; i < defaultHackData.Length; i++)
            {
                if (data.equippedHackName == defaultHackData[i].skillName)
                    equippedHackSkill = defaultHackData[i];
            }


            HackSkillData[] allHacks = Resources.LoadAll<HackSkillData>("HackSkills");
            foreach (var hack in allHacks)
            {
                hack.isget = data.unlockedHackSkills.Contains(hack.skillName);
            }


            string[] folders = { "AccessoryData/GravityAccessory", "AccessoryData/LaserAccessory", "AccessoryData/MeleeAccessory", "AccessoryData/ThrowingAccessory" };
            foreach (var folder in folders)
            {
                AccessoryData[] accs = Resources.LoadAll<AccessoryData>(folder);
                foreach (var acc in accs)
                {
                    acc.isget = data.unlockedAccessories.Contains(acc.accessoryID);
                }
            }


            WeaponData[] allWeapons = Resources.LoadAll<WeaponData>("WeaponData");
            foreach (var weapon in allWeapons)
            {
                weapon.isget = data.unlockedWeapons.Contains(weapon.weaponID);
            }
        }
    }
}