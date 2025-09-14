using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Wind : MonoBehaviour
{
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private Light2D globalLight;

    float maxTime = 14f;
    float duration = 4f;
    float curTime;

    Color yellow = new Color(0.5f,0.5f,0.3f);

    void Start(){
        curTime = maxTime;
    }

    void Update(){
        curTime += Time.deltaTime;

        if(curTime >= maxTime){
            StartCoroutine(ActivateWind());
            curTime = 0f;
        }

        transform.position = new Vector2(GameManager.inst.player.transform.position.x + 40,transform.position.y);
    }

    public void Init(){
        globalLight = GameManager.inst.globalLight;
    }

    IEnumerator ActivateWind(){
        float cur = 0, max = 0.75f;

        while(cur < max){
            cur += Time.deltaTime;
            yield return null;
            globalLight.color = Color.Lerp(Color.white,yellow,cur/max);
        }
        particle.Play();
        yield return new WaitForSeconds(duration);

        cur = 0;
        while(cur < max){
            cur += Time.deltaTime;
            yield return null;
            globalLight.color = Color.Lerp(yellow,Color.white,cur/max);
        }
        particle.Stop();
    }
}
