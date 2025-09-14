using System.Collections.Generic;
using UnityEngine;

public class LocalMinimapController : MonoBehaviour
{
    public RectTransform mapArea;
    public GameObject playerIconPrefab;
    public GameObject enemyIconPrefab;
    public GameObject hackableIconPrefab;
    public GameObject exitIconPrefab;
    public Room2 currentRoom;

    void Start()
    {
        SpawnIcon(currentRoom.player, playerIconPrefab);

        foreach (var enemy in currentRoom.enemies)
            SpawnIcon(enemy, enemyIconPrefab);

        foreach (var hack in currentRoom.hackables)
            SpawnIcon(hack, hackableIconPrefab);

        if (currentRoom.exitPoint != null)
            SpawnIcon(currentRoom.exitPoint, exitIconPrefab);
    }

    void SpawnIcon(Transform target, GameObject prefab)
    {
        GameObject icon = Instantiate(prefab, mapArea);
        var script = icon.AddComponent<MinimapIcon>();
        script.target = target;
        script.roomBounds = currentRoom.bounds;
        script.minimapArea = mapArea;
    }
}
