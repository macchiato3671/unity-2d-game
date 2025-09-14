using UnityEngine;
using UnityEngine.UI;

public class DeathConditionUI : MonoBehaviour
{
    [SerializeField] private Text text;

    public void SetState(string txt){
        text.text = txt;
    }
}
