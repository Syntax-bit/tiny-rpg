using UnityEngine;

public interface IInteractable
{
    public void Interact();

    public float GetInteractionTime();

    public string GetInteractionPrompt();
}
