using UnityEngine;

namespace Code.UnitSystem
{
    [CreateAssetMenu(fileName = "UnitIngGameSO", menuName = "UnitIngGameSO", order = 0)]
    public class UnitInGameSO : ScriptableObject
    {
        public UnitType UnitType = UnitType.None;
        
        public float Maxhealth { get; set; }
        public float AtkDamage { get; set; }
        
        public float SkillDamage { get; set; }

        public float DefensivePower { get; set; }
    }
}