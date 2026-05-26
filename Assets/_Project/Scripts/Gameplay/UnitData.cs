using UnityEngine;

namespace TinyRPG.Gameplay
{
    [CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
    public class UnitData : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string unitName;
        [Range(1, 10)]
        [SerializeField] private int baseLevel = 1;

        [Header("Base Attributes")]
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private float baseMoveSpeed = 8f;
        [SerializeField] private int baseAttackDamage = 12;


        public string UnitName => unitName;
        public int MaxHealth => maxHealth;
        public float BaseMoveSpeed => baseMoveSpeed;
        public int AttackDamage => baseAttackDamage;

    }
}