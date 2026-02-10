using System;
using Code.Core.Events.Bus;
using TMPro;
using UnityEngine;

namespace Code.UI
{
    public class CharacterSkillInfoUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI SkillCostText;
        [SerializeField] private GameObject SkillPrefab;

        private UnitSO _unit;

        private void Awake()
        {
            Bus<CharacterInfoEvent>.Subscribe(HandleCharacterInfo);
            
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Bus<CharacterInfoEvent>.Unsubscribe(HandleCharacterInfo);
        }
        
        private void HandleCharacterInfo(CharacterInfoEvent evt)
        {
            _unit = evt.Unit.Data;

            RefreshUI();
        }
        
        private void RefreshUI()
        {
            
            
            SkillCostText.text = $"0 / {_unit.Cost}";
        }
    }
}