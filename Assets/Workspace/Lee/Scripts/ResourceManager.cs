using System.Reflection;
using UnityEngine;

public static class ResourceManager
{
    // private const string KEY_RESOURCE_AMOUNT = "HaveResourceAmount";
    public static int resourceAmount = 0;

    // �ʱⰪ�� ����� ���� ������ �ҷ����� ������ 20���� ����
    public static int have_resource_amount
    {
        // get => PlayerPrefs.GetInt(KEY_RESOURCE_AMOUNT, 20);
        // private set => PlayerPrefs.SetInt(KEY_RESOURCE_AMOUNT, value);
        get => resourceAmount;
        private set => resourceAmount = value;
    }

    public static void CollectResource(int amount)
    {
        have_resource_amount += amount;
        // PlayerPrefs.SetInt(KEY_RESOURCE_AMOUNT, have_resource_amount);
        // PlayerPrefs.Save();
    }

    public static bool UseResource(int amount)
    {
        if (have_resource_amount >= amount)
        {
            have_resource_amount -= amount;
            // PlayerPrefs.SetInt(KEY_RESOURCE_AMOUNT, have_resource_amount);
            // PlayerPrefs.Save();
            return true;
        }
        else return false;
    }

    public static void LoadResource(int value)
    {
        have_resource_amount = value;
    }
}