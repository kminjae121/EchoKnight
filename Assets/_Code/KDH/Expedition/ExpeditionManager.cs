using Code.Expedition.Components;
using UnityEngine;
using Input; 
using _00.Core._02.Scripts._01.Manager;
using Code.Core;

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
            }

            if (_currentNode != null && player != null)
            {
                player.Initialize(_currentNode.transform.position);
            }
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
        }

        private void OnDisable()
        {
            if (inputReader != null)
            {
                inputReader.OnClickEvent -= HandleClick;
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
                    if (_hoveredNode != null) _hoveredNode.SetOutline(false);

                    _hoveredNode = hitNode;
                    if (_hoveredNode != null) _hoveredNode.SetOutline(true);
                }
            }
            else
            {
                if (_hoveredNode != null)
                {
                    _hoveredNode.SetOutline(false);
                    _hoveredNode = null;
                }
            }
        }

        private void HandleClick()
        {
            if (_isMoving) return;
            if (mainCamera == null) return;
            if (inputReader == null)
            {
                Debug.LogError("InputReader가 연결되지 않았습니다.");
                return;
            }

            Vector2 mousePos = inputReader.MousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            
            Debug.DrawRay(ray.origin, ray.direction * 10000f, Color.red, 2f);
            
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, nodeLayer))
            {
                ExpeditionNode selectedNode = hit.collider.GetComponent<ExpeditionNode>();
                
                if (selectedNode != null)
                {
                    TryMoveToNode(selectedNode);
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

            ExpeditionPath path = _currentNode.GetPathTo(targetNode);
            if (path != null)
            {
                _isMoving = true;
                player.MoveAlongPath(path.GetCurvePoints(_currentNode.transform.position), () =>
                {
                    _isMoving = false;
                    _currentNode = targetNode;
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