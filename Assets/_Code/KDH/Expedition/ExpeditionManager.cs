using System.Collections.Generic;
using Code.Expedition.Data;
using Code.Expedition.Logic;
using Code.Core;
using Code.Core.Events.Bus;
using UnityEngine;

namespace Code.Expedition
{
    public class ExpeditionManager : MonoSingleton<ExpeditionManager>
    {
        [SerializeField] private ExpeditionMapGenerator mapGenerator;
        [SerializeField] private string mapSceneName = "ExpeditionMapScene"; 

        private RuntimeExpeditionNode _currentNode;
        private Dictionary<ExpeditionNodeType, INodeLogic> _nodeLogics;

        public string MapSceneName => mapSceneName;

        protected override void Awake()
        {
            base.Awake();
            if (mapGenerator == null)
                mapGenerator = GetComponent<ExpeditionMapGenerator>();

            InitializeLogics();
        }

        [ContextMenu("Force Start Expedition")]
        public void ForceStartExpedition()
        {
            Debug.Log("강제로 원정을 시작합니다...");
            StartNewExpedition();
        }
        
        private void InitializeLogics()
        {
            _nodeLogics = new Dictionary<ExpeditionNodeType, INodeLogic>
            {
                { ExpeditionNodeType.Battle, new BattleNodeLogic() },
                { ExpeditionNodeType.EliteBattle, new BattleNodeLogic() },
                { ExpeditionNodeType.Boss, new BattleNodeLogic() },
                { ExpeditionNodeType.Event, new EventNodeLogic() }
            };
        }

        public void StartNewExpedition()
        {
            _currentNode = mapGenerator.GenerateMap();
            
            if (_currentNode != null)
            {
                _currentNode.SetVisited();
                _currentNode.SetLocked(false);
                CompleteCurrentNode(); 
                
                Debug.Log($"원정 시작: 시작 노드 [{_currentNode.Data.nodeName}]");
                Bus<ExpeditionNodeArriveEvent>.Raise(new ExpeditionNodeArriveEvent(_currentNode));
            }
        }

        public void SelectNode(RuntimeExpeditionNode nextNode)
        {
            if (nextNode.IsLocked) return;
            if (!_currentNode.NextNodes.Contains(nextNode)) return;

            _currentNode = nextNode;
            _currentNode.SetVisited();

            Debug.Log($"노드 선택됨: [{_currentNode.Data.nodeName}]");
            Bus<ExpeditionNodeSelectEvent>.Raise(new ExpeditionNodeSelectEvent(_currentNode));
            
            if (_nodeLogics.TryGetValue(_currentNode.Data.nodeType, out INodeLogic logic))
            {
                logic.Execute(_currentNode);
            }
        }

        public void CompleteCurrentNode()
        {
            if (_currentNode == null) return;
            
            UnlockNextNodes(_currentNode);
            Bus<ExpeditionNodeArriveEvent>.Raise(new ExpeditionNodeArriveEvent(_currentNode));
        }

        private void UnlockNextNodes(RuntimeExpeditionNode node)
        {
            foreach (var next in node.NextNodes)
            {
                next.SetLocked(false);
            }
        }
        
        public RuntimeExpeditionNode GetCurrentNode() => _currentNode;

        public void FailExpedition()
        {
            Debug.Log("원정 실패. 데이터를 초기화합니다.");
            _currentNode = null;
            
            if (BattleContext.Instance != null)
                BattleContext.Instance.ClearContext();
        }
    }
}