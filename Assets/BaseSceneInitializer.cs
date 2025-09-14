using UnityEngine;

public class BaseSceneInitializer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SoundManager.inst?.PlayBaseBGM();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
