using UnityEngine;

public interface IHotbarAction
{
    public string Label { get; }
    public Sprite Icon { get; }
    public float Cooldown { get; }

    public void Use(GameObject user);
}
