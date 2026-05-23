using UnityEngine;

public class LootableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionPrompt;
    [SerializeField] private float interactionTime = 0f; 

    public void Interact()
    {
        Debug.Log("interacted!");
    }

    public float GetInteractionTime()
    {
        return interactionTime;
    }

    public string GetInteractionPrompt()
    {
        return interactionPrompt;
    }
}
