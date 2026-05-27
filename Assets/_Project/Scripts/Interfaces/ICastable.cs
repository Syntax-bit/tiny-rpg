using UnityEngine;

public interface ICastable
{
    string CastActionName { get; }
    float CastProgress { get; }
    bool IsCurrentlyCasting { get; }

    bool InvertFill { get; }
}
