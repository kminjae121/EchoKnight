using System;
using System.Collections.Generic;
using System.Linq;
using _Code.KMJ.UnitSystem;
using Code.SkillSystem;
using Code.UnitSystem;
using NUnit.Framework;
using UnityEngine;

namespace _Code.Passive
{
    public class PassiveComponent : MonoBehaviour, IUnitComponent
    {
        private Unit _unit;

        private List<PassiveSO> _passiveList;
        
        private Dictionary<PassiveSO, BasePassive> _passiveDict;
        
        public void Initialize(Unit owner)
        {
            _unit = owner;

            _passiveList = PassiveStorage.Instance.GetPassive(_unit.unitSO.UnitType);

            FindPassive();
        }

        private void Start()
        {
            StartAllPassives();
        }

        private void OnDestroy()
        {
            StopAllPassives();
        }

        private void FindPassive()
        {
            foreach (var passiveData in _passiveList)
            {
                if (passiveData == null || string.IsNullOrEmpty(passiveData.ClassName))
                    continue;

                Type type = GetTypeByName(passiveData.ClassName);

                if (type == null)
                {
                    Debug.LogError($"[Passive] '{_unit.name}'의 패시브 '{passiveData.PassiveName}'에 해당하는 클래스 '{passiveData.ClassName}'를 찾을 수 없습니다. (네임스페이스 확인 필요)");
                    continue;
                }

                var component = _unit.GetComponentInChildren(type, true);

                if (component is BasePassive basePassive)
                {
                    if (component == null)
                        continue;
                    _passiveDict.TryAdd(passiveData, basePassive);
                }
                else
                    Debug.LogWarning($"[Passive] '{_unit.name}'에 패시프 컴포넌트에 '{type.Name}'가 부착되어 있지 않습니다.");
            }
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

        public void StartAllPassives()
        {
            foreach (var passive in _passiveDict)
            {
                passive.Value.StartPassive();
            }
        }

        public void StopAllPassives()
        {
            foreach (var passive in _passiveDict)
            {
                passive.Value.StopPassive();
            }
        }

        public void StartPassive(PassiveSO passive)
        {
            _passiveDict.GetValueOrDefault(passive)?.StartPassive();
        }

        public void StopPassive(PassiveSO passive)
        {
            _passiveDict.GetValueOrDefault(passive)?.StopPassive();
        }
    }
}