using UnityEngine;

public class MainSceneInitializer : MonoBehaviour
{
    void Start()
    {
        Debug.Log("MainSceneInitializer");
        SoundManager.inst?.PlayAdventureBGM();
    }
}
