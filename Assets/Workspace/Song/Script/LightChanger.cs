using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightChanger : MonoBehaviour
{
    [SerializeField] private Light2D targetLight;

    float curTime = 0, changeTime = 1.25f;

    void FixedUpdate()
    {
        curTime += Time.fixedDeltaTime;
        if(curTime < changeTime)
            targetLight.intensity = Mathf.Lerp(1.5f,3f,curTime/changeTime);
        else if(curTime < changeTime*2)
            targetLight.intensity = Mathf.Lerp(3f,1.5f,curTime/changeTime-1);
        else curTime = 0;
    }
}
