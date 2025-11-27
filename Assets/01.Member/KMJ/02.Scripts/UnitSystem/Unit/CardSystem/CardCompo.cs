using System;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnitSystem;
using UnityEngine;

namespace CardSystem
{
    public class CardCompo : MonoBehaviour, IUnitComponent
    {
        [SerializeField] private List<UnitSkillCardSO> skillList;
     
        public Dictionary<string, BaseCard> skillDict;

        private BasicUnit _owner;

        public void Initialize(Unit owner)
        {
            _owner = owner as BasicUnit;
        }
        
        private void Start()
        {
            skillDict = new Dictionary<string, BaseCard>();

            if(skillDict == null)
                return;
            else
            {
                foreach (var skillSo in skillList)
                {
                    var type = Type.GetType(skillSo.skillCardName);

                    if (type == null)
                        return;

                    var components = gameObject.GetComponentsInChildren(type, true);

                    if (components.Length > 0)
                    {
                        BaseCard component = components[0] as BaseCard;
                        
                        skillDict.Add(skillSo.skillCardName, component);
                    }
                }
            }
           

            if (skillDict == null)
                return;
            else
                skillDict.Values.ToList().ForEach(skill => skill.GetCard());    
        }
    

        public void AddSkill(UnitSkillCardSO cardSO)
        {
            if (cardSO == null) return;

            if (_owner.GetCost(cardSO.cost))
            {
                skillList.Add(cardSO);

                var type = Type.GetType(cardSO.skillCardName);

                var components = gameObject.GetComponentsInChildren(type, true);

                if (components.Length > 0)
                {
                    BaseCard component = components[0] as BaseCard;
             
                    skillDict.Add(cardSO.name, component);
                    skillDict.GetValueOrDefault(cardSO.skillCardName).GetCard();
                }
            }
        }

        public void RemoveSkill(UnitSkillCardSO cardSO)
        {
            if (cardSO == null) return;

            _owner.RemoveCost(cardSO.cost);

            skillList.Remove(cardSO);
                
            skillDict.Remove(cardSO.name);
        }

        public void DefaltSkill()
        { 
            if (skillDict == null)
                return;

            skillDict.Values.ToList().ForEach(skill => skill.EventDefault());
        }

    }
}