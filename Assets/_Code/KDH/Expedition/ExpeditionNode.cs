using System.Collections.Generic;
using Code.Expedition.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Expedition.Components
{
    public class ExpeditionNode : MonoBehaviour
    {
        [Header("Node Data")]
        [SerializeField] private ExpeditionNodeSO nodeData;
        [SerializeField] private string targetSceneName;
        [SerializeField] private bool isUnlocked = false;
        [SerializeField] private bool isCleared = false;

        [Header("UI Visuals")]
        [SerializeField] private Image nodeIcon;
        [SerializeField] private Button nodeButton;
        [SerializeField] private Image outlineImage;
        
        [Header("Colors")]
        [SerializeField] private Color clearedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        [SerializeField] private Color unclearedColor = Color.white;

        [Header("Connections")]
        [SerializeField] private List<ExpeditionNode> connectedNodes = new List<ExpeditionNode>();

        public ExpeditionNodeSO NodeData => nodeData;
        public string TargetSceneName => targetSceneName;
        public bool IsUnlocked => isUnlocked;
        public bool IsCleared => isCleared;
        public List<ExpeditionNode> ConnectedNodes => connectedNodes;

        private void Awake()
        {
            if (nodeButton == null)
                nodeButton = GetComponent<Button>();

            if (nodeButton != null)
            {
                nodeButton.onClick.AddListener(OnNodeClicked);
            }

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

        public void SetOutline(bool isActive)
        {
            if (outlineImage != null)
            {
                outlineImage.enabled = isActive;
            }
        }

        public void UpdateVisual(bool isCurrentNode)
        {
            if (nodeIcon == null) return;

            if (nodeData != null && nodeData.icon != null)
            {
                nodeIcon.sprite = nodeData.icon;
            }

            if (isCleared || isCurrentNode)
            {
                nodeIcon.color = clearedColor;
            }
            else
            {
                nodeIcon.color = unclearedColor;
            }
        }

        private void OnNodeClicked()
        {
            Managers.ExpeditionManager.Instance.OnNodeClicked(this);
        }
    }
}