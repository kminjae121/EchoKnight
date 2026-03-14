using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using _Code.KMJ.UnitSystem.involveUnitSO;
using Code.Core.Events.Bus;
using UnityEngine;

namespace Code.UnitSystem.SkillSystem
{
    public class SkillComponent : MonoBehaviour, IUnitComponent
    {
        [SerializeField] private List<SkillSO> _skillList = new List<SkillSO>();
        public int currentSkillCost = 10;
        
        [SerializeField] private UnitSkillStorageSO storageSO = null;
        
        public Dictionary<string, BaseSkill> skills = new Dictionary<string, BaseSkill>();

        private Unit _unit;
        private bool isUseSkill = true;
        
        public void Initialize(Unit owner)
        {
            _unit = owner;
            skills = new Dictionary<string, BaseSkill>();
            
            if (storageSO != null && storageSO.skills != null)
            {
                foreach (var skillInfo in storageSO.skills)
                    if (!_skillList.Contains(skillInfo))
                        _skillList.Add(skillInfo);
            }

            foreach (var skillSo in _skillList)
            {
                if (skillSo == null || string.IsNullOrEmpty(skillSo.className)) continue;

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
                        
                        if (!skills.ContainsKey(skillSo.skillName))
                            skills.Add(skillSo.skillName, component);
                    }
                }
                else
                    Debug.LogWarning($"[SkillComponent] '{_unit.name}'에 스킬 컴포넌트 '{type.Name}'가 부착되어 있지 않습니다.");
            }

            if (skills.Count > 0)
            {
                foreach (var skill in skills.Values)
                    skill.InitializeSkill();
            }
        }
        
        public void UpdateSkillUI()
        {
            for (int i = 0; i <= 2; i++) Bus<SkillUIEvent>.Raise(new SkillUIEvent(i, null,0, null, null));
            
            if (skills != null)
            {
                int idx = 0;
                foreach (var skill in skills)
                {
                    Bus<SkillUIEvent>.Raise(new SkillUIEvent(idx, skill.Key, skill.Value.UseSkillPoint,skill.Value.SkillImage, this));
                    idx++;
                }
            }
        }

        private Type GetTypeByName(string className)
        {
            Type type = Type.GetType(className);
            if (type != null) return type;
            
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetTypes().FirstOrDefault(t => t.Name == className || t.FullName == className || t.FullName.EndsWith($".{className}"));
                if (type != null) return type;
            }

            return null;
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
        
        public void StartSkill(string skillName)
        {
            if (isUseSkill)
            {
                if(!skills.ContainsKey(skillName))
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

        public void AddSkill(SkillSO skillSO)
        {
            if (skillSO == null) return;
            
            if (!_skillList.Contains(skillSO))
                _skillList.Add(skillSO);

            Type type = GetTypeByName(skillSO.className);
            if (type == null) return;

            var components = _unit.GetComponentsInChildren(type, true);

            if (components.Length > 0)
            {
                BaseSkill component = components[0] as BaseSkill;
                if (component != null && !skills.ContainsKey(skillSO.skillName))
                {
                    skills.Add(skillSO.skillName, component);
                    component.InitializeSkill();
                }
            }
        }

        public void RemoveSkill(SkillSO skillSO)
        {
            if (skillSO == null) return;
            
            _skillList.Remove(skillSO);
            skills.Remove(skillSO.skillName);
        }
    }
}