using System.Collections.Generic;
using UnityEngine;

public class Room2 : MonoBehaviour
{
    public Rect bounds; // 현재 구역의 월드 범위
    public Transform player; // 플레이어 위치 참조
    public List<Transform> enemies = new List<Transform>(); // 적들
    public List<Transform> hackables = new List<Transform>(); // 해킹 대상
    public Transform exitPoint; // 출구 위치
}
