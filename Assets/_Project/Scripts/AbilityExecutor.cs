using ImprovedTimers;
using TinyRPG.Gameplay;
using TinyRPG.Player;
using TinyRPG.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityExecutor : MonoBehaviour, ICastable
{
    [SerializeField] private AbilityData ability;

    private CountdownTimer castTimer;
    private Unit playerUnit;
    private PlayerTargeter playerTargeter;

    private Unit currentCastTarget;

    public string CastActionName => ability.label;

    public float CastProgress => castTimer.Progress;

    public bool IsCurrentlyCasting => castTimer.IsRunning;

    public bool InvertFill => false;

    private void Awake()
    {
        playerUnit = GetComponent<Unit>();
        playerTargeter = GetComponent<PlayerTargeter>();

        castTimer = new CountdownTimer(ability.castTime);

        castTimer.OnTimerStop = () => SpawnVFX();
    }

    private void SpawnVFX()
    {
        if (ability.vfxPrefab == null || currentCastTarget == null) return;

        var vfxInstance = Instantiate(ability.vfxPrefab, transform.position, transform.rotation);

        if (vfxInstance.TryGetComponent<Projectile>(out Projectile projectile))
        {
            projectile.Initialize(currentCastTarget);

            projectile.SetCallBack(() =>
            {
                if (currentCastTarget != null)
                {
                    foreach (var effect in ability.effects)
                    {
                        effect.Execute(playerUnit, currentCastTarget);
                    }
                }
            });
        }

        PlayerUIManager.Instance.HideCastBar();
    }

    public void Execute(Unit target)
    {
        if (castTimer.IsRunning) return;

        currentCastTarget = target;
        castTimer.Start();

        PlayerUIManager.Instance.ShowCastBar(this);
    }

    private void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            if (playerTargeter.selectedUnit == null) return;
            Execute(playerTargeter.selectedUnit);
        }
    }
}
