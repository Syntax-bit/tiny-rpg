using System;
using TinyRPG.Gameplay;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 15f;

    private Unit casterUnit;
    private Unit targetUnit;
    private AbilityData abilityData;
    private Action onImpactCallback;

    public void Initialize(Unit caster, Unit target, AbilityData ability)
    {
        casterUnit = caster;
        targetUnit = target;
        abilityData = ability;
    }

    public void SetCallBack(Action callback)
    {
        onImpactCallback = callback;
    }

    private void Update()
    {
        if (targetUnit == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 targetPosition = targetUnit.transform.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        transform.LookAt(targetPosition);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Unit>(out Unit hitUnit))
        {
            if (hitUnit == targetUnit)
            {
                //onImpactCallback?.Invoke();
                abilityData.Execute(casterUnit, targetUnit, targetUnit.transform.position);

                Destroy(gameObject);
            }
        }
    }
}
