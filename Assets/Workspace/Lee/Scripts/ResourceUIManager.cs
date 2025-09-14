using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceUIManager : MonoBehaviour
{
    public Text resourceText;

    private void FixedUpdate()
    {
        resourceText.text = ResourceManager.have_resource_amount.ToString();
    }

    public void updateResourceText()
    {
        resourceText.text = ResourceManager.have_resource_amount.ToString();
    }
}

