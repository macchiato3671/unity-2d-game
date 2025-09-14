using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using static RoomManager.RoomType; // 룸타입 편하게 사용

public class RoomManager : MonoBehaviour
{
    public enum RoomType { MobRoom, RewardRoom, EventRoom, BossRoom };

    [SerializeField] private CinemachineConfiner2D cineCam;
    [SerializeField] private CinemachineConfiner2D cutsceneCam;

    [SerializeField] private GameObject[] mobRooms;
    [SerializeField] private GameObject[] rewardRooms;
    [SerializeField] private GameObject[] eventRooms;
    [SerializeField] private GameObject[] bossRooms;

    public Room curRoom; // 첫 방은 임시로 직접 등록
    RoomType curRoomType;
    int _roomCount;
    public int roomCount
    {
        get
        {
            return _roomCount;
        }
        set
        {
            _roomCount = value;
            Debug.Log($"roomCount = {value}");
            //if(_roomCount == 5) _roomCount = 1;
        }
    } // 거쳐간 방 수 카운트 및 초기화


    void Awake()
    {
        if (GameManager.inst != null) GameManager.inst.roomManager = this;
    }

    void Start()
    {
        // 스타트룸 설정
        GameManager.inst.player.transform.position = curRoom.GetSpawnPoint();
        cineCam.BoundingShape2D = curRoom.GetCameraConfiner();
        if (cutsceneCam) cutsceneCam.BoundingShape2D = curRoom.GetCameraConfiner();
        curRoom.SetWind();
        GameManager.inst.player.SetConfiner((int)curRoom.GetPortalPos().x);
        GameManager.inst.enemyManager.Spawn(curRoom.GetEnemySpawnPoints());
        curRoom.SetPortalState(true);
        GameManager.inst.screenUI.SetClearConditionUI(2, "시작 지점");
        roomCount = 0;
    }

    public void MoveRoom(RoomType type)
    {
        if (curRoom != null) Destroy(curRoom.gameObject); // 기존 방 제거

        switch (type)
        {
            case MobRoom:
                curRoom = Instantiate(mobRooms[Random.Range(0, mobRooms.Length)], transform).GetComponent<Room>();
                break;
            case RewardRoom:
                curRoom = Instantiate(rewardRooms[Random.Range(0, rewardRooms.Length)], transform).GetComponent<Room>();
                break;
            case EventRoom:
                curRoom = Instantiate(eventRooms[Random.Range(0, eventRooms.Length)], transform).GetComponent<Room>();
                break;
            case BossRoom:
                float clearRate = GameManager.inst.ClearPercent[1].rate;
                int idx = 0;
                if (clearRate < 0.1f) idx = 0;
                else if(clearRate < 0.3f) idx = 1;
                else if(clearRate < 0.5f) idx = 2;
                else if(clearRate < 0.7f) idx = 3;
                else if(clearRate < 0.9f) idx = 4;

                curRoom = Instantiate(bossRooms[idx], transform).GetComponent<Room>();
                break;
        }
        curRoomType = type;

        GameManager.inst.screenUI.subUIManager.GetComponent<UIManager>().inventoryManager.weaponCanChange = false;

        // 플레이어 위치 옮기기
        GameManager.inst.player.transform.position = curRoom.GetSpawnPoint();

        // 카메라 한정 범위 재설정
        cineCam.BoundingShape2D = curRoom.GetCameraConfiner();
        if (cutsceneCam) cutsceneCam.BoundingShape2D = curRoom.GetCameraConfiner();

        // 카메라 즉시 원상복구(컷신 도중 맵 이동시)
        cutsceneCam.GetComponent<CinemachineCamera>().Priority = 0;

        // 바람 설정
        curRoom.SetWind();

        // 플레이어 위치 한정 설정
        GameManager.inst.player.SetConfiner((int)curRoom.GetPortalPos().x);

        // 룸 몬스터 소환 요청
        GameManager.inst.enemyManager.Spawn(curRoom.GetEnemySpawnPoints());

        // 기존 투사체 비활성화
        GameManager.inst.pool.DisableRange();

        // ClearCondition UI 설정 및 포탈 활성화 여부 설정
        switch (type)
        {
            case MobRoom:
                GameManager.inst.screenUI.SetClearConditionUI(0, "모든 적 처치");
                curRoom.SetPortalState(false);
                break;
            case RewardRoom:
                SetRoomClear();
                break;
            case EventRoom:
                SetRoomClear();
                break;
            case BossRoom:
                GameManager.inst.screenUI.SetClearConditionUI(0, "보스 처치");
                curRoom.SetPortalState(false);
                break;
        }
        // 거쳐간 방 개수 + 1
        roomCount++;

        GameManager.inst.hackModeManager.ResetHackLimit(); 
    }

    public void SetNewConfiner(Collider2D confiner)
    {
        StartCoroutine(Confine(confiner));
    }

    IEnumerator Confine(Collider2D confiner)
    {
        yield return new WaitForSeconds(0.25f);
        cineCam.BoundingShape2D = confiner;
    }

    public void SetRoomClear()
    {
        curRoom.SetPortalState(true);
        GameManager.inst.screenUI.subUIManager.GetComponent<UIManager>().inventoryManager.weaponCanChange = true;
        GameManager.inst.screenUI.SetClearConditionUI(1, "포탈 활성화");
        if (roomCount == 1 && curRoomType == MobRoom) StartCoroutine(SetClearCamera()); //맨 첫번째 맵 플레이시에만 실행하는 식으로
    }

    IEnumerator SetClearCamera()
    {
        Debug.Log($"카메라 조정됨. roomCount = {roomCount}");
        CinemachineCamera cam = cutsceneCam.GetComponent<CinemachineCamera>();
        cam.Follow = curRoom.GetPortal().transform;
        cam.Priority = 2;
        yield return new WaitForSeconds(2.5f);
        cam.Priority = 0;
    }
    
}
