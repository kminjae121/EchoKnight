using System;
using System.Collections.Generic;
using System.Linq;
using Code.UnitSystem.SkillSystem;
using UnityEngine;

namespace _Code.KMJ.UnitSystem.involveUnitSO
{
    [CreateAssetMenu(fileName = "UnitSO", menuName = "UnitSO/UnitSKillStorage")]
    public class UnitSkillStorageSO : ScriptableObject
    {
        public UnitType uniType = UnitType.None;
        public List<SkillSO> skills = null;
    }
}