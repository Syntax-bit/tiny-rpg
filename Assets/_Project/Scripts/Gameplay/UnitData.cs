using UnityEngine;

namespace TinyRPG.Gameplay
{
    [CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
    public class UnitData : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string unitName;
        [SerializeField] private Faction faction;
        [SerializeField] [Range(1, 10)] private int baseLevel = 1;

        [Header("Base Attributes")]
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int baseMana = 50;
        [SerializeField] private float baseMoveSpeed = 8f;
        [SerializeField] private int baseAttackDamage = 12;


        public string UnitName => unitName;
        public Faction UnitFaction => faction;
        public int MaxHealth => maxHealth;
        public float BaseMoveSpeed => baseMoveSpeed;
        public int AttackDamage => baseAttackDamage;
        public int BaseMana => baseMana;

    }
}