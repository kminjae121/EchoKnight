using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    [CreateAssetMenu(fileName = "Input", menuName = "Input/InputReader", order = 0)]
    public class InputReader : ScriptableObject, Controls.IPlayerActions
    {
        private Controls _controls;
        
        [SerializeField] private LayerMask whatIsGround;
        
        public event Action OnAttackEvent;
        public event Action OnClickMoveEvent;
        
        private Vector3 _gridPosition;
        private Vector2 _screenPosition; 

        private void Awake()
        {
            if (_controls == null)
            {
                _controls = new Controls();
                _controls.Player.SetCallbacks(this);
            }
            _controls.Player.Enable();
        }

        private void OnDestroy()
        {
            _controls.Player.Disable();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnAttackEvent?.Invoke();
        }

        public void OnClickMove(InputAction.CallbackContext context)
        {
            if(context.performed)
                OnClickMoveEvent?.Invoke();
        }
        
        public Vector3 GetWorldPosition()
        {
            Camera mainCam = Camera.main;
            Debug.Assert(mainCam != null, "No main camera in this scene");
            
            Ray cameraRay = mainCam.ScreenPointToRay(_screenPosition);
            if (Physics.Raycast(cameraRay, out RaycastHit hit, mainCam.farClipPlane, whatIsGround))
            {
                _gridPosition = hit.point;
            }

            return _gridPosition;
        }
        public void OnPointer(InputAction.CallbackContext context)
        {
            _screenPosition = context.ReadValue<Vector2>();
        }
        
        public void SetActive(bool isActive)
        {
            if (isActive)
            {
                _controls.Player.Enable();
            }
            else
            {
                _controls.Player.Disable();
            }
        }
    }
}