using Code.Expedition;
using Code.Expedition.Data;
using Code.Core.Events.Bus;
using UnityEngine;

namespace Code.Expedition.Logic
{
    public class EventNodeLogic : INodeLogic
    {
        public void Execute(RuntimeExpeditionNode node)
        {
            if (node.Data is EventNodeSO eventData)
            {
                Debug.Log($"이벤트 노드 진입: {eventData.title}");
                Bus<ShowEventPopupEvent>.Raise(new ShowEventPopupEvent(eventData));
            }
            else
            {
                Debug.LogError("이벤트 노드 데이터 캐스팅 실패");
            }
        }
    }
}