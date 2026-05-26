using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TinyRPG.Gameplay
{
    public class Unit : MonoBehaviour
    {
        public UnitData UnitData => unitData;

        public event Action OnHealthChanged;

        [SerializeField] private UnitData unitData;

        private int currentHealth;


        private void Awake()
        {
            currentHealth = unitData.MaxHealth;
        }

        public float GetNormalizedCurrentHealth()
        {
            return (float)currentHealth / (float)unitData.MaxHealth;
        }

        public void TakeDamage(int amount)
        {
            currentHealth = Mathf.Max(0, currentHealth - amount);
            OnHealthChanged?.Invoke();

            if (currentHealth <= 0)
            {
                Debug.Log(unitData.UnitName + " died!");
            }
        }
    }
}