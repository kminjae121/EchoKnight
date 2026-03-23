using Code.Core.Managers;
using Code.UnitSystem.SkillSystem;
using UnityEngine;

namespace Code.UI.SkillTreeUI
{
    public class SkillNode : MonoBehaviour, INode
    {
        [SerializeField] private UnitType unitType;

        [SerializeField] private SkillSO skillSO;
       
        public void UseNode()
        {
            SkillSendManager.Instance.AddSkillList(skillSO);
        }
    }
}