using System.Collections;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Resource : MonoBehaviour
{
    public int resource_num;
    private bool isCollected = false; // �ڿ� ���� ���� üũ

    [SerializeField] private AudioClip getResourceSound;

    void OnEnable()
    {
        StartCoroutine(Destruction());
    }

    public void SetAmount(int amount)
    {
        resource_num = amount;
    }

    // �ڿ� �������� �÷��̾�� �浹 �� �ڿ��� �����ϵ��� ��
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isCollected)
        {
            isCollected = true;
            CollectResource(resource_num);  // �ڿ� ����
            HideResource();       // �ڿ� �����
        }
    }
    // �ڿ� ���� �Լ�
    private void CollectResource(int amount)
    {
        if (getResourceSound) SoundManager.inst.PlaySFX(getResourceSound);
        ResourceManager.CollectResource(amount);  // ItemManager���� �ڿ� ����

        Debug.Log($"Collected {amount} of resource");
    }

    // �ڿ� ����� (�ڿ� �̹����� ������ ��Ҹ� ����� ���)
    private void HideResource()
    {
        // Renderer renderer = GetComponent<Renderer>();
        // if (renderer != null)
        // {
        //     renderer.enabled = false;  // �ڿ� �����
        // }

        // Collider2D col = GetComponent<Collider2D>();
        // if (col != null)
        // {
        //     col.enabled = false;
        // }
        gameObject.SetActive(false);
    }

    IEnumerator Destruction()
    {
        yield return new WaitForSeconds(5f);
        if(gameObject.activeSelf == true) HideResource();
    }
}