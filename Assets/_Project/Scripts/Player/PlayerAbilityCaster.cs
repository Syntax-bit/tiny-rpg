using ImprovedTimers;
using System;
using TinyRPG.Gameplay;
using TinyRPG.Player;
using TinyRPG.UI;
using UnityEngine;

[RequireComponent(typeof(PlayerTargeter))]
public class PlayerAbilityCaster : MonoBehaviour, ICastable
{
    [SerializeField] private Transform abilityHolder;
    private IHotbarAction[] hotbarActions;
    [SerializeField] private AbilityData[] startingAbilities;

    private CountdownTimer castTimer;
    private Unit playerUnit;
    private PlayerTargeter playerTargeter;
    private Vector3 currentCastWorldPosition;

    private AbilityData currentCastingAbility;
    private Unit currentCastingTarget;
    private PlayerInputHandler playerInputHandler;
    private PlayerController playerController;
    private CooldownTracker cooldownTracker;

    public string CastActionName => currentCastingAbility != null ? currentCastingAbility.label : "";
    public float CastProgress => castTimer != null ? castTimer.Progress : 0f;
    public bool IsCurrentlyCasting => castTimer != null && castTimer.IsRunning;
    public bool InvertFill => false;

    private void Awake()
    {
        playerUnit = GetComponent<Unit>();
        playerTargeter = GetComponent<PlayerTargeter>();
        playerInputHandler = GetComponent<PlayerInputHandler>();
        playerController = GetComponent<PlayerController>();
        cooldownTracker = GetComponent<CooldownTracker>();

        castTimer = new CountdownTimer(0);
        castTimer.OnTimerStop = () => ExecuteAbility(currentCastingAbility, currentCastingTarget, currentCastWorldPosition);

        hotbarActions = new IHotbarAction[abilityHolder.childCount];

        for (int i = 0; i < hotbarActions.Length; i++)
        {
            if (startingAbilities != null && i < startingAbilities.Length)
            {
                hotbarActions[i] = startingAbilities[i];
            }

            if (abilityHolder.GetChild(i).TryGetComponent<HotbarSlotUI>(out HotbarSlotUI hotSlot))
            {
                hotSlot.Initialize(i, hotbarActions[i], cooldownTracker);
            }
        }
    }

    private void Update()
    {
        if (IsCurrentlyCasting && currentCastingAbility != null)
        {
            if (!currentCastingAbility.canCastWhileMoving && playerController.IsMoving)
            {
                Debug.LogWarning("Cast Interrupted by Movement!");
                CleanUpCastState();
            }
        }
    }

    private void OnEnable()
    {
        if (playerInputHandler != null)
        {
            playerInputHandler.OnHotbarPressed += HandleHotbarInputIndex;
        }
    }

    private void OnDisable()
    {
        if (playerInputHandler != null)
        {
            playerInputHandler.OnHotbarPressed -= HandleHotbarInputIndex;
        }
    }

    private void HandleHotbarInputIndex(int index)
    {
        if (index < 0 || index >= hotbarActions.Length || IsCurrentlyCasting) return;

        IHotbarAction actionToUse = hotbarActions[index];
        if (actionToUse != null)
        {
            if (cooldownTracker.IsOnCooldown(actionToUse))
            {
                Debug.LogWarning($"{actionToUse} is on cooldown!");
                return;
            }

            actionToUse.Use(gameObject);
        }
    }

    public void TryCast(AbilityData abilityData)
    {
        if (abilityData == null || IsCurrentlyCasting) return;

        AbilityData chosenAbility = abilityData;
        if (chosenAbility == null) return;

        chosenAbility.Target(playerTargeter, (resolvedTarget) =>
        {
            if (resolvedTarget != null && CanCastAbility(chosenAbility, resolvedTarget))
            {
                Vector3 targetWorldPos = resolvedTarget.transform.position;
                if (chosenAbility.targetingStrategy is AOETargetingStrategy aoe)
                {
                    targetWorldPos = aoe.GetLastConfirmedPosition();
                }

                StartCast(chosenAbility, resolvedTarget, targetWorldPos);
            }
        });
    }

    private void StartCast(AbilityData abilityData, Unit target, Vector3 worldPos)
    {
        currentCastingAbility = abilityData;
        currentCastingTarget = target;
        currentCastWorldPosition = worldPos;

        castTimer.Stop();
        castTimer.Reset(abilityData.castTime);
        castTimer.Start();

        PlayerUIManager.Instance.ShowCastBar(this);
    }

    private void ExecuteAbility(AbilityData ability, Unit target, Vector3 targetWorldPosition)
    {
        if (ability == null || target == null)
        {
            CleanUpCastState();
            return;
        }

        cooldownTracker.StartCooldown(ability, ability.Cooldown, ability.triggersGlobalCooldown);

        if (ability.targetingStrategy is ProjectileTargeting)
        {
            ProjectileTargeting projectileTargeting = ability.targetingStrategy as ProjectileTargeting;

            Projectile projectileInstance = Instantiate(projectileTargeting.projectilePrefab, transform.position, transform.rotation).GetComponent<Projectile>();

            if (projectileInstance != null)
            {
                projectileInstance.Initialize(playerUnit, target, ability);

                projectileInstance.SetCallBack(() =>
                {
                    ability.Execute(playerUnit, target, targetWorldPosition);
                });
            }
        }
        else
        {
            ability.Execute(playerUnit, target, targetWorldPosition);
        }

        CleanUpCastState();
    }

    private void CleanUpCastState()
    {
        currentCastingAbility = null;
        currentCastingTarget = null;
        PlayerUIManager.Instance.HideCastBar();

        castTimer.Stop();
    }

    private bool CanCastAbility(AbilityData abilityData, Unit target)
    {
        if (target == playerUnit) return true;

        // Distance check
        float distanceToTarget = Vector3.Distance(playerUnit.transform.position, target.transform.position);
        if (distanceToTarget > abilityData.range)
        {
            Debug.LogWarning($"{abilityData.label} Out of Range! Required: {abilityData.range}, Current: {distanceToTarget}");
            return false;
        }

        // Looking direction check - Ensure player is facing the target within a reasonable angle
        Vector3 playerPos = new Vector3(playerUnit.transform.position.x, 0, playerUnit.transform.position.z);
        Vector3 targetPos = new Vector3(target.transform.position.x, 0, target.transform.position.z);

        Vector3 directionToTarget = (targetPos - playerPos).normalized;
        float lookDot = Vector3.Dot(playerUnit.transform.forward, directionToTarget);

        if (lookDot < 0.5f)
        {
            Debug.LogWarning("You must be facing your target!");
            return false;
        }

        return true;
    }
}