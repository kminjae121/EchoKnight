using Code.UnitSystem.SkillSystem;
using UnityEngine;

namespace _Code.KMJ.UnitSystem.involveUnitSO
{
    [CreateAssetMenu(fileName = "UnitSO", menuName = "UnitSO/UnitSKillStorage")]
    public class UnitOwnSkillStorageSO : ScriptableObject
    {
        public UnitType uniType = UnitType.None;
        public SkillSO[] skills = null;
    }
}