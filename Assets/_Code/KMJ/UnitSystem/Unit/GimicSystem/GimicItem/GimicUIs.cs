using System;
using System.Collections.Generic;
using Code.Core.Events.Bus;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UnitSystem.GimicSystem
{
    public class GimicUIs : MonoBehaviour
    {
        private Dictionary<UnitType, Image> _uiDict = new Dictionary<UnitType, Image>();
        
        [SerializeField] private Image _knightUI;
        [SerializeField] private Image _magicianUI;
        [SerializeField] private Image _archerUI;


        private void Awake()
        {
            _uiDict.Add(UnitType.Knight, _knightUI);
            _uiDict.Add(UnitType.Magician, _magicianUI);
            _uiDict.Add(UnitType.Archer, _archerUI);
            
            Bus<WhatUnitTurnEvent>.Subscribe(UnitUI);
            
            foreach (var value in _uiDict.Values)
            {
                value.gameObject.SetActive(false);
            }

        }

        private void OnDestroy()
        {
            Bus<WhatUnitTurnEvent>.Unsubscribe(UnitUI);
        }

        private void UnitUI(WhatUnitTurnEvent evt)
        {
            foreach (var value in _uiDict.Values)
            {
                value.gameObject.SetActive(false);
            }

            if (_uiDict.GetValueOrDefault(evt.unitType))
            {
                _uiDict.GetValueOrDefault(evt.unitType).gameObject.SetActive(true);
            }
        }
    }
}