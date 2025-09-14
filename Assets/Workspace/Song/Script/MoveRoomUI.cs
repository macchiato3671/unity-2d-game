using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static RoomManager.RoomType;
using Random = UnityEngine.Random;


public class MoveRoomUI : MonoBehaviour
{
    [SerializeField] private Button roomButton; // 버튼 프리팹
    [SerializeField] private Button firstButton; // 맨 처음에 있는 버튼

    Vector2 curButtonPos; // 이동할 수 있는 룸의 버튼을 그리기 위한, 이전에 선택한 버튼의 위치
    Button upButton, downButton; // 새로 그리는 두 버튼
    List<Button> buttons; // 생성한 모든 버튼 저장

    float[] weight = { 0.7f, 0.1f, 0.2f }; // 순서대로 몹 룸 / 보상 룸 / 이벤트 룸 등장 확률

    void Awake()
    {
        curButtonPos = firstButton.transform.localPosition;
        buttons = new List<Button>();
    }

    void OnEnable()
    {
        if (GameManager.inst.roomManager.roomCount < 4)
        { // 일반
            int x, y; // 랜덤하게 선택할 두 버튼의 인덱스
            // do{
            //     x = Random.Range(0,Enum.GetValues(typeof(RoomManager.RoomType)).Length-1); // 보스룸 제외하고 선택
            //     y = Random.Range(0,Enum.GetValues(typeof(RoomManager.RoomType)).Length-1);
            // }while(x == y);
            // x = 0;
            // y = Random.Range(0, Enum.GetValues(typeof(RoomManager.RoomType)).Length - 1);
            x = GetWeightedRandom();
            y = GetWeightedRandom();
            upButton = Instantiate(roomButton, transform);
            downButton = Instantiate(roomButton, transform);

            upButton.GetComponent<RectTransform>().localPosition = curButtonPos + new Vector2(100, 50);
            downButton.GetComponent<RectTransform>().localPosition = curButtonPos + new Vector2(100, -50);

            upButton.GetComponent<RoomButton>().SetType((RoomManager.RoomType)x);
            downButton.GetComponent<RoomButton>().SetType((RoomManager.RoomType)y);

            buttons.Add(upButton);
            buttons.Add(downButton);
        }
        else
        { // 보스 방
            upButton = Instantiate(roomButton, transform);
            upButton.GetComponent<RectTransform>().localPosition = curButtonPos + new Vector2(100, 0);
            upButton.GetComponent<RoomButton>().SetType(BossRoom);

            buttons.Add(upButton);
        }
    }

    // 누른 버튼대로 MoveRoom 실행하고 UI 비활성화
    public void OnButtonClick(RoomManager.RoomType type, Vector2 buttonPos)
    {
        curButtonPos = buttonPos; // 마지막으로 누른 버튼 위치 저장

        if (GameManager.inst.roomManager.roomCount < 4)
        {
            upButton.interactable = false; // 생성했던 버튼 상호작용 끄기
            downButton.interactable = false;
        }
        else
        { // 보스방 버튼 클릭시 맵 초기화
            ClearButton();
        }

        GameManager.inst.roomManager.MoveRoom(type); // 실제 방 이동
        GameManager.inst.screenUI.DisableMoveRoomUI(); // 검정 배경 없애기, 방 이동 UI 비활성화
    }

    void ClearButton()
    {
        curButtonPos = firstButton.transform.localPosition;
        foreach (Button i in buttons)
            Destroy(i.gameObject);
        buttons.RemoveAll(btn => { Destroy(btn.gameObject); return true; });
    }

    int GetWeightedRandom()
    {
        float total = 0f, randWeight, cumWeight = 0f;
        foreach (float w in weight)
            total += w;

        randWeight = Random.Range(0, total);

        for (int i = 0; i < weight.Length; i++)
        {
            cumWeight += weight[i];
            if (randWeight < cumWeight)
                return i;
        }
        return -1; // Error
    }
}
