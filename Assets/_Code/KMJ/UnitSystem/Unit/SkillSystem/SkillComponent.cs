using System;
using System.Collections.Generic;
using System.Linq;
using _Code.Core.Managers;
using Code.Core.Events.Bus;
using Code.Core.Managers;
using Code.UnitSystem.Combat;
using UnityEngine;

namespace Code.UnitSystem.SkillSystem
{
    public class SkillComponent : MonoBehaviour, IUnitComponent
    {
        [SerializeField] private List<SkillSO> _skillList;

        public Dictionary<SkillSO, BaseSkill> skills;

        private UnitStatCompo _statCompo;
        private Unit _unit;

        private float basicDamage = 0;
        private bool isUseSkill = true;

        public void Initialize(Unit owner)
        {
            _unit = owner;
            
            if (_unit != null && _statCompo == null)
                _statCompo = _unit.GetUnitCompo<UnitStatCompo>();
            
            SkillSendManager.Instance.GetEquipSkills(_unit.unitSO.UnitType).ToList().ForEach(skill =>
            {
                _skillList.Add(skill);
            });
            
            skills = new Dictionary<SkillSO, BaseSkill>();
            

            foreach (var skillSo in _skillList)
            {
                if (skillSo == null || string.IsNullOrEmpty(skillSo.className))
                    continue;

                Type type = GetTypeByName(skillSo.className);

                if (type == null)
                {
                    Debug.LogError(
                        $"[SkillComponent] '{_unit.name}'의 스킬 '{skillSo.skillName}'에 해당하는 클래스 '{skillSo.className}'를 찾을 수 없습니다. (네임스페이스 확인 필요)");
                    continue;
                }

                var components = _unit.GetComponentsInChildren(type, true);

                if (components.Length > 0)
                {
                    BaseSkill component = components[0] as BaseSkill;

                    if (component != null)
                    {
                        component.UseSkillPoint = skillSo.UsingSkillCost;
                        skills.TryAdd(skillSo, component);
                    }
                }
                else
                    Debug.LogWarning($"[SkillComponent] '{_unit.name}'에 스킬 컴포넌트 '{type.Name}'가 부착되어 있지 않습니다.");
            }
            

            if (skills.Count > 0)
                foreach (var skill in skills.Values)
                {
                    if (skill.SkillType == SkillType.ActiveSkill)
                    {
                        if (_statCompo != null)
                        {
                            float skillDamageValue = _statCompo.GetStat<float>(StatInfo.SkillDamage);
                            float floatdamage = skill.basicSkillDamage * skillDamageValue;
                            basicDamage = (int)floatdamage;
                        }
                        else
                            basicDamage = skill.basicSkillDamage;
                    }
                    else if(skill.SkillType == SkillType.BasicSkill)
                    {
                        if (_statCompo != null)
                        {
                            float atkDamage = _statCompo.GetStat<float>(StatInfo.AtkDamage);
                            float floatdamage = atkDamage;
                            basicDamage = (int)floatdamage;
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
            Bus<SkillUIEvent>.Raise(new SkillUIEvent(_skillList, this));
        }

        public void SetAddSkillDamage(float addDamage,SkillType skillType)
        {
            foreach (var skill in skills.Values)
            {
                if (skill.SkillType == skillType)
                {
                    float damage = basicDamage + addDamage;
                    skill.SetDamage(damage);
                }
            }
        }
        public void StartSkill(SkillSO skillSO)
        {
            if (isUseSkill)
            {
                if (!skills.ContainsKey(skillSO))
                    return;

                BaseSkill skill = skills.GetValueOrDefault(skillSO);

                if (skill != null)
                {
                    skill.ConfigureSkillRange(skillSO);
                    skill.ShowSkillRange();
                    Bus<UsingSkillEvent>.Raise(new UsingSkillEvent(false));
                }
            }
        }

        public void CancelAllSkill()
        {
            foreach (var skill in skills.Values)
            {
                skill.skillEnd();
                skill.BlockThisSkill();
                Bus<UsingSkillEvent>.Raise(new UsingSkillEvent(true));
            }
        }
    }
}
