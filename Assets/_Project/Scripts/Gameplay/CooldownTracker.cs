using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TinyRPG.Gameplay
{
    public class CooldownTracker : MonoBehaviour
    {
        public event Action<IHotbarAction, float> OnCooldownStarted;
        public event Action<float> OnGlobalCooldownStarted;

        private Dictionary<IHotbarAction, float> abilityCooldowns = new Dictionary<IHotbarAction, float>();
        private float globalCooldownEndTime;

        public void StartCooldown(IHotbarAction ability, float cooldownDuration, bool triggerGlobalCooldown = true, float globalDuration = 1f)
        {
            if (cooldownDuration <= 0) return;
            abilityCooldowns[ability] = Time.time + cooldownDuration;

            if (triggerGlobalCooldown && globalDuration > 0f)
            {
                float targetGCDEndTime = Time.time + globalDuration;
                if (targetGCDEndTime > globalCooldownEndTime)
                {
                    globalCooldownEndTime = targetGCDEndTime;
                    OnGlobalCooldownStarted?.Invoke(globalDuration);
                }
            }

            OnCooldownStarted?.Invoke(ability, cooldownDuration);
        }

        public bool IsOnCooldown(IHotbarAction ability)
        {
            if(Time.time < globalCooldownEndTime) return true;

            if (abilityCooldowns.TryGetValue(ability, out float cooldownEndTime))
            {
                return Time.time < cooldownEndTime;
            }
            return false;
        }

        public float GetCooldownProgress(IHotbarAction ability)
        {
            float individualRemaining = 0f;
            if (abilityCooldowns.TryGetValue(ability, out float endTime))
            {
                individualRemaining = Mathf.Max(0f, endTime - Time.time);
            }

            float gcdRemaining = Mathf.Max(0f, globalCooldownEndTime - Time.time);

            // Return whichever restriction is longer right now
            return Mathf.Max(individualRemaining, gcdRemaining);
        }
    }
}
