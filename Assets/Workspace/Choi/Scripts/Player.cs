using Unity.VisualScripting;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    // 플레이어의 각 기능을 모듈화하여 분리하고, Player 스크립트에서 총괄하여 관리

    public enum PlayerState{Default,Attack};
    PlayerState state;
    public PlayerState State{
        get{return state;}
        set{state = value;}
    }
    public PlayerHealth Health => health;
    public GameOverUI gameOverUI;

    [SerializeField] private PlayerMove2 move;
    [SerializeField] private PlayerAttack2 attack;
    [SerializeField] private PlayerHealth health; // 새로 추가
         // 외부 참조용 프로퍼티


    void Awake()
    {
        state = PlayerState.Default;
    }

    void Start()
    {
        health.OnPlayerDie += gameOverUI.ShowGameOver;
    }

}
