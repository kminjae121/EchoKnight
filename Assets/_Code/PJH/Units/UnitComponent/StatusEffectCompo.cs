using System;
using System.Collections.Generic;
using System.Reflection;
using Code.Combat.StatusEffect;
using UnityEngine;

namespace Code.UnitSystem.UnitComponent
{
    public class StatusEffectCompo : MonoBehaviour, IUnitComponent
    {
        private static Dictionary<EffectType, Type> _effectTypeFactory;

        private Unit _owner;
        private Dictionary<EffectType, StatusEffect> _activeEffectDict;
        private List<StatusEffect> _activeEffects;
        private int _statusEffectBit;

        public void Initialize(Unit owner)
        {
            _owner = owner;
            _activeEffects = new List<StatusEffect>();
            _activeEffectDict = new Dictionary<EffectType, StatusEffect>();
            _statusEffectBit = 0;

            CacheEffectTypes();
        }

        // 턴마다 실행
        public void UpdateStatusEffects()
        {
            for (int i = _activeEffects.Count - 1; i >= 0; --i)
            {
                StatusEffect effect = _activeEffects[i];
                effect.UpdateEffect();

                if (effect.IsCompleted())
                    RemoveStatusEffect(effect.EffectType);
            }
        }

        public StatusEffect AddStatusEffect(EffectType effectType, int duration)
        {
            if (_owner == null || effectType == EffectType.None || duration <= 0)
                return null;

            if (_activeEffectDict.TryGetValue(effectType, out StatusEffect activeEffect))
            {
                activeEffect.EndEffect();
                _activeEffects.Remove(activeEffect);
                _activeEffectDict.Remove(effectType);
            }

            StatusEffect effect = CreateEffect(effectType);

            if (effect == null)
                return null;

            effect.ApplyEffect(_owner, duration);
            _activeEffects.Add(effect);
            _activeEffectDict[effectType] = effect;
            _statusEffectBit |= (int)effectType;

            return effect;
        }

        public void RemoveStatusEffect(EffectType effectType)
        {
            if (_activeEffectDict.TryGetValue(effectType, out StatusEffect effect) == false)
                return;

            effect.EndEffect();
            _activeEffects.Remove(effect);
            _activeEffectDict.Remove(effectType);
            _statusEffectBit &= ~(int)effectType;
        }

        public bool IsUnderStatusEffect(EffectType effectType)
            => (_statusEffectBit & (int)effectType) != 0;

        private static void CacheEffectTypes()
        {
            if (_effectTypeFactory != null)
                return;

            _effectTypeFactory = new Dictionary<EffectType, Type>();

            foreach (EffectType effectType in Enum.GetValues(typeof(EffectType)))
            {
                if (effectType == EffectType.None)
                    continue;

                Type effectClassType = ResolveEffectType(effectType);

                if (effectClassType != null)
                    _effectTypeFactory[effectType] = effectClassType;
            }
        }

        private static Type ResolveEffectType(EffectType effectType)
        {
            string expectedTypeName = $"{effectType}StatusEffect";

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types;

                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types;
                }

                for (int i = 0; i < types.Length; i++)
                {
                    Type type = types[i];

                    if (type == null || type.IsAbstract)
                        continue;

                    if (type.Name != expectedTypeName || !typeof(StatusEffect).IsAssignableFrom(type))
                        continue;

                    return type;
                }
            }

            Debug.LogWarning($"[{nameof(StatusEffectCompo)}] {expectedTypeName} type was not found.");
            return null;
        }

        private StatusEffect CreateEffect(EffectType effectType)
        {
            if (!_effectTypeFactory.TryGetValue(effectType, out Type effectClassType))
            {
                Debug.LogWarning($"[{nameof(StatusEffectCompo)}] {effectType} effect is not registered.");
                return null;
            }

            if (Activator.CreateInstance(effectClassType) is not StatusEffect effect)
            {
                Debug.LogWarning($"[{nameof(StatusEffectCompo)}] Failed to create {effectClassType.Name} instance.");
                return null;
            }

            effect.Initialize(effectType);
            return effect;
        }
    }
}
