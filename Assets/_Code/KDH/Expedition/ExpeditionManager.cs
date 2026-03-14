using Code.Expedition.Components;
using UnityEngine;
using Input; 
using _00.Core._02.Scripts._01.Manager;
using Code.Core;
using Code.Core.Events.Bus;
using System.Collections.Generic;
using UnityEngine.SceneManagement; 
using Code.Expedition.Data;
using PixeLadder.EasyTransition; 

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
        [SerializeField] private InputReader inputReader;
        [SerializeField] private LayerMask nodeLayer;
        
        [Header("Camera")]
        [SerializeField] private Camera mainCamera;

        [Header("Event UIs")]
        [SerializeField] private List<EventUIMapping> eventUIMappings; 

        private ExpeditionNode _currentNode;
        private ExpeditionNode _hoveredNode;
        private ExpeditionNode _selectedNodeForMove; 
        private bool _isMoving;

        private static string _savedCurrentNodeName = "";
        private static HashSet<string> _savedClearedNodes = new HashSet<string>();

        private Canvas canvas = null; 

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            InitializeExpeditionScene();

            canvas = FindAnyObjectByType<Canvas>();
        }

        private void Update()
        {
            HandleHover();
        }

        private void OnEnable()
        {
            if (inputReader != null)
            {
                inputReader.OnClickEvent += HandleClick;
            }
            Bus<StageClearEvent>.Subscribe(OnStageCleared);
            SceneManager.sceneLoaded += OnSceneLoaded; 
        }

        private void OnDisable()
        {
            if (inputReader != null)
            {
                inputReader.OnClickEvent -= HandleClick;
            }
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
            
            if (allNodes.Length == 0) return;

            if (mainCamera == null) mainCamera = Camera.main;
            if (player == null) player = FindAnyObjectByType<ExpeditionPlayer>();

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
                {
                    if (node.name == _savedCurrentNodeName)
                    {
                        _currentNode = node;
                        break;
                    }
                }
            }

            foreach (var node in allNodes)
            {
                if (_savedClearedNodes.Contains(node.name))
                {
                    node.SetCleared(true);
                }
            }

            if (_currentNode != null && player != null)
            {
                player.Initialize(_currentNode.transform.position);
            }
            
            UpdateAllNodesVisuals(allNodes);

            _hoveredNode = null;
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
            if (allNodes == null) return;
            foreach (var node in allNodes)
            {
                node.UpdateMaterial(node == _currentNode);
            }
        }

        private void HandleHover()
        {
            if (mainCamera == null || inputReader == null) return;

            Ray ray = mainCamera.ScreenPointToRay(inputReader.MousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, nodeLayer))
            {
                ExpeditionNode hitNode = hit.collider.GetComponent<ExpeditionNode>();
                
                if (hitNode != _hoveredNode)
                {
                    if (_hoveredNode != null && _hoveredNode != _selectedNodeForMove) 
                        _hoveredNode.SetOutline(false);

                    _hoveredNode = hitNode;
                    if (_hoveredNode != null) _hoveredNode.SetOutline(true);
                }
            }
            else
            {
                if (_hoveredNode != null)
                {
                    if (_hoveredNode != _selectedNodeForMove)
                        _hoveredNode.SetOutline(false);
                    
                    _hoveredNode = null;
                }
            }
        }

        private void HandleClick()
        {
            if (_isMoving) return;
            if (mainCamera == null) return;
            if (inputReader == null) return;

            Vector2 mousePos = inputReader.MousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, nodeLayer))
            {
                ExpeditionNode hitNode = hit.collider.GetComponent<ExpeditionNode>();
                
                if (hitNode != null)
                {
                    if (_selectedNodeForMove == hitNode)
                    {
                        hitNode.SetOutline(false);
                        TryMoveToNode(hitNode);
                        _selectedNodeForMove = null; 
                    }
                    else
                    {
                        if (_selectedNodeForMove != null && _selectedNodeForMove != _hoveredNode)
                        {
                            _selectedNodeForMove.SetOutline(false);
                        }

                        _selectedNodeForMove = hitNode;
                        _selectedNodeForMove.SetOutline(true); 
                        Debug.Log($"[{hitNode.name}] 노드가 선택되었습니다. 한 번 더 클릭하면 이동합니다.");
                    }
                }
            }
            else
            {
                if (_selectedNodeForMove != null)
                {
                    if (_selectedNodeForMove != _hoveredNode)
                    {
                        _selectedNodeForMove.SetOutline(false);
                    }
                    
                    _selectedNodeForMove = null;
                    Debug.Log("노드 선택이 취소되었습니다.");
                }
            }
        }

        private void TryMoveToNode(ExpeditionNode targetNode)
        {
            if (targetNode == _currentNode) 
            {
                EnterStage(_currentNode);
                return;
            }

            if (_currentNode != null && !_currentNode.IsCleared)
            {
                Debug.LogWarning("현재 노드를 클리어해야 다음 노드로 이동할 수 있습니다!");
                return;
            }

            ExpeditionPath path = _currentNode.GetPathTo(targetNode);
            if (path != null)
            {
                _isMoving = true;
                player.MoveAlongPath(path.GetCurvePoints(_currentNode.transform.position), () =>
                {
                    _isMoving = false;
                    _currentNode = targetNode;
                    
                    _savedCurrentNodeName = _currentNode.name; 
                    
                    UpdateAllNodesVisuals(FindObjectsByType<ExpeditionNode>(FindObjectsSortMode.None)); 
                    
                    EnterStage(_currentNode);
                });
            }
            else
            {
                Debug.Log($"이동 불가: [{_currentNode.name}]에서 [{targetNode.name}]로 연결된 경로가 없습니다.");
            }
        }

        private void EnterStage(ExpeditionNode node)
        {
            if (node.NodeData != null && node.NodeData.nodeType == ExpeditionNodeType.Event)
            {
                if (SceneTransitioner.Instance != null)
                {
                    _isMoving = true; 
                    
                    GameObject targetUI = null;
                    EventNodeSO currentEventData = node.NodeData as EventNodeSO;

                    foreach (var mapping in eventUIMappings)
                    {
                        if (mapping.eventNodeData == currentEventData)
                        {
                            targetUI = mapping.uiPanel;
                            GameObject ui = Instantiate(eventUIMappings[0].uiPanel,canvas.transform);
                            ui.transform.localPosition = new Vector3(13, -82, -12f);
                            break;
                        }
                    }

                    if (targetUI == null && eventUIMappings.Count > 0)
                    {
                        targetUI = eventUIMappings[0].uiPanel;
                        GameObject ui = Instantiate(eventUIMappings[0].uiPanel,canvas.transform);
                        ui.transform.localPosition = new Vector3(13, -82, -12f);
                        
                        Debug.Log("매칭되는 EventNodeSO가 없어 기본 UI를 사용합니다.");
                    }

                    SceneTransitioner.Instance.DoTransition(
                        midTransitionAction: () => 
                        {
                            if (targetUI != null) targetUI.SetActive(true);
                        },
                        onCompleteAction: () =>
                        {
                            _isMoving = false;
                        }
                    );
                }
                else
                {
                    Debug.LogWarning("SceneTransitioner가 없습니다!");
                }
                return;
            }

            if (string.IsNullOrEmpty(node.TargetSceneName))
            {
                Debug.LogWarning($"[{node.name}] 노드에 이동할 씬 이름이 설정되지 않았습니다.");
                return;
            }

            if (SceneChangeManager.Instance != null)
            {
                SceneChangeManager.Instance.ChangeSelectScene(node.TargetSceneName);
            }
            else
            {
                SceneManager.LoadScene(node.TargetSceneName);
            }
        }
    }
}