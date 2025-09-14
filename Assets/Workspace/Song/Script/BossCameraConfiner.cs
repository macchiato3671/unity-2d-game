using UnityEngine;

public class BossCameraConfiner : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("Player"))
        {
            GameManager.inst.roomManager.SetNewConfiner(GetComponent<Collider2D>());
        }   
    }
}
