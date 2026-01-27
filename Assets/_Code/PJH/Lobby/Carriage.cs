using System.Collections.Generic;
using Code.Core.Debugs;
using Code.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.Lobby
{
    [RequireComponent(typeof(Collider))]
    public class Carriage : MonoBehaviour, IPointerClickHandler
    {
        // 카메라에 Physics Raycaster 달아야 함.

        [SerializeField] private Image carriagePanel;
        [SerializeField] private CarriageCharacterUI character1, character2, character3;
        [SerializeField] private List<RecruitUnitData> unitInfoList;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            RandomSelectCharacter();
            OnClickEvent();
        }
        
        private void OnClickEvent()
        {
            UnityLogger.Log("마차 상호작용");
            carriagePanel.gameObject.SetActive(true);
        }

        private void RandomSelectCharacter()
        {
            // 임시
            character1.gameObject.SetActive(true);
            character2.gameObject.SetActive(true);
            character3.gameObject.SetActive(true);
            
            character1.SetUnitInfo(unitInfoList[Random.Range(0, unitInfoList.Count)]);
            character2.SetUnitInfo(unitInfoList[Random.Range(0, unitInfoList.Count)]);
            character3.SetUnitInfo(unitInfoList[Random.Range(0, unitInfoList.Count)]);
        }
    }
}