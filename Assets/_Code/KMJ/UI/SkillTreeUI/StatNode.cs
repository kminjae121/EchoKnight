using _Code.KMJ.UnitSystem;
using Code.Core.Managers;
using Code.Managers;
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
        
        
        [SerializeField] private int nodePrice;
        
        public void UseNode()
        {
            if (nodePrice > PlayerManager.Instance.Gold)
                return;
            
            PlayerManager.Instance.RemoveGold(nodePrice);
            InGameStatCompo.Instance.SetStat(upgradeStat, upgradeValue, unitType);
        }
    }
}