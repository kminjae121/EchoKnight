using Code.Core.Debugs;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.Lobby
{
    [RequireComponent(typeof(Collider))]
    public class Carriage : MonoBehaviour, IPointerClickHandler
    {
        // 카메라에 Physics Raycaster 달아야 함.
        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickEvent();
        }
        
        private void OnClickEvent()
        {
            UnityLogger.Log("마차 상호작용");
        }
    }
}