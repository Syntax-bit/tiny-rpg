using UnityEngine;
using ImprovedTimers;
using System;

namespace TinyRPG.Gameplay
{
    public class ActiveDoTBehavior : MonoBehaviour
    {
        public Action<ActiveDoTBehavior> OnEffectExpired;
        public bool isStackable {  get; private set; }
        public float duration { get; private set; }

        public int currentStacks { get; private set; }
        private int maxStackSize;
        private Unit targetUnit;
        private int damagePerTick;
        private IntervalTimer timer;

        private GameObject spawnedVfxInstance;

        public void Initialize(float duration, float interval, int damage, Unit target, GameObject vfxPrefab, bool stackable, int maxStacks)
        {
            this.duration = duration;
            targetUnit = target;
            damagePerTick = damage;
            isStackable = stackable;
            maxStackSize = maxStacks;
            currentStacks = 1;

            timer = new IntervalTimer(duration, interval);
            timer.OnInterval = HandleTick;
            timer.OnTimerStop = HandleExpiry;

            timer.Start();

            if (vfxPrefab != null)
            {
                spawnedVfxInstance = Instantiate(vfxPrefab, target.transform.position, Quaternion.identity, target.transform);
                spawnedVfxInstance.transform.localPosition = Vector3.zero;
            }
        }

        public void IncrementStack()
        {
            if (!isStackable) return;

            if(currentStacks < maxStackSize)
            {
                currentStacks++;
                Refresh();
            }
        }

        public void Refresh()
        {
            timer.Reset();
        }

        public void CancelEarly()
        {
            if (timer != null) timer.Stop();
            HandleExpiry();
        }

        private void HandleTick()
        {
            if (targetUnit != null)
            {
                targetUnit.TakeDamage(damagePerTick);
            }
        }

        private void HandleExpiry()
        {
            if (spawnedVfxInstance != null)
            {
                Destroy(spawnedVfxInstance);
            }

            if (timer != null)
            {
                timer.OnInterval = null;
                timer.OnTimerStop = null;
            }

            OnEffectExpired?.Invoke(this);
            Destroy(this);
        }

    }
}