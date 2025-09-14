using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{

    // 플레이어의 각 기능을 모듈화하여 분리하고, Player 스크립트에서 총괄하여 관리
    public enum PlayerState
    {
        Default,
        Attack,
        Climbing,
        Dead
    };

    PlayerState state;
    public PlayerState State
    {
        get { return state; }
        set { state = value; }
    }

    public PlayerHealth Health => health;
    public GameOverUI gameOverUI;

    [SerializeField] private PlayerMove move;
    [SerializeField] private PlayerAttack attack;
    [SerializeField] private PlayerConfiner confiner;
    [SerializeField] private PlayerHealth health;

    void Awake()
    {
        state = PlayerState.Default;
        if (GameManager.inst != null) GameManager.inst.player = this;


    }

    // void OnEnable()
    // {
    //     if(gameOverUI) health.OnPlayerDie += gameOverUI.ShowGameOver;
    // }

    // void OnDisable()
    // {
    //     if(gameOverUI) health.OnPlayerDie -= gameOverUI.ShowGameOver;
    // }

    public void SetConfiner(int maxX)
    {
        confiner.SetMax(maxX);
    }

    public void DealPlayer(int amount)
    {
        health.TakeDamage(amount);
    }

    public void HealPlayer(int amount)
    {
        health.Heal(amount);
    }

    public int GetPlayerWeaponIdx()
    {
        return attack.currentWeaponIndex;
    }

    public void AddPlayerSpeedFactor(float fac)
    {
        move.AddSpeedFactor(fac);
    }

    public void UpdateEquippedWeapon()
    {
        attack.UpdateEquippedWeapon();
    }

    public void UpdateEquipOn()
    {
        int hplevel = GameManager.inst.equipmentManager.equipDatas[0].currentLevel;
        int addedHP = (hplevel - 1);

        health.maxHP = 7 + addedHP;
        health.currentHP = health.maxHP;

        int movelevel = GameManager.inst.equipmentManager.equipDatas[2].currentLevel;
        move.move_speed = move.basemovespeed + (movelevel - 1) * 2f;

        int jumplevel = GameManager.inst.equipmentManager.equipDatas[1].currentLevel;
        move.jump_power = move.basejumppower + (jumplevel - 1) * 1.5f;
    }
}
