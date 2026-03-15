using System;
using System.Collections.Generic;
using System.Linq;
using _Code.Core.Managers;
using Code.Core.Events.Bus;
using UnityEngine;

namespace Code.UnitSystem.SkillSystem
{
    public class SkillComponent : MonoBehaviour, IUnitComponent
    {
        [SerializeField] private List<SkillSO> _skillList;
        
        public Dictionary<string, BaseSkill> skills;

        private Unit _unit;
        private bool isUseSkill = true;
        
        public void Initialize(Unit owner)
        {
            _unit = owner;
            _skillList = SkillSendManager.Instance.GetEquipSkills(_unit.unitSO.UnitType).ToList();
            
            skills = new Dictionary<string, BaseSkill>();
            
            foreach (var skillSo in _skillList)
            {
                if (skillSo == null || string.IsNullOrEmpty(skillSo.className))
                    continue;

                Type type = GetTypeByName(skillSo.className);

                if (type == null)
                {
                    Debug.LogError($"[SkillComponent] '{_unit.name}'의 스킬 '{skillSo.skillName}'에 해당하는 클래스 '{skillSo.className}'를 찾을 수 없습니다. (네임스페이스 확인 필요)");
                    continue;
                }
                
                var components = _unit.GetComponentsInChildren(type, true);

                if (components.Length > 0)
                {
                    BaseSkill component = components[0] as BaseSkill;
                    
                    if (component != null)
                    {
                        component.UseSkillPoint = skillSo.UsingSkillCost;
                        skills.TryAdd(skillSo.skillName, component);
                    }
                }
                else
                    Debug.LogWarning($"[SkillComponent] '{_unit.name}'에 스킬 컴포넌트 '{type.Name}'가 부착되어 있지 않습니다.");
            }

            if (skills.Count > 0)
                foreach (var skill in skills.Values)
                    skill.InitializeSkill();
        }
        
        public void UpdateSkillUI()
        {
            for (int i = 0; i <= 2; i++)
                Bus<SkillUIEvent>.Raise(new SkillUIEvent(i, null, null));
            
            for (int i = 0; i < _skillList.Count; ++i)
                Bus<SkillUIEvent>.Raise(new SkillUIEvent(i, _skillList[i], this));
        }
        private void Awake()
        {
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
                type = assembly.GetTypes().FirstOrDefault(t => t.Name == className || t.FullName == className || t.FullName.EndsWith($".{className}"));
                
                if (type != null)
                    return type;
            }

            return null;
        }


        private void BooleanSkill(UsingSkillEvent evt)
        {
            isUseSkill = evt.isUsingSkill;
        }
        
        public void StartSkill(string skillName)
        {
            if (isUseSkill)
            {
                if (!skills.ContainsKey(skillName))
                    return;
            
                BaseSkill skill = skills.GetValueOrDefault(skillName);
                
                if (skill != null)
                {
                    skill.ShowSkillRange();
                    Bus<UsingSkillEvent>.Raise(new UsingSkillEvent(false));
                }
            }
        }

        public void CancelSkill(string skillName)
        {
            if(!skills.ContainsKey(skillName))
                return;
            
            BaseSkill skill = skills.GetValueOrDefault(skillName);
            
            if (skill != null)
            {
                skill.BlockThisSkill();
                skill.skillEnd();
                Bus<UsingSkillEvent>.Raise(new UsingSkillEvent(true));
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