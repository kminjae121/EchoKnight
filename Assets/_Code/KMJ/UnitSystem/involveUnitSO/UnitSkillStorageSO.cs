using Code.UnitSystem.SkillSystem;
using UnityEngine;

namespace _Code.KMJ.UnitSystem.involveUnitSO
{
    [CreateAssetMenu(fileName = "UnitSO", menuName = "UnitSO/UnitSKillStorage")]
    public class UnitSkillStorageSO : ScriptableObject
    {
        public SkillSO[] skills { get; } = null;
    }
}