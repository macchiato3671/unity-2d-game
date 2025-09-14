using UnityEditor;
using UnityEngine;

public class HackModeManager : MonoBehaviour
{
    public HackSkillData selectedSkill;
    [SerializeField] private AudioClip hackSound;
    [SerializeField] private AudioClip hackStartSound;

    private bool isAwaitingTarget = false;
    private bool hasUsedInThisRoom = false;
    private float bossCooldownTimer = 0f;
    private float hackModeTimer = 0f; // 해킹 모드 타이머 추가

    private RoomManager roomManager;

    float savedTimeScale;
    float defaultFixedDeltaTime;

    public bool isTutorial = false;

    [SerializeField] private Texture2D[] cursor;

    void Awake()
    {
        if (GameManager.inst != null) GameManager.inst.hackModeManager = this;
    }

    void Start()
    {
        if (GameManager.inst != null)
        {
            selectedSkill = GameManager.inst.equippedHackSkill;
        }
        roomManager = GameManager.inst.roomManager;
    }

    void Update()
    {
        // 보스룸일 때 쿨타임 감소
        if (roomManager.curRoom.GetRoomType() == RoomManager.RoomType.BossRoom && bossCooldownTimer > 0f)
        {
            bossCooldownTimer -= Time.unscaledDeltaTime;
        }
        if (bossCooldownTimer < 0f)
        {
            bossCooldownTimer = 0f;
            GameManager.inst.screenUI.SetHackIcon(true);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (isAwaitingTarget)
            {
                // 해킹 모드 중이면 종료
                CancelHackMode();
            }
            else
            {
                TryActivateSkill();
            }
        }

        if (isAwaitingTarget)
        {
            hackModeTimer -= Time.unscaledDeltaTime;
            if (hackModeTimer <= 0f)
            {
                CancelHackMode();
            }

            if (Input.GetMouseButtonDown(0))
            {
                TryTargetedSkill();
            }
        }
    }

    public void TryActivateSkill()
    {
        if (selectedSkill == null) return;

        var roomType = roomManager.curRoom.GetRoomType();

        if (roomType == RoomManager.RoomType.BossRoom)
        {
            if (bossCooldownTimer > 0f)
            {
                Debug.Log($"[HACK] 보스방 쿨타임 중... {bossCooldownTimer:F1}s 남음");
                return;
            }
        }
        else
        {
            if (hasUsedInThisRoom)
            {
                Debug.Log("[HACK] 이 방에서는 이미 해킹을 사용했습니다.");
                return;
            }
        }

        isAwaitingTarget = true;
        hackModeTimer = 5f; // 해킹 모드 유지 시간 5초로 설정

        savedTimeScale = Time.timeScale;
        defaultFixedDeltaTime = Time.fixedDeltaTime;
        Time.timeScale = 0.2f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        ChangeCursor(1);

        if (hackStartSound != null)
            SoundManager.inst.PlaySFX(hackStartSound);
        Debug.Log($"[HACK] {selectedSkill.skillName} 대상 선택 대기...");
    }

    void TryTargetedSkill()
    {
        isAwaitingTarget = false;
        Time.timeScale = savedTimeScale;
        Time.fixedDeltaTime = defaultFixedDeltaTime;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, LayerMask.GetMask("Enemy"));

        if (hit.collider == null)
        {
            CancelHackMode();
            return;
        }

        EnemyHackable target = hit.collider.GetComponent<EnemyHackable>();
        if (target != null)
        {
            selectedSkill.Execute(target);
            GameManager.inst.screenUI.SetHackIcon(false);
            ChangeCursor(0);

            if (hackSound != null)
                SoundManager.inst.PlaySFX(hackSound);

            var roomType = roomManager.curRoom.GetRoomType();
            if (roomType == RoomManager.RoomType.BossRoom)
            {
                bossCooldownTimer = 30f;
            }
            else
            {
                hasUsedInThisRoom = true;
            }
        }
    }

    void CancelHackMode()
    {
        isAwaitingTarget = false;
        Time.timeScale = savedTimeScale;
        Time.fixedDeltaTime = defaultFixedDeltaTime;

        ChangeCursor(0);

        Debug.Log("[HACK] 해킹 모드가 종료되었습니다.");
    }

    // RoomManager에서 방 이동 시 호출
    public void ResetHackLimit()
    {
        hasUsedInThisRoom = false;
        bossCooldownTimer = 0f;
        GameManager.inst.screenUI.SetHackIcon(true);
    }

    void ChangeCursor(int idx)
    {
        if (cursor.Length <= idx) return;
        Cursor.SetCursor(cursor[idx], Vector2.zero, CursorMode.Auto);
    }
}
