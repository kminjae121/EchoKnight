using System;
using System.Collections.Generic;
using Code.Core.Events.Bus;
using TMPro;
using UnityEngine;

namespace Code.UI
{
    [Serializable]
    public struct UnitModelMapping
    {
        public UnitType unitType;
        public GameObject modelPrefab;
    }

    public class PartyCharacterInfoUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject lobbyStatPanel;
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI characterClassText;

        [Header("3D Model Settings")]
        [SerializeField] private Transform[] modelSpawnPoints;
        [SerializeField] private List<UnitModelMapping> unitModelMappings;

        [Header("Stat TMPs")]
        [SerializeField] private TextMeshProUGUI maxHealthText;
        [SerializeField] private TextMeshProUGUI atkText;
        [SerializeField] private TextMeshProUGUI defText;
        [SerializeField] private TextMeshProUGUI moveSpeedText;
        [SerializeField] private TextMeshProUGUI turnSpeedText;
        [SerializeField] private TextMeshProUGUI criticalProbabilityText;
        [SerializeField] private TextMeshProUGUI criticalDamageIncreaseText;
        [SerializeField] private TextMeshProUGUI maxSkillCostText;
        [SerializeField] private TextMeshProUGUI recoverySkillCostText;

        private GameObject[] _spawnedModels = new GameObject[3];
        private List<UnitSO> _selectedUnits = new List<UnitSO>();

        private void Awake()
        {
            Bus<PartyCharacterHoverEvent>.Subscribe(HandleHover);
            Bus<PartyCharacterSelectEvent>.Subscribe(HandleSelect);
            Bus<PartyCharacterDeselectEvent>.Subscribe(HandleDeselect);

            if (lobbyStatPanel != null) lobbyStatPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            Bus<PartyCharacterHoverEvent>.Unsubscribe(HandleHover);
            Bus<PartyCharacterSelectEvent>.Unsubscribe(HandleSelect);
            Bus<PartyCharacterDeselectEvent>.Unsubscribe(HandleDeselect);
            
            CleanupAllModels();
        }

        private void HandleHover(PartyCharacterHoverEvent evt)
        {
            bool isHovering = evt.Unit != null;
            
            if (lobbyStatPanel != null) 
                lobbyStatPanel.SetActive(isHovering);

            if (isHovering)
            {
                UpdateStats(evt.Unit);
            }
        }

        private void HandleSelect(PartyCharacterSelectEvent evt)
        {
            if (evt.Unit == null) return;

            if (!_selectedUnits.Contains(evt.Unit) && _selectedUnits.Count < 3)
            {
                _selectedUnits.Add(evt.Unit);
                RefreshAllCharacterModels();
            }
        }

        private void HandleDeselect(PartyCharacterDeselectEvent evt)
        {
            if (evt.Unit == null) return;

            if (_selectedUnits.Contains(evt.Unit))
            {
                _selectedUnits.Remove(evt.Unit);
                RefreshAllCharacterModels();
            }
        }

        private void RefreshAllCharacterModels()
        {
            CleanupAllModels();

            for (int i = 0; i < _selectedUnits.Count; i++)
            {
                if (i >= modelSpawnPoints.Length) break;

                UnitSO unit = _selectedUnits[i];
                GameObject prefabToSpawn = GetModelPrefab(unit.UnitType);

                if (prefabToSpawn != null && modelSpawnPoints[i] != null)
                {
                    _spawnedModels[i] = Instantiate(prefabToSpawn, modelSpawnPoints[i]);
                    _spawnedModels[i].transform.localPosition = Vector3.zero;
                    _spawnedModels[i].transform.localRotation = Quaternion.identity;
                }
            }
        }

        private GameObject GetModelPrefab(UnitType unitType)
        {
            if (unitModelMappings == null) return null;

            foreach (var mapping in unitModelMappings)
            {
                if (mapping.unitType == unitType)
                    return mapping.modelPrefab;
            }
            return null;
        }

        private void CleanupAllModels()
        {
            for (int i = 0; i < _spawnedModels.Length; i++)
            {
                if (_spawnedModels[i] != null)
                {
                    Destroy(_spawnedModels[i]);
                    _spawnedModels[i] = null;
                }
            }
        }

        private void UpdateStats(UnitSO data)
        {
            if (data == null) return;

            if (characterNameText != null) characterNameText.text = data.UnitName ?? string.Empty;
            if (characterClassText != null) characterClassText.text = data.UnitClass ?? string.Empty;

            if (maxHealthText != null) maxHealthText.text = data.Maxhealth.ToString("F1");
            if (atkText != null) atkText.text = data.AttackDamage.ToString("F1");
            if (defText != null) defText.text = data.DefensivePower.ToString("F1");
            if (moveSpeedText != null) moveSpeedText.text = data.MoveRange.ToString("F1");
            if (turnSpeedText != null) turnSpeedText.text = data.turnSpeed.ToString("F1");
            if (criticalProbabilityText != null) criticalProbabilityText.text = $"{data.CriticalProbability:F1}%";
            if (criticalDamageIncreaseText != null) criticalDamageIncreaseText.text = data.CriticalDamageIncrease.ToString("F1");
            if (maxSkillCostText != null) maxSkillCostText.text = data.MaxSkillCost.ToString();
            if (recoverySkillCostText != null) recoverySkillCostText.text = data.RecoverySkillCost.ToString();
        }
    }
}