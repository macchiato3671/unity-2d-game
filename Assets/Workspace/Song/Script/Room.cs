using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject cameraConfiner;
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private GameObject wind;
    [SerializeField] private GameObject portal;
    [SerializeField] private Transform enemySpawnPoint;

    public RoomManager.RoomType roomType;

    public RoomManager.RoomType GetRoomType()
    {
        return roomType;
    }


    public Collider2D GetCameraConfiner()
    {
        return cameraConfiner.GetComponent<Collider2D>();
    }

    public Vector3 GetSpawnPoint(){
        return spawnPoint.transform.position;
    }

    public void SetWind(){
        wind.GetComponent<Wind>().Init();
    }

    public GameObject GetPortal(){
        return portal;
    }

    public Vector3 GetPortalPos(){
        return portal.transform.position;
    }

    public Transform[] GetEnemySpawnPoints(){
        if(enemySpawnPoint != null) 
            return enemySpawnPoint.GetComponentsInChildren<Transform>().Where(t => t != enemySpawnPoint.transform).ToArray();
        else return null;
    }

    public void SetPortalState(bool isActive){
        portal.GetComponent<Portal>().SetUsable(isActive);
    }
}
