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

        private UnitSO[] _partyUnits;

        private void Awake()
        {
            _partyUnits = new UnitSO[maxUnitCount];

            Bus<PartyCharacterSelectEvent>.Subscribe(HandleCharacterSelected);
            Bus<PartyCharacterDeselectEvent>.Subscribe(HandleCharacterDeselected);
            
            startButton.onClick.AddListener(HandleStartButton);
        }

        private void Start()
        {
            for (int i = 0; i < characterSlots.Count; i++)
            {
                if (characterSlots[i] != null)
                {
                    characterSlots[i].UpdateSlot(null);
                }
            }
        }

        private void OnDestroy()
        {
            Bus<PartyCharacterSelectEvent>.Unsubscribe(HandleCharacterSelected);
            Bus<PartyCharacterDeselectEvent>.Unsubscribe(HandleCharacterDeselected);

            startButton.onClick.RemoveListener(HandleStartButton);
        }

        private void HandleCharacterSelected(PartyCharacterSelectEvent evt)
        {
            for (int i = 0; i < _partyUnits.Length; i++)
            {
                if (_partyUnits[i] == evt.Unit) return;
            }

            for (int i = 0; i < _partyUnits.Length; i++)
            {
                if (_partyUnits[i] == null)
                {
                    _partyUnits[i] = evt.Unit;
                    
                    if (i < characterSlots.Count)
                    {
                        characterSlots[i].UpdateSlot(evt.Unit); 
                    }
                    break;
                }
            }
        }

        private void HandleCharacterDeselected(PartyCharacterDeselectEvent evt)
        {
            for (int i = 0; i < _partyUnits.Length; i++)
            {
                if (_partyUnits[i] == evt.Unit)
                {
                    _partyUnits[i] = null;
                    
                    if (i < characterSlots.Count)
                    {
                        characterSlots[i].UpdateSlot(null); 
                    }
                    break;
                }
            }
        }

        private void HandleStartButton()
        {
            bool hasUnit = false;
            unitStorage.units.Clear();
            unitStorage.unitStates.Clear();

            foreach (var unit in _partyUnits)
            {
                if (unit != null)
                {
                    hasUnit = true;
                    unitStorage.units.Add(unit.UnitSpawn);
                    unitStorage.unitStates.Add(new UnitState(unit));
                }
            }

            if (!hasUnit)
            {
                UnityLogger.Log("파티에 유닛이 없습니다.");
                return;
            }

            SceneChangeManager.Instance.ChangeSelectScene("ExpeditionMapScene");
        }
    }
}