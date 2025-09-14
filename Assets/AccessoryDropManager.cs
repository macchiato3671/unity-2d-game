using System.Collections.Generic;
using UnityEngine;

public class AccessoryDropManager : MonoBehaviour
{
    public GameObject accessoryDropPrefab; // ����߸� ������
    public Transform dropParent; // ��� ��ġ �θ� (������ null ����)

    float dropChance = 0.1f; // ��� Ȯ�� (20%)

    private void Awake()
    {
        if (GameManager.inst != null) GameManager.inst.accessoryDropManager = this;
    }

    public void TrySpawnAccessoryDrop(Vector3 position)
    {
        if (Random.value > dropChance)
            return;

        AccessoryData dropAcc = GetRandomUnownedAccessory();
        if (dropAcc == null) return;

        GameObject dropObj = Instantiate(accessoryDropPrefab, position-Vector3.down, Quaternion.identity, dropParent);
        AccessoryDrop dropScript = dropObj.GetComponent<AccessoryDrop>();
        dropScript.Init(dropAcc);
    }

    AccessoryData GetRandomUnownedAccessory()
    {
        List<AccessoryData> pool = new List<AccessoryData>();
        string[] folders = { "GravityAccessory", "LaserAccessory", "MeleeAccessory", "ThrowingAccessory" };

        foreach (string folder in folders)
        {
            var accs = Resources.LoadAll<AccessoryData>("AccessoryData/" + folder);
            foreach (var a in accs)
            {
                if (!a.isget)
                    pool.Add(a);
            }
        }

        if (pool.Count == 0) return null;

        return pool[Random.Range(0, pool.Count)];
    }
}