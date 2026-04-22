using Code.Expedition.Components;
using UnityEngine;
using _00.Core._02.Scripts._01.Manager;
using Code.Core.Events.Bus;
using System.Collections.Generic;
using Code.Core;
using UnityEngine.SceneManagement; 
using Code.Expedition.Data;

namespace Code.Expedition.Managers
{
    [System.Serializable]
    public struct EventUIMapping
    {
        public EventNodeSO eventNodeData;
        public GameObject uiPanel;
    }

    public class ExpeditionManager : MonoSingleton<ExpeditionManager>
    {
        [Header("References")]
        [SerializeField] private ExpeditionPlayer player;
        [SerializeField] private ExpeditionNode startNode;
        // inputReader 및 nodeLayer(3D 레이캐스트용) 제거됨

        [Header("Event UIs")]
        [SerializeField] private List<EventUIMapping> eventUIMappings; 

        private ExpeditionNode _currentNode;
        private ExpeditionNode _selectedNodeForMove; 
        private bool _isMoving;

        private static string _savedCurrentNodeName = "";
        private static readonly HashSet<string> _savedClearedNodes = new();

        private Canvas _canvas;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            InitializeExpeditionScene();
        }

        // 마우스 호버 등 매 프레임 검사하던 Update() 제거

        private void OnEnable()
        {
            Bus<StageClearEvent>.Subscribe(OnStageCleared);
            SceneManager.sceneLoaded += OnSceneLoaded; 
        }

        private void OnDisable()
        {
            Bus<StageClearEvent>.Unsubscribe(OnStageCleared);
            SceneManager.sceneLoaded -= OnSceneLoaded; 
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            InitializeExpeditionScene();
        }

        private void InitializeExpeditionScene()
        {
            ExpeditionNode[] allNodes = FindObjectsByType<ExpeditionNode>(FindObjectsSortMode.None);
            
            if (allNodes.Length == 0)
                return;
            
            if (player == null)
                player = FindAnyObjectByType<ExpeditionPlayer>();

            if (string.IsNullOrEmpty(_savedCurrentNodeName))
            {
                if (startNode != null)
                {
                    _currentNode = startNode;
                    _savedCurrentNodeName = startNode.name; 
                    _savedClearedNodes.Add(startNode.name); 
                }
            }
            else
            {
                foreach (var node in allNodes)
                    if (node.name == _savedCurrentNodeName)
                    {
                        _currentNode = node;
                        break;
                    }
            }

            foreach (var node in allNodes)
                if (_savedClearedNodes.Contains(node.name))
                    node.SetCleared(true);

            // 3D 위치(Transform) 대신 2D UI 위치(RectTransform)로 초기화
            if (_currentNode != null && player != null)
            {
                RectTransform nodeRect = _currentNode.GetComponent<RectTransform>();
                if (nodeRect != null)
                {
                    player.Initialize(nodeRect.anchoredPosition);
                }
            }
            
            UpdateAllNodesVisuals(allNodes);

            _selectedNodeForMove = null;
            _isMoving = false;
        }

        private void OnStageCleared(StageClearEvent evt)
        {
            if (evt.isClear)
            {
                if (!string.IsNullOrEmpty(_savedCurrentNodeName))
                {
                    _savedClearedNodes.Add(_savedCurrentNodeName);
                    Debug.Log($"[{_savedCurrentNodeName}] 노드가 클리어 기록에 추가되었습니다!");
                }

                if (_currentNode != null)
                {
                    _currentNode.SetCleared(true);
                    UpdateAllNodesVisuals(FindObjectsByType<ExpeditionNode>(FindObjectsSortMode.None));
                }
            }
        }

        private void UpdateAllNodesVisuals(ExpeditionNode[] allNodes)
        {
            if (allNodes == null)
                return;
            
            foreach (var node in allNodes)
                node.UpdateVisual(node == _currentNode);
        }

        // 노드 UI 버튼 클릭 시 호출되는 핵심 로직 (HandleClick 대체)
        public void OnNodeClicked(ExpeditionNode clickedNode)
        {
            if (_isMoving) return;

            // 이미 있는 노드를 다시 클릭하면 바로 씬/이벤트 진입
            if (clickedNode == _currentNode) 
            {
                EnterStage(_currentNode);
                return;
            }

            // Slay the Spire 규칙: 현재 노드와 연결된 노드로만 이동 가능
            if (_currentNode != null && !_currentNode.ConnectedNodes.Contains(clickedNode))
            {
                Debug.LogWarning("현재 노드에서 갈 수 없는 위치입니다.");
                return;
            }

            if (_selectedNodeForMove == clickedNode)
            {
                // 두 번째 클릭: 이동 실행
                clickedNode.SetOutline(false);
                TryMoveToNode(clickedNode);
                _selectedNodeForMove = null; 
            }
            else
            {
                // 첫 번째 클릭: 선택 및 하이라이트
                if (_selectedNodeForMove != null)
                {
                    _selectedNodeForMove.SetOutline(false);
                }

                _selectedNodeForMove = clickedNode;
                _selectedNodeForMove.SetOutline(true); 
                Debug.Log($"[{clickedNode.name}] 노드가 선택되었습니다. 한 번 더 클릭하면 이동합니다.");
            }
        }

        private void TryMoveToNode(ExpeditionNode targetNode)
        {
            if (_currentNode != null && !_currentNode.IsCleared)
            {
                Debug.LogWarning("현재 노드를 클리어해야 다음 노드로 이동할 수 있습니다!");
                return;
            }

            _isMoving = true;
            RectTransform targetRect = targetNode.GetComponent<RectTransform>();

            // UI 2D 공간(anchoredPosition) 기반으로 플레이어 이동
            player.MoveTo(targetRect.anchoredPosition, () =>
            {
                _isMoving = false;
                _currentNode = targetNode;
                
                _savedCurrentNodeName = _currentNode.name; 
                
                UpdateAllNodesVisuals(FindObjectsByType<ExpeditionNode>(FindObjectsSortMode.None)); 
                
                EnterStage(_currentNode);
            });
        }

        private void EnterStage(ExpeditionNode node)
        {
            if (node.NodeData != null && node.NodeData.nodeType == ExpeditionNodeType.Event)
            {
                GameObject targetUIPrefab = null;
                EventNodeSO currentEventData = node.NodeData as EventNodeSO;

                if (_canvas == null)
                {
                    Canvas[] canvas = FindObjectsOfType<Canvas>();
                    foreach (var canva in canvas)
                    {
                        if (canva.gameObject.name == "UI")
                        {
                            _canvas = canva;
                            break;
                        }
                    }
                }

                foreach (var mapping in eventUIMappings)
                {
                    if (mapping.eventNodeData == currentEventData)
                    {
                        targetUIPrefab = mapping.uiPanel;
                        break;
                    }
                }

                if (targetUIPrefab == null && eventUIMappings.Count > 0)
                {
                    targetUIPrefab = eventUIMappings[0].uiPanel;
                    Debug.Log("매칭되는 EventNodeSO가 없어 기본 UI를 사용합니다.");
                }

                if (targetUIPrefab != null && _canvas != null)
                {
                    GameObject uiInstance = Instantiate(targetUIPrefab, _canvas.transform);
                    // UI 환경이므로 로컬 포지션은 Canvas 설정에 맞게 조정이 필요할 수 있습니다.
                    uiInstance.transform.localPosition = new Vector3(0, 0, 0); 
                    uiInstance.SetActive(true);
                }
                else
                {
                    Debug.LogWarning("이벤트 UI 프리팹 또는 Canvas를 찾을 수 없습니다.");
                }

                _isMoving = false;
                return;
            }

            string targetSceneName = node.TargetSceneName;

            if (node.NodeData is BattleNodeSO battleNodeData)
                targetSceneName = battleNodeData.GetRandomBattleSceneName();

            if (string.IsNullOrEmpty(targetSceneName))
            {
                Debug.LogWarning($"[{node.name}] 노드에 이동할 씬 이름이 설정되지 않았습니다.");
                return;
            }

            if (SceneChangeManager.Instance != null)
                SceneChangeManager.Instance.ChangeSelectScene(targetSceneName);
            else
                SceneManager.LoadScene(targetSceneName);
        }
    }
}