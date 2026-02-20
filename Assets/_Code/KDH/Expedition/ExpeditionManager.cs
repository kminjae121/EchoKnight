using Code.Expedition.Components;
using UnityEngine;
using Input; 
using _00.Core._02.Scripts._01.Manager;
using Code.Core;
using Code.Core.Events.Bus; 

namespace Code.Expedition.Managers
{
    public class ExpeditionManager : MonoSingleton<ExpeditionManager>
    {
        [Header("References")]
        [SerializeField] private ExpeditionPlayer player;
        [SerializeField] private ExpeditionNode startNode;
        [SerializeField] private InputReader inputReader;
        [SerializeField] private LayerMask nodeLayer;
        
        [Header("Camera")]
        [SerializeField] private Camera mainCamera;

        private ExpeditionNode _currentNode;
        private ExpeditionNode _hoveredNode;
        private ExpeditionNode _selectedNodeForMove; 
        private bool _isMoving;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            
            if (mainCamera == null) mainCamera = Camera.main;
        }

        private void Start()
        {
            if (_currentNode == null && startNode != null)
            {
                _currentNode = startNode;
                _currentNode.SetCleared(true); 
            }

            if (_currentNode != null && player != null)
            {
                player.Initialize(_currentNode.transform.position);
            }
            
            UpdateAllNodesVisuals();
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
        }

        private void OnDisable()
        {
            if (inputReader != null)
            {
                inputReader.OnClickEvent -= HandleClick;
            }
            Bus<StageClearEvent>.Unsubscribe(OnStageCleared);
        }

        private void OnStageCleared(StageClearEvent evt)
        {
            if (evt.isClear && _currentNode != null)
            {
                _currentNode.SetCleared(true);
                Debug.Log($"[{_currentNode.name}] 스테이지가 클리어되었습니다!");
                UpdateAllNodesVisuals();
            }
        }

        private void UpdateAllNodesVisuals()
        {
            ExpeditionNode[] allNodes = FindObjectsByType<ExpeditionNode>(FindObjectsSortMode.None);
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
                Debug.LogWarning("현재 스테이지를 클리어해야 다음 노드로 이동할 수 있습니다!");
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
                    
                    UpdateAllNodesVisuals(); 
                    
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
            if (string.IsNullOrEmpty(node.TargetSceneName))
            {
                Debug.LogWarning($"[{node.name}] 노드에 이동할 씬 이름(TargetSceneName)이 설정되지 않았습니다.");
                return;
            }

            if (SceneChangeManager.Instance != null)
            {
                SceneChangeManager.Instance.ChangeSelectScene(node.TargetSceneName);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(node.TargetSceneName);
            }
        }
    }
}