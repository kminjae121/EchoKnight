using TMPro;
using System;
using Code.Expedition;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Expedition.UI
{
    public class ExpeditionNodeView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image nodeIcon;
        [SerializeField] private Image lockIcon;
        [SerializeField] private TextMeshProUGUI nodeNameText;
        [SerializeField] private Button nodeButton;

        [Header("State Colors")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color lockedColor = Color.gray;
        [SerializeField] private Color visitedColor = Color.yellow;

        private RuntimeExpeditionNode _targetNode;
        private Action<RuntimeExpeditionNode> _onClickCallback;

        public void Initialize(RuntimeExpeditionNode node, Action<RuntimeExpeditionNode> onClick)
        {
            _targetNode = node;
            _onClickCallback = onClick;

            if (node.Data != null)
            {
                if (nodeIcon != null) nodeIcon.sprite = node.Data.icon;
                if (nodeNameText != null) nodeNameText.text = node.Data.nodeName;
            }

            nodeButton.onClick.RemoveAllListeners();
            nodeButton.onClick.AddListener(OnClickNode);

            UpdateVisualState();
        }

        private void UpdateVisualState()
        {
            if (_targetNode.IsLocked)
            {
                nodeButton.interactable = false;
                if (nodeIcon != null) nodeIcon.color = lockedColor;
                if (lockIcon != null) lockIcon.gameObject.SetActive(true);
            }
            else
            {
                nodeButton.interactable = true;
                if (nodeIcon != null) nodeIcon.color = _targetNode.IsVisited ? visitedColor : normalColor;
                if (lockIcon != null) lockIcon.gameObject.SetActive(false);
            }
        }

        private void OnClickNode()
        {
            _onClickCallback?.Invoke(_targetNode);
        }
    }
}