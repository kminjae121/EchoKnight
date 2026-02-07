using System.Collections;
using System.Collections.Generic;
using _00.Core._02.Scripts._01.Manager;
using Code.Core;
using Code.Expedition.Data;
using Code.Expedition.Logic;
using Code.Core.Events.Bus;
using UnityEngine;

namespace Code.Expedition
{
    public class ExpeditionManager : MonoSingleton<ExpeditionManager>
    {
        [Header("Settings")]
        [SerializeField] private string mapSceneName = "ExpeditionMapScene"; 
        [SerializeField] private float nodeSelectionDelay = 1.5f;
        [SerializeField] private float battleReturnDelay = 3.0f;

        [Header("Components")]
        [SerializeField] private ExpeditionMapGenerator mapGenerator;

        private RuntimeExpeditionNode _currentNode;
        private Dictionary<ExpeditionNodeType, INodeLogic> _nodeLogics;

        public string MapSceneName => mapSceneName;

        protected override void Awake()
        {
            base.Awake();
            
            DontDestroyOnLoad(gameObject);

            if (mapGenerator == null)
                mapGenerator = GetComponent<ExpeditionMapGenerator>();

            InitializeLogics();
        }

        private void OnEnable()
        {
            Bus<StageClearEvent>.Subscribe(OnStageClear);
        }

        private void OnDisable()
        {
            Bus<StageClearEvent>.Unsubscribe(OnStageClear);
        }

        private void Start()
        {
            if (_currentNode == null)
            {
                StartNewExpedition();
            }
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

            StartCoroutine(ProcessNodeSelectionRoutine(nextNode));
        }

        private IEnumerator ProcessNodeSelectionRoutine(RuntimeExpeditionNode nextNode)
        {
            _currentNode = nextNode;
            _currentNode.SetVisited();

            Debug.Log($"노드 선택됨: [{_currentNode.Data.nodeName}]");
            
            Bus<ExpeditionNodeSelectEvent>.Raise(new ExpeditionNodeSelectEvent(_currentNode));

            yield return new WaitForSeconds(nodeSelectionDelay);

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

        private void OnStageClear(StageClearEvent evt)
        {
            Debug.Log("전투 종료. 결과와 무관하게 맵으로 복귀합니다.");
            StartCoroutine(ProcessBattleReturnRoutine());
        }

        private IEnumerator ProcessBattleReturnRoutine()
        {
            yield return new WaitForSeconds(battleReturnDelay);
            
            CompleteCurrentNode();

            if (SceneChangeManager.Instance != null)
            {
                SceneChangeManager.Instance.ChangeSelectScene(mapSceneName);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(mapSceneName);
            }
        }
    }
}