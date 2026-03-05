using UnityEngine;

namespace Code.UnitSystem
{
    [CreateAssetMenu(fileName = "UnitIngGameSO", menuName = "UnitIngGameSO", order = 0)]
    public class UnitInGameSO : ScriptableObject
    {
        public UnitType UnitType = UnitType.None;
        
        public float Maxhealth;
        public float AtkDamage;

        public float SkillDamage;

        public float DefensivePower;
    }
}