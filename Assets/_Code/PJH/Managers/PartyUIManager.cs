using System.Collections.Generic;
using Code.UI;
using Code.UnitSystem;
using UnityEngine;

namespace Code.Managers
{
    public class PartyUIManager : MonoBehaviour
    {
        [SerializeField] private UnitStorageSO unitStorage;
        [SerializeField] private List<CharacterStateUI> characterUIList;
        
        private void Start()
        {
            BindPartyUnits();
        }
        
        private void BindPartyUnits()
        {
            for (int i = 0; i < characterUIList.Count; ++i)
                if (i < unitStorage.unitStates.Count)
                    characterUIList[i].SetUnit(unitStorage.unitStates[i]);
                else
                    characterUIList[i].gameObject.SetActive(false);
        }
    }
}