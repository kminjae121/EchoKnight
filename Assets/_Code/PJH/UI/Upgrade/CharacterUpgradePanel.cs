using System.Collections.Generic;
using Code.Core.Events.Bus;
using Code.UnitSystem.Upgrade;
using GondrLib.Dependencies;
using GondrLib.ObjectPool.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class CharacterUpgradePanel : Panel
    {
        [Header("Tree Settings")]
        [SerializeField] private PoolingItemSO nodeButtonPoolSO;
        [SerializeField] private Transform treeContainer;
        [SerializeField] private List<UpgradeNodeSO> TreeData;

        [Header("Detail Settings")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI statInfoText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private Button upgradeButton;
        
        [Inject] private PoolManagerMono _poolManager;
        
        private List<UpgradeNodeButton> _activeNodes = new();
        private UpgradeNodeSO _selectedNode;

        public override void Awake()
        {
            base.Awake();
            
            if (_poolManager == null) 
                _poolManager = FindFirstObjectByType<PoolManagerMono>();
            
            upgradeButton.onClick.AddListener(HandleUpgradeClick);
        }

        private void OnDestroy()
        {
            upgradeButton.onClick.RemoveListener(HandleUpgradeClick);
        }

        public override void Open()
        {
            base.Open();
            RefreshTree();
            ClearDetailView();
        }

        private void RefreshTree()
        {
            foreach (var node in _activeNodes) node.ReturnToPool();
            _activeNodes.Clear();

            foreach (var data in TreeData)
            {
                var btn = _poolManager.Pop<UpgradeNodeButton>(nodeButtonPoolSO);
                btn.transform.SetParent(treeContainer);
                btn.transform.SetAsLastSibling();
                btn.transform.localScale = Vector3.one;
                btn.SetData(data, OnNodeSelected);
                
                _activeNodes.Add(btn);
            }
        }

        private void OnNodeSelected(UpgradeNodeSO nodeData)
        {
            _selectedNode = nodeData;

            if (iconImage != null)
            {
                iconImage.sprite = nodeData.icon;
                iconImage.color = Color.white;
                iconImage.gameObject.SetActive(true);
            }
            
            nameText.text = nodeData.upgradeName;
            descriptionText.text = nodeData.description;
            statInfoText.text = $"{nodeData.statOrSkillInfo}";
            costText.text = $"{nodeData.cost}";
            
            upgradeButton.interactable = !nodeData.isUnlocked; 
        }

        private void ClearDetailView()
        {
            _selectedNode = null;
            
            if (iconImage != null)
                iconImage.gameObject.SetActive(false);
            
            nameText.text = "업그레이드 선택";
            descriptionText.text = "위 트리에서 업그레이드 항목을 선택해주세요.";
            statInfoText.text = "-";
            costText.text = "-";
            upgradeButton.interactable = false;
        }

        private void HandleUpgradeClick()
        {
            if (_selectedNode == null) return;
            
            Bus<ShowMessageUIEvent>.Raise(new ShowMessageUIEvent($"[{_selectedNode.upgradeName}] 업그레이드 완료!"));
            
            _selectedNode.isUnlocked = true;
            
            RefreshTree(); 
            OnNodeSelected(_selectedNode); 
        }
    }
}