using Code.Expedition.Data;
using Code.Core.Events.Bus;
using UnityEngine;

namespace Code.Expedition
{
    public struct ExpeditionNodeArriveEvent : IEvent
    {
        public RuntimeExpeditionNode Node;
        public ExpeditionNodeArriveEvent(RuntimeExpeditionNode node) { Node = node; }
    }

    public struct ExpeditionNodeSelectEvent : IEvent
    {
        public RuntimeExpeditionNode SelectedNode;
        public ExpeditionNodeSelectEvent(RuntimeExpeditionNode selectedNode) { SelectedNode = selectedNode; }
    }

    public struct ShowEventPopupEvent : IEvent
    {
        public EventNodeSO EventData;
        public ShowEventPopupEvent(EventNodeSO data) { EventData = data; }
    }
}