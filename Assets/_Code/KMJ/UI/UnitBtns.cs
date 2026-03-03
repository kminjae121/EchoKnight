using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using GameEventChannel;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class UnitBtns : MonoBehaviour
    { 
        [SerializeField] private UnitStorageSO unitStorageSO;

        [SerializeField] private List<Button> unitSelectBtns;

        private void Awake()
        {
            SetUnitSelectBtn();
        }

        private void SetUnitSelectBtn()
        {
            unitSelectBtns.ForEach(Btn =>
            {
                Btn.gameObject.SetActive(true);
            });
            
            for (int i = 0; i < unitSelectBtns.Count; i++)
            {
                if (unitStorageSO.unitStates.Count <= i)
                {
                    unitSelectBtns[i].gameObject.SetActive(false);
                }
                else
                {
                    unitSelectBtns[i].onClick.RemoveAllListeners();
                    int capturedIndex = i;
                    unitSelectBtns[i].onClick.AddListener(() => SetUnitSelect(capturedIndex));
                    unitSelectBtns[i].GetComponent<Image>().sprite =
                        unitStorageSO.unitStates[capturedIndex].Data.UnitImage;
                }
            }
            
            unitSelectBtns[0].onClick?.Invoke();
        }

        private void SetUnitSelect(int idx)
        {
            Debug.Log(idx);
            Bus<SendUnitInfoEvent>.Raise(new SendUnitInfoEvent(unitStorageSO.unitStates[idx]));
        }
        
        
    }
}