using UnityEngine;
using Code.Core.Events.Bus;

namespace Code.Expedition
{
    public class ExpeditionCameraController : MonoBehaviour
    {
        [SerializeField] private float smoothTime = 0.3f;
        [SerializeField] private Vector3 offset = new Vector3(0, 0, -10f);

        private Camera _mainCamera;
        private Vector3 _targetPosition;
        private Vector3 _currentVelocity;
        private bool _isMoving = false;

        private void Awake()
        {
            _mainCamera = Camera.main;
            if (_mainCamera != null)
                _targetPosition = _mainCamera.transform.position;
            
            Bus<CamMovingEvent>.Subscribe(OnCamMoveRequest);
        }

        private void OnDestroy()
        {
            Bus<CamMovingEvent>.Unsubscribe(OnCamMoveRequest);
        }

        private void OnCamMoveRequest(CamMovingEvent evt)
        {
            if (evt.target != null)
            {
                SetTarget(evt.target.transform.position);
            }
        }

        public void SetTarget(Vector3 worldPosition)
        {
            _targetPosition = worldPosition + offset;
            _isMoving = true;
        }

        private void LateUpdate()
        {
            if (!_isMoving || _mainCamera == null) return;

            _mainCamera.transform.position = Vector3.SmoothDamp(
                _mainCamera.transform.position, 
                _targetPosition, 
                ref _currentVelocity, 
                smoothTime
            );

            if (Vector3.Distance(_mainCamera.transform.position, _targetPosition) < 0.01f)
            {
                _mainCamera.transform.position = _targetPosition;
                _isMoving = false;
            }
        }
    }
}