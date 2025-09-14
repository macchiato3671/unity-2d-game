using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    IInteractable curInter;

    void Update(){
        if (curInter != null && Input.GetKeyDown(KeyCode.F))
        {
            if(!GameManager.inst.IsPaused || GameManager.inst.IsOnInventory) curInter.Interact();
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.TryGetComponent(out IInteractable inter)){
            curInter = inter;
        }
    }

    void OnTriggerExit2D(Collider2D other){
        if(other.TryGetComponent(out IInteractable inter) && curInter == inter){
            curInter.Cancel();
            curInter = null;
        }
    }
}
