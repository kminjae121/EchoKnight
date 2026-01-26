using System;
using System.Collections.Generic;
using System.Linq;
using _Code.KMJ.UnitSystem.involveUnitSO;
using Code.Core.Events.Bus;
using UnityEngine;

namespace Code.UnitSystem.SkillSystem
{
    public class SkillComponent : MonoBehaviour, IUnitComponent
    {
        [SerializeField] private List<SkillSO> _skillList;
        
        public int currentSkillCost = 10;
        
        [SerializeField] private UnitSkillStorageSO storageSO = null;
        
        public Dictionary<string, BaseSkill> skills = new Dictionary<string, BaseSkill>();

        private Unit _unit;

        private bool isUseSkill = true;
        
        public void Initialize(Unit owner)
        {
            if (storageSO != null&& storageSO.skills != null)
            {
                storageSO.skills.ToList().ForEach(skillinfo =>
                {
                    _skillList.Add(skillinfo);
                });
            }
            
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

        private void Awake()
        {
            Bus<UsingSkillEvent>.Subscribe(BooleanSkill);
        }

        private void OnDestroy()
        {
            Bus<UsingSkillEvent>.Unsubscribe(BooleanSkill);
        }

        private void BooleanSkill(UsingSkillEvent evt)
        {
            isUseSkill = evt.isUsingSkill;
        }

        public void InitializeSkills()
        {
        }
        
        public void StartSkill(string skillName)
        {
            if (isUseSkill)
            {
                if(!skills.ContainsKey(skillName))
                    return;
            
                BaseSkill skill = skills.GetValueOrDefault(skillName);
            
                skill.ShowSkillRange();
                Bus<UsingSkillEvent>.Raise(new UsingSkillEvent(false));
            }
        }

        public void CancelSkill(string skillName)
        {
            if(!skills.ContainsKey(skillName))
                return;
            
            BaseSkill skill = skills.GetValueOrDefault(skillName);
            
            skill.BlockThisSkill();
            skill.skillEnd();
            
            Bus<UsingSkillEvent>.Raise(new UsingSkillEvent(true));
        }

        public void CancelAllSkill()
        {
            skills.Values.ToList().ForEach(skill =>
            {
                skill.skillEnd();
                skill.BlockThisSkill();
                Bus<UsingSkillEvent>.Raise(new UsingSkillEvent(true));
            });
        }

        public void AddSkill(SkillSO skillSO)
        {
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