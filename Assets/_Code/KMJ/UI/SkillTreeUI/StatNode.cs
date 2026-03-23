using _Code.KMJ.UnitSystem;
using Code.UnitManaging;
using Code.UnitSystem;
using UnityEngine;

namespace Code.UI.SkillTreeUI
{
    public class StatNode : MonoBehaviour, INode
    {
        [SerializeField] private UnitType unitType;
        [SerializeField] private StatInfo upgradeStat;
        [SerializeField] private float upgradeValue;
        
        public void UseNode()
        {
            InGameStatCompo.Instance.SetStat(upgradeStat, upgradeValue, unitType);
        }
    }
}