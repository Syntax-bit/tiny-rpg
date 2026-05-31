using System;
using TinyRPG.Gameplay;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    public event Action OnResourceChanged;

    // Later make this based of the unit data and have it be more dynamic
    [SerializeField] private int regenerationAmount = 10;
    [SerializeField] private float regenerationInterval = 1f;

    [SerializeField] private int maxResource = 100;
    [SerializeField] private ResourceType resourceType;

    private int currentResource;
    private float regenerationTimer;

    private UnitData unitData;

    private void Awake()
    {
        currentResource = maxResource;
    }

    private void Update()
    {
        RegenerateResource();
    }

    public ResourceType GetResourceType()
    {
        return resourceType;
    }

    public float GetNormalizedCurrentResource()
    {
        return (float)currentResource / (float)maxResource;
    }

    public bool CanSpendResource(int amount)
    {
        return currentResource >= amount;
    }

    public void SpendResource(int amount)
    {
        currentResource -= amount;
        OnResourceChanged?.Invoke();
    }

    private void RegenerateResource()
    {
        regenerationTimer += Time.deltaTime;
        if(regenerationTimer >= regenerationInterval)
        {
            currentResource = Mathf.Min(currentResource + regenerationAmount, maxResource);
            regenerationTimer = 0f;
            OnResourceChanged?.Invoke();
        }
    }
}

public enum ResourceType
{
    None,
    Mana,
    Energy,
    Health
}