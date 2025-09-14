using UnityEngine;
using UnityEngine.UI;
using static RoomManager.RoomType;

public class RoomButton : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private GameObject buttonLine1;
    [SerializeField] private GameObject buttonLine2;

    [SerializeField] private Sprite[] icons;
    Button button;
    
    MoveRoomUI moveRoomUI;

    RoomManager.RoomType type;

    void Awake(){
        button = GetComponent<Button>();
        moveRoomUI = transform.parent.GetComponent<MoveRoomUI>();
    }

    public void SetType(RoomManager.RoomType inType){
        type = inType;
        switch(type){
            case MobRoom: image.sprite = icons[0]; break;
            case RewardRoom: image.sprite = icons[1]; break;
            case EventRoom: image.sprite = icons[2]; break;
            case BossRoom: image.sprite = icons[3]; break;
        }
    }

    public void OnButtonClick(){ // 에디터에서 설정
        if(GameManager.inst.roomManager.roomCount < 4-1) buttonLine1.SetActive(true);
        else buttonLine2.SetActive(true);

        moveRoomUI.OnButtonClick(type,transform.localPosition);
    }
}
