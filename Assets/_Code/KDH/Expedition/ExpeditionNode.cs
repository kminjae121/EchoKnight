using System.Collections.Generic;
using Code.Expedition.Data;
using EPOOutline;
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

        [Header("Visual")]
        [SerializeField] private Outlinable outlinable;

        [Header("Connections")]
        [SerializeField] private List<ExpeditionPath> connectedPaths = new List<ExpeditionPath>();

        public ExpeditionNodeSO NodeData => nodeData;
        public string TargetSceneName => targetSceneName;
        public bool IsUnlocked => isUnlocked;
        public List<ExpeditionPath> ConnectedPaths => connectedPaths;

        private void Awake()
        {
            if (outlinable == null)
                outlinable = GetComponent<Outlinable>();

            SetOutline(false);
        }

        public void SetUnlocked(bool unlocked)
        {
            isUnlocked = unlocked;
        }

        public void SetCleared(bool cleared)
        {
            isCleared = cleared;
        }

        // [추가] 아웃라인 활성화/비활성화 함수
        public void SetOutline(bool isActive)
        {
            if (outlinable != null)
            {
                outlinable.enabled = isActive;
            }
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