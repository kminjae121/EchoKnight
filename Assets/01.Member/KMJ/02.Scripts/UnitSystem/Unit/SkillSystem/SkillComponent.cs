using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.UnitSystem.SkillSystem
{
    public class SkillComponent : MonoBehaviour, IUnitComponent
    {
        [SerializeField] private List<SkillSO> _skillList;
        
        public int currentSkillCost = 10;
        
        private Dictionary<string, BaseSkill> skills = new Dictionary<string, BaseSkill>();

        private Unit _unit;
        
        public void Initialize(Unit owner)
        {
            _unit = owner;
            
            skills = new Dictionary<string, BaseSkill>();

            if(skills == null)
                return;
            else
            {
                foreach (var skillSo in _skillList)
                {
                    var type = Type.GetType(skillSo.className);

                    if (type == null)
                        return;

                    var components = _unit.GetComponentsInChildren(type, true);

                    if (components.Length > 0)
                    {
                        BaseSkill component = components[0] as BaseSkill;

                   

                        skills.Add(skillSo.skillName, component);
                    }
                }
            }
           

            if (skills == null)
                return;
            else
                skills.Values.ToList().ForEach(skill => skill.InitializeSkill());

            InitializeSkills();
        }
        
        public void InitializeSkills()
        {
        }
        
        public void StartSkill(string skillName)
        {
            if(!skills.ContainsKey(skillName))
                return;
            
            BaseSkill skill = skills.GetValueOrDefault(skillName);
            
            skill.ShowSkillRange();
        }

        public void CancelSkill(string skillName)
        {
            if(!skills.ContainsKey(skillName))
                return;
            
            BaseSkill skill = skills.GetValueOrDefault(skillName);
            
            skill.BlockThisSkill();
            skill.ResetSTile();
        }

        public void AddSkill(SkillSO skillSO)
        {
            if (_skillList.Count == 2)
                return;
            
            if (skillSO == null) return;
            _skillList.Add(skillSO);

            var type = Type.GetType(skillSO.className);

            var components = _unit.GetComponentsInChildren(type, true);

            if (components.Length > 0)
            {
                BaseSkill component = components[0] as BaseSkill;
             
                skills.Add(skillSO.skillName, component);
                skills.GetValueOrDefault(skillSO.skillName).InitializeSkill();
            }

        }

        public void RemoveSkill(SkillSO skillSO)
        {
            _skillList.Remove(skillSO);

            skills.Remove(skillSO.skillName);
        }
    }
}