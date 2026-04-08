using System.Collections.Generic;
using _00.Core._02.Scripts._01.Manager;
using Code.Core.Debugs;
using Code.Core.Events.Bus;
using Code.UnitManaging;
using Code.UnitSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class PartyUI : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button startButton;

        [Header("Slots")]
        [SerializeField] private List<SelectedCharacterSlotUI> characterSlots;

        [Header("Data")]
        [SerializeField] private UnitStorageSO unitStorage;
        [SerializeField] private int maxUnitCount = 3;

        private readonly List<UnitSO> _partyUnits = new();

        private void Awake()
        {
            Bus<PartyCharacterSelectEvent>.Subscribe(HandleCharacterSelected);
            Bus<PartyCharacterDeselectEvent>.Subscribe(HandleCharacterDeselected);
            
            startButton.onClick.AddListener(HandleStartButton);
        }

        private void OnDestroy()
        {
            Bus<PartyCharacterSelectEvent>.Unsubscribe(HandleCharacterSelected);
            Bus<PartyCharacterDeselectEvent>.Unsubscribe(HandleCharacterDeselected);

            startButton.onClick.RemoveListener(HandleStartButton);
        }

        private void HandleCharacterSelected(PartyCharacterSelectEvent evt)
        {
            if (_partyUnits.Count >= maxUnitCount || _partyUnits.Contains(evt.Unit))
                return;

            _partyUnits.Add(evt.Unit);
            RefreshSlots();
        }

        private void HandleCharacterDeselected(PartyCharacterDeselectEvent evt)
        {
            if (_partyUnits.Remove(evt.Unit))
                RefreshSlots();
        }

        private void RefreshSlots()
        {
            for (int i = 0; i < characterSlots.Count; ++i)
                characterSlots[i].SetUnit(i < _partyUnits.Count ? _partyUnits[i] : null);
        }

        private void HandleStartButton()
        {
            if (_partyUnits.Count == 0)
            {
                UnityLogger.Log("파티에 유닛이 없습니다.");
                return;
            }

            unitStorage.units.Clear();
            unitStorage.unitStates.Clear();

            foreach (var unit in _partyUnits)
            {
                unitStorage.units.Add(unit.UnitSpawn);
                unitStorage.unitStates.Add(new UnitState(unit));
            }

            SceneChangeManager.Instance.ChangeSelectScene("ExpeditionMapScene");
        }
    }
}