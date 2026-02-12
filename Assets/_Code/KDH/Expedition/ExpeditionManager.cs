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

        private void HandleClick()
        {
            if (_isMoving) return;
            if (mainCamera == null) return;

            Ray ray = mainCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, nodeLayer))
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
                Debug.Log("이동할 수 없는 노드입니다 (연결되지 않음).");
            }
        }

        private void EnterStage(ExpeditionNode node)
        {
            if (string.IsNullOrEmpty(node.TargetSceneName))
            {
                Debug.LogWarning("이동할 씬 이름이 설정되지 않았습니다.");
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