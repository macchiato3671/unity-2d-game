using UnityEngine;
using UnityEngine.UI;
using static RoomManager.RoomType; // 룸타입 편하게 사용

public class ClearConditionUI : MonoBehaviour
{
    [SerializeField] private Sprite[] iconSprites;
    [SerializeField] private UnityEngine.UI.Image icon;
    [SerializeField] private Text text;

    public void SetState(int iconIdx, string txt){
        icon.sprite = iconSprites[iconIdx];
        text.text = txt;
    }
}
