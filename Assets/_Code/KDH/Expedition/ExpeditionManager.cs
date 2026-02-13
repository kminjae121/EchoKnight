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
            if (inputReader == null)
            {
                Debug.LogError("InputReader가 연결되지 않았습니다.");
                return;
            }

            Vector2 mousePos = inputReader.MousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePos);

            // [디버그] 클릭 시 씬 뷰에 빨간색 레이저를 2초간 표시합니다.
            Debug.DrawRay(ray.origin, ray.direction * 10000f, Color.red, 2f);
            
            // [디버그] 마우스 좌표 확인 (만약 (0,0)만 나온다면 Input System 설정 문제)
            // Debug.Log($"클릭 감지됨 - 마우스 위치: {mousePos}");

            // 거리 제한을 100f -> Mathf.Infinity(무제한)으로 변경
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, nodeLayer))
            {
                ExpeditionNode selectedNode = hit.collider.GetComponent<ExpeditionNode>();
                
                // [디버그] 감지된 오브젝트 이름 출력
                Debug.Log($"Raycast Hit: {hit.collider.name}");

                if (selectedNode != null)
                {
                    TryMoveToNode(selectedNode);
                }
            }
            else
            {
                // [디버그] 아무것도 맞지 않음
                // Debug.Log("Raycast 실패: 아무것도 맞지 않았습니다.");
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