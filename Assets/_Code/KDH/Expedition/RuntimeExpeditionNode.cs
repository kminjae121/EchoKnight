using System.Collections.Generic;
using Code.Expedition.Data;
using UnityEngine;

namespace Code.Expedition
{
    public class RuntimeExpeditionNode
    {
        public ExpeditionNodeSO Data { get; private set; }
        public List<RuntimeExpeditionNode> NextNodes { get; private set; }
        public bool IsVisited { get; private set; }
        public bool IsLocked { get; private set; }
        public int LayerIndex { get; private set; }

        public RuntimeExpeditionNode(ExpeditionNodeSO data, int layerIndex)
        {
            Data = data;
            LayerIndex = layerIndex;
            NextNodes = new List<RuntimeExpeditionNode>();
            IsVisited = false;
            IsLocked = true;
        }

        public void AddConnection(RuntimeExpeditionNode nextNode)
        {
            if (nextNode != null && !NextNodes.Contains(nextNode))
                NextNodes.Add(nextNode);
        }

        public void SetVisited() => IsVisited = true;
        public void SetLocked(bool isLocked) => IsLocked = isLocked;
    }
}