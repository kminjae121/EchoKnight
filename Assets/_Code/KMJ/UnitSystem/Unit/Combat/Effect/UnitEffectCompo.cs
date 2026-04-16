using System;
using System.Collections.Generic;
using Code.Core.Debugs;
using UnityEngine;

namespace _Code.UnitSystem
{
    enum EffectType
    {
        
    }
    public class UnitEffectCompo : MonoBehaviour
    {
        private Dictionary<string, UnitEffect> _effectDict = new Dictionary<string, UnitEffect>();

        private void Awake()
        {
            UnitEffect[] atkEffect = GetComponentsInChildren<UnitEffect>(true);

            foreach (UnitEffect effect in atkEffect)
            {
                if (string.IsNullOrWhiteSpace(effect.EffectName))
                {
                    UnityLogger.LogWarning($"EffectName이 비어있음: {effect.name}");
                    continue;
                }

                if (_effectDict.ContainsKey(effect.EffectName))
                {
                    UnityLogger.LogWarning($"{effect.EffectName}이 딕셔너리에 이미 존재함");
                    continue;
                }

                _effectDict.Add(effect.EffectName, effect);
            }
        }

        public void PlayTargetEffect(string effectName)
        {
            if (string.IsNullOrWhiteSpace(effectName)) 
                    return;
            
            if (_effectDict.TryGetValue(effectName, out var effect)) 
                effect.PlayEffect();
        }

        public void StopTargetEffect(string effectName)
        {
            if (string.IsNullOrWhiteSpace(effectName)) 
                return;
            
            if (_effectDict.TryGetValue(effectName, out var effect)) 
                effect.StopEffect();
        }
    }
}