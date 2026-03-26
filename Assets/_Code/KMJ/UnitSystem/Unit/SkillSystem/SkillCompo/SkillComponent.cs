using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Events.Bus;
using Code.Core.Managers;
using UnityEngine;

namespace Code.UnitSystem.SkillSystem
{
    public abstract class SkillComponent : MonoBehaviour, IUnitComponent
    {
        [SerializeField] protected List<SkillSO> skillList;

        public Dictionary<SkillSO, BaseSkill> Skills { get; private set; }

        protected UnitStatCompo _statCompo;
        protected Unit _unit;

        protected float basicDamage = 0;
        protected bool isUseSkill = true;

        public void Initialize(Unit owner)
        {
            _unit = owner;
            
            if (_unit != null && _statCompo == null)
                _statCompo = _unit.GetUnitCompo<UnitStatCompo>();

            foreach (var skill in SkillSendManager.Instance.GetEquipSkills(_unit.unitSO.UnitType))
                skillList.Add(skill);
            
            Skills = new Dictionary<SkillSO, BaseSkill>();
            

            foreach (var skillData in skillList)
            {
                if (skillData == null || string.IsNullOrEmpty(skillData.className))
                    continue;

                Type type = GetTypeByName(skillData.className);

                if (type == null)
                {
                    Debug.LogError($"[SkillComponent] '{_unit.name}'의 스킬 '{skillData.skillName}'에 해당하는 클래스 '{skillData.className}'를 찾을 수 없습니다. (네임스페이스 확인 필요)");
                    continue;
                }

                var component = _unit.GetComponentInChildren(type, true);

                if (component is BaseSkill baseSkill)
                {

                    if (component == null)
                        continue;
                    Skills.TryAdd(skillData, baseSkill);
                }
                else
                    Debug.LogWarning($"[SkillComponent] '{_unit.name}'에 스킬 컴포넌트 '{type.Name}'가 부착되어 있지 않습니다.");
            }
            
            if (Skills.Count > 0)
                foreach (var skill in Skills.Values)
                {
                    if (skill.SkillSO.SkillType == SkillType.ActiveSkill)
                    {
                        if (_statCompo != null)
                        {
                            float skillDamageValue = _statCompo.GetStat(StatInfo.SkillDamage);
                            float floatDamage = skill.basicSkillDamage * skillDamageValue;
                            basicDamage = (int)floatDamage;
                        }
                        else
                            basicDamage = skill.basicSkillDamage;
                    }
                    else if(skill.SkillSO.SkillType == SkillType.BasicSkill)
                    {
                        if (_statCompo != null)
                        {
                            float atkDamage = _statCompo.GetStat(StatInfo.AtkDamage);
                            float floatDamage = atkDamage;
                            basicDamage = (int)floatDamage;
                        }
                        else
                            basicDamage = skill.basicSkillDamage;
                    }
                    
                    skill.InitializeSkill();
                    skill.SetDamage(basicDamage);   
                }
            
            Bus<UsingSkillEvent>.Subscribe(BooleanSkill);
        }

        private void OnDestroy()
        {
            Bus<UsingSkillEvent>.Unsubscribe(BooleanSkill);
        }
        
        private Type GetTypeByName(string className)
        {
            Type type = Type.GetType(className);

            if (type != null)
                return type;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetTypes().FirstOrDefault(t =>
                    t.Name == className || t.FullName == className || t.FullName.EndsWith($".{className}"));

                if (type != null)
                    return type;
            }

            return null;
        }
        
        private void BooleanSkill(UsingSkillEvent evt)
        {
            isUseSkill = evt.isUsingSkill;
        }
        
        public void UpdateSkillUI()
        {
            Bus<SkillUIEvent>.Raise(new SkillUIEvent(skillList, this));
        }

        public void SetAddSkillDamage(float addDamage,SkillType skillType)
        {
            foreach (var skill in Skills.Values)
            {
                if (skill.SkillSO.SkillType == skillType)
                {
                    float damage = basicDamage + addDamage;
                    skill.SetDamage(damage);
                }
            }
        }
        
        public void StartSkill(SkillSO skillSO)
        {
            if (!isUseSkill)
                return;
            
            if (!Skills.ContainsKey(skillSO))
                return;

            BaseSkill skill = Skills.GetValueOrDefault(skillSO);

            if (skill != null)
            {
                StartSkill(skill,skillSO);
            }
        }

        public void CancelAllSkill()
        {
            foreach (var skill in Skills.Values)
            {
                CancelSkill(skill);
            }
        }

        protected virtual void StartSkill(BaseSkill skill, SkillSO skillSO)
        {
            
        }

        protected virtual void CancelSkill(BaseSkill skill)
        {
            
        }
    }
}
