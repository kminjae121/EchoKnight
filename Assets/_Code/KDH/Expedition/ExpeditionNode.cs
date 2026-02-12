using System.Collections.Generic;
using Code.Expedition.Data;
using UnityEngine;

namespace Code.Expedition.Components
{
    public class ExpeditionNode : MonoBehaviour
    {
        [Header("Node Data")]
        [SerializeField] private ExpeditionNodeSO nodeData;
        [SerializeField] private string targetSceneName;
        [SerializeField] private bool isUnlocked = false;
        [SerializeField] private bool isCleared = false;

        [Header("Connections")]
        [SerializeField] private List<ExpeditionPath> connectedPaths = new List<ExpeditionPath>();

        public ExpeditionNodeSO NodeData => nodeData;
        public string TargetSceneName => targetSceneName;
        public bool IsUnlocked => isUnlocked;
        public List<ExpeditionPath> ConnectedPaths => connectedPaths;

        public void SetUnlocked(bool unlocked)
        {
            isUnlocked = unlocked;
        }

        public void SetCleared(bool cleared)
        {
            isCleared = cleared;
        }

        public ExpeditionPath GetPathTo(ExpeditionNode targetNode)
        {
            foreach (var path in connectedPaths)
            {
                if (path.TargetNode == targetNode)
                {
                    return path;
                }
            }
            return null;
        }
    }
}