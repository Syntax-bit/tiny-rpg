using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    [SerializeField] private string label;
    [SerializeField] private Sprite icon;

    [SerializeField] private bool isStackable;
    [SerializeField] [Range(1, 64)] private int stackSize;
    [SerializeField] [Range(0, 10)] private float cooldown;

    [SerializeField] private ResourceType resourceType;
    [SerializeField] private int resourceCost;

    public string Label => label;
    public Sprite Icon => icon;
    public bool IsStackable => isStackable;
    public int StackSize => stackSize;
    public float Cooldown => cooldown;
    public ResourceType ResourceType => resourceType;
    public int ResourceCost => resourceCost;
}
