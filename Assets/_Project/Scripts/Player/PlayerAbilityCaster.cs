using ImprovedTimers;
using System;
using TinyRPG.Gameplay;
using TinyRPG.Player;
using TinyRPG.UI;
using UnityEngine;
using static AbilityData;

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
    private ResourceController resourceController;

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
        resourceController = GetComponent<ResourceController>();

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

            if(!resourceController.CanSpendResource(actionToUse.ResourceCost))
            {
                Debug.LogWarning($"Not enough resource to use {actionToUse}!");
                return;
            }

            actionToUse.Use(gameObject);
        }
    }

    public void TryCast(AbilityData abilityData)
    {
        if (abilityData == null || IsCurrentlyCasting) return;

        AbilityData chosenAbility = abilityData;

        bool isFriendlySpell = chosenAbility.targetFilter == TargetFilter.Self ||
                           chosenAbility.targetFilter == TargetFilter.Allies;

        if (isFriendlySpell)
        {
            bool targetingEnemy = playerTargeter.selectedUnit != null &&
                playerTargeter.selectedUnit.UnitData.UnitFaction != playerUnit.UnitData.UnitFaction;

            if (playerTargeter.selectedUnit == null || playerTargeter.selectedUnit == playerUnit || targetingEnemy)
            {

                ExecuteSelfCast(chosenAbility);
                return;
            }
        }

        chosenAbility.Target(playerTargeter, (resolvedTarget) =>
        {
            // Secondary fallback just in case a strategy resolves to null internally
            if (resolvedTarget == null && isFriendlySpell)
            {
                resolvedTarget = playerUnit;
            }

            ProcessCastFinalization(chosenAbility, resolvedTarget);
        });
    }

    private void ExecuteSelfCast(AbilityData abilityData)
    {
        if (CanCastAbility(abilityData, playerUnit))
        {
            Vector3 targetWorldPos = transform.position;

            if (abilityData.targetingStrategy is AOETargetingStrategy aoe)
            {
                targetWorldPos = aoe.GetLastConfirmedPosition();
            }

            StartCast(abilityData, playerUnit, targetWorldPos);
        }
    }

    private void ProcessCastFinalization(AbilityData abilityData, Unit resolvedTarget)
    {
        if (CanCastAbility(abilityData, resolvedTarget))
        {
            Vector3 targetWorldPos = transform.position;

            if (abilityData.targetingStrategy is AOETargetingStrategy aoe)
            {
                targetWorldPos = aoe.GetLastConfirmedPosition();
            }
            else if (resolvedTarget != null)
            {
                targetWorldPos = resolvedTarget.transform.position;
            }

            StartCast(abilityData, resolvedTarget, targetWorldPos);
        }
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
        if (ability == null)
        {
            CleanUpCastState();
            return;
        }

        cooldownTracker.StartCooldown(ability, ability.Cooldown, ability.triggersGlobalCooldown, .5f);
        resourceController.SpendResource(ability.ResourceCost);

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
        if (abilityData.targetingStrategy is AOETargetingStrategy) return true;

        if (target == null)
        {
            if (abilityData.targetFilter == TargetFilter.Enemies)
            {
                Debug.LogWarning("You must select an enemy target first!");
                return false;
            }

            Debug.LogWarning("Invalid Target!");
            return false;
        }

        if (!IsTargetInValidPosition(abilityData, target)) return false;

        bool isAlly = playerUnit.UnitData.UnitFaction == target.UnitData.UnitFaction;

        switch (abilityData.targetFilter)
        {
            //case TargetFilter.Self:
            //    if (target != playerUnit)
            //    {
            //        Debug.LogWarning("This spell can only be cast on yourself!");
            //        return false;
            //    }
            //    break;

            case TargetFilter.Allies:
                if (!isAlly)
                {
                    Debug.LogWarning("You can only cast this on friendly allies!");
                    return false;
                }
                break;

            case TargetFilter.Enemies:
                if (isAlly)
                {
                    Debug.LogWarning("You cannot cast harmful spells on your allies!");
                    return false;
                }
                break;
        }

        return true;
    }

    private bool IsTargetInValidPosition(AbilityData abilityData, Unit target)
    {
        if (target == null) return false;
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