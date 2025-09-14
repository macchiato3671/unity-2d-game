using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TestText : MonoBehaviour
{
    Text text;

    void Awake()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) text.text = "변경";
    }
}
