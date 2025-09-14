using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // 투사체 프리팹을 에디터에서 등록받고, 각 프리팹마다 오브젝트 관리
    [SerializeField] private GameObject[] rangePrefabs;
    public List<GameObject>[] rangePools;

    // 적 오브젝트 관리(일반 몬스터는 종류 구분 없이 하나의 프리팹으로 처리, 보스만 별도의 프리팹으로)
    [SerializeField] private GameObject[] enemyPrefabs;
    public List<GameObject>[] enemyPools;

    [SerializeField] private GameObject[] bossPrefabs;
    public List<GameObject>[] bossPools;

    // 동적 생성이 필요한 UI 오브젝트 관리
    [SerializeField] private GameObject[] uiPrefabs;
    public List<GameObject>[] uiPools;

    [SerializeField] private GameObject[] effectPrefabs;
    public List<GameObject>[] effectPools;

    void Awake(){
        if(GameManager.inst != null) GameManager.inst.pool = this;

        rangePools = new List<GameObject>[rangePrefabs.Length];
        enemyPools = new List<GameObject>[enemyPrefabs.Length];
        bossPools = new List<GameObject>[bossPrefabs.Length];
        uiPools = new List<GameObject>[uiPrefabs.Length];
        effectPools = new List<GameObject>[effectPrefabs.Length];

        for(int i = 0; i < rangePools.Length; i++)
            rangePools[i] = new List<GameObject>();

        for(int i = 0; i < enemyPools.Length; i++)
            enemyPools[i] = new List<GameObject>();

        for (int i = 0; i < bossPools.Length; i++)
            bossPools[i] = new List<GameObject>();

        for (int i = 0; i < uiPools.Length; i++)
            uiPools[i] = new List<GameObject>();

        for (int i = 0; i < effectPools.Length; i++)
            effectPools[i] = new List<GameObject>();
    }

    public GameObject GetRange(int idx){
        // 풀에 미사용하는(비활성화된) 오브젝트가 있으면 가져오고, 없다면 새로 생성하여 반환
        GameObject select = null;

        foreach(GameObject item in rangePools[idx])
            if(!item.activeSelf){
                select = item;
                select.SetActive(true);
                break;
            }

        if(!select){
            select = Instantiate(rangePrefabs[idx],transform);
            rangePools[idx].Add(select);
        }

        return select;
    }

    // poolmanager에서 활성화된 것 중 idx에 해당하는 개수 반환
    public int GetActiveCount(int idx)
    {
        int count = 0;
        foreach (GameObject obj in rangePools[idx])
        {
            if (obj.activeSelf)
                count++;
        }
        return count;
    }

    public void DisableRange(){
        for(int i = 0; i < rangePools.Length; i++)
            foreach(GameObject range in rangePools[i]){
                range.SetActive(false);
            }
    }

    // poolmanager에서 활성화 된 것 중 idx에 해당하는 것만 반환
    public List<GameObject> GetAllActiveInPool(int idx)
    {
        List<GameObject> activeObjects = new List<GameObject>();

        foreach (GameObject obj in rangePools[idx])
        {
            if (obj.activeSelf)
                activeObjects.Add(obj);
        }

        return activeObjects;
    }

    // 에너미 오브젝트 생성(재활용) : 일반 에너미의 인덱스는 0
    public GameObject GetEnemy(int idx){
        GameObject select = null;

        foreach(GameObject enemy in enemyPools[idx]){
            if(!enemy.activeSelf){
                select = enemy;
                select.SetActive(true);
                break;
           }
        }

        if(!select){
            select = Instantiate(enemyPrefabs[idx],transform);
            enemyPools[idx].Add(select);
        }

        return select;
    }

    public void DisableEnemy(){
        for(int i = 0; i < enemyPools.Length; i++)
            foreach(GameObject enemy in enemyPools[i]){
                enemy.SetActive(false);
            }
    }

    public GameObject GetBoss(int idx)
    {
        GameObject select = null;

        foreach (GameObject boss in bossPools[idx])
        {
            if (!boss.activeSelf)
            {
                select = boss;
                select.SetActive(true);
                break;
            }
        }

        if (select == null)
        {
            select = Instantiate(bossPrefabs[idx], transform);
            bossPools[idx].Add(select);
        }

        return select;
    }

    public void DisableBoss()
    {
        for (int i = 0; i < bossPools.Length; i++)
            foreach (GameObject boss in bossPools[i])
            {
                boss.SetActive(false);
            }
    }

    // UI 오브젝트 생성(재활용)
    public GameObject GetUI(int idx)
    {
        GameObject select = null;

        Debug.Log($"[DEBUG] GetUI({idx}) 호출됨. 프리팹 이름: {uiPrefabs[idx]?.name}");

        foreach (GameObject ui in uiPools[idx])
        {
            if (!ui.activeSelf)
            {
                select = ui;
                select.SetActive(true);
                break;
            }
        }

        if (!select)
        {
            if (uiPrefabs[idx] == null)
            {
                Debug.LogError($"[ERROR] uiPrefabs[{idx}] 가 비어있음!");
                return null;
            }
            select = Instantiate(uiPrefabs[idx], transform);
            select.SetActive(true);
            uiPools[idx].Add(select);
            Debug.Log($"[DEBUG] 새로 생성된 UI: {select.name}");
        }

        return select;
    }


    public GameObject GetEffect(int idx)
    {
        GameObject select = null;

        foreach (GameObject effect in effectPools[idx])
        {
            if (!effect.activeSelf)
            {
                select = effect;
                select.SetActive(true);
                break;
            }
        }

        if (select == null)
        {
            select = Instantiate(effectPrefabs[idx], transform);
            effectPools[idx].Add(select);
        }

        return select;
    }
}
