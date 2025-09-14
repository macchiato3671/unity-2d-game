using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class EquipmentManager : MonoBehaviour
{
    public EquipmentData[] equipDatas;  // ��� ������ �迭

    void Awake()
    {
        if (GameManager.inst != null) GameManager.inst.equipmentManager = this;
        gameObject.SetActive(false);
        UpdateSkillCooldown();
    }

    public void Init_equip()
    {
        foreach (var i in equipDatas)
        {
            i.currentLevel = 1;
            i.requiredResource = 10;
        }
    }

    // ������ ������ ó��
    public void Upgrade_equip(int equipID)
    {
        if (equipDatas[equipID].currentLevel >= 5)
        {
            Debug.Log($"Item {equipDatas[equipID].equipName} is already at max level.");
            return;
        }

        if (ResourceManager.UseResource(equipDatas[equipID].requiredResource))
        {
            equipDatas[equipID].currentLevel += 1;  // ������ ������
            equipDatas[equipID].requiredResource += 10;  // ���� �������� �ʿ��� �ڿ� ����
            Debug.Log($"Item {equipDatas[equipID].equipName} upgraded to level {equipDatas[equipID].currentLevel}");


            if (equipID == 3)
            {
                UpdateSkillCooldown();
            }

            /*
            switch (equipID)
            {
                case 0:
                    UpdatePlayerMaxHP();
                    break;
                case 1:
                    UpdatePlayerMoveSpeed();
                    break;
                case 2:
                    UpdatePlayerJumpPower();
                    break;
                case 3:
                    UpdateSkillCooldown();
                    break;
            }*/
        }
        else
        {
            Debug.Log("Not enough resources to upgrade.");
        }
    }
    /*
    // maxHP ������Ʈ (equipID 0 -> equipDatas[0]�� �Ҵ�Ǿ� �־�� ��)
    private void UpdatePlayerMaxHP()
    {
        int level = equipDatas[0].currentLevel;
        int addedHP = (level - 1); 

    //    playerHealth.maxHP = 5 + addedHP;  // �⺻ 5 + �߰� HP
     //   playerHealth.Init(); // ���� HP�� �ִ� HP�� �ʱ�ȭ
        Debug.Log($"Player max HP updated: {level}");
    }

    private void UpdatePlayerMoveSpeed()
    {
        int level = equipDatas[1].currentLevel;
     //   playerMove.move_speed = playerMove.basemovespeed + (level - 1) * 3;
        Debug.Log($"Player move speed updated: {level}");
    }

    private void UpdatePlayerJumpPower()
    {
        int level = equipDatas[2].currentLevel;
     //   playerMove.jump_power = playerMove.basejumppower + (level - 1) * 3;
        Debug.Log($"Player jump power updated: {level}");
    }*/

    public void UpdateSkillCooldown()
    { // level 1 -> 0%, level�ö󰨿� ���� 10%�� ����
        int level = equipDatas[3].currentLevel;
        if (level == 0) return;

        float reductionMultiplier = 1f - ((level-1) * 0.1f);

        SkillData[] allSkills = Resources.LoadAll<SkillData>("SkillData");

        foreach (SkillData skill in allSkills)
        {
            if (skill.baseCooldown > 0f)
            {
                skill.ApplyCooldownModifier(reductionMultiplier);
                Debug.Log($"��ٿ� ����: {skill.skillName} �� {skill.cooldown:F2}s");
            }
        }
    }

    public void ClickSound()
    {
        SoundManager.inst.PlaySFX(Resources.Load<AudioClip>("Audio/Inventory_Equip"), 1);
    }
}