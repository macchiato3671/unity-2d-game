using System;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // PoolManager를 통한 에너미 소환 및 초기화 담당
    // 룸의 에너미 소환 위치 정보를 가져와서 해당 위치에 에너미 소환

    // Scriptable Object로 구성된 에너미 데이터
    [SerializeField] private EnemyData[] enemyData;
    PoolManager pool;

    int _curCount = 0;
    public int curCount{
        get{
            return _curCount;
        }
        set{
            _curCount = value;
            if(_curCount == 0 && isInitialized){
                GameManager.inst.roomManager.SetRoomClear();
            }
        }
    }

    bool isInitialized = false;

    void Awake()
    {
        if(GameManager.inst != null) GameManager.inst.enemyManager = this;
    }

    void OnEnable(){
        Enemy.OnEnemyDied += DecreaseCount;
    }

    void OnDisable(){
        Enemy.OnEnemyDied += DecreaseCount;
    }

    public void Spawn(Transform[] spawnPoints)
    {
        if (pool == null) pool = GameManager.inst.pool;
        if (spawnPoints == null) return;

        pool.DisableEnemy(); // 이전 에너미 전부 디스폰
        isInitialized = false;
        curCount = 0;

        foreach (Transform i in spawnPoints)
        {
            int[] idArr = i.GetComponent<EnemySpawnPoint>().idToSpawn;
            int randomIdx = idArr[UnityEngine.Random.Range(0, idArr.Length)];

            SpawnAuto(randomIdx, i.position);
        }
        isInitialized = true;
    }

    public GameObject SpawnAuto(int enemyIdx, Vector3 position)
    {
        if (pool == null) pool = GameManager.inst.pool;
        curCount++;

        EnemyData data = enemyData[enemyIdx];

        GameObject newObject;
        if (data.enemyID >= 6)
        {
            newObject = pool.GetBoss(0);
        }
        else
        {
            newObject = pool.GetEnemy(0);
        }

        newObject.transform.position = position;
        newObject.GetComponent<Enemy>().Init(data);

        return newObject;
    }



    public void Despawn()
    {
        if (pool == null) pool = GameManager.inst.pool;
        pool.DisableEnemy();
        pool.DisableBoss();
        curCount = 0;
    }

    void DecreaseCount(int none){
        curCount -= 1;
    }
}
