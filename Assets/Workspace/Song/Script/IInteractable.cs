using UnityEngine;

public interface IInteractable
{
    public bool isActive { get; set; }

    void Interact();
    void Cancel();
}
