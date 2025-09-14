using System.Collections;
using UnityEngine;

public class AccessoryDrop : MonoBehaviour
{
    private AccessoryData accessory;

    void Start()
    {
        StartCoroutine(Destruction());   
    }

    public void Init(AccessoryData data)
    {
        accessory = data;
        GetComponent<SpriteRenderer>().sprite = data.icon;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && accessory != null && !accessory.isget)
        {
            accessory.isget = true;
            Debug.Log($"[��� ȹ��] {accessory.accessoryName}");
            Destroy(gameObject);
        }
    }
    
    IEnumerator Destruction()
    {
        yield return new WaitForSeconds(5f);
        if(gameObject.activeSelf == true) Destroy(gameObject);
    }
}