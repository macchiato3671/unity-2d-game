using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLight : MonoBehaviour
{
    void Awake()
    {
        if(GameManager.inst != null) GameManager.inst.globalLight = this.GetComponent<Light2D>();   
    }
}
