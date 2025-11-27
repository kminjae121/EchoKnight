using System;
using EntityComponent;
using UnitSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    [CreateAssetMenu(fileName = "Input", menuName = "Input/InputReader", order = 0)]
    public class InputReader : ScriptableObject, Controls.IPlayerActions
    {
        
        [SerializeField] private LayerMask whatIsGround;
        
        [SerializeField] private LayerMask WhatIsEnemy;
        
        [SerializeField] private LayerMask WhatIsPlayer;
        
        private Controls _controls;
        public event Action OnAttackEvent;
        public event Action OnClickMoveEvent;

        public event Action OnSelectUnitEvent;
        
        private Vector3 _gridPosition;
        private Vector2 _screenPosition;


        private void OnEnable()
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

        public Unit GetUnit()
        {
            Camera mainCam = Camera.main;
            Debug.Assert(mainCam != null, "No main camera in this scene");
            
            Ray cameraRay = mainCam.ScreenPointToRay(_screenPosition);
            
            if (Physics.Raycast(cameraRay, out RaycastHit hit, mainCam.farClipPlane, WhatIsPlayer))
            {
                return hit.collider.gameObject.GetComponent<Unit>();
            }
            return null;
        }

        public Entity GetEnemy()
        {
            Camera mainCam = Camera.main;
            Debug.Assert(mainCam != null, "No main camera in this scene");
            
            Ray cameraRay = mainCam.ScreenPointToRay(_screenPosition);
            
            if (Physics.Raycast(cameraRay, out RaycastHit hit, mainCam.farClipPlane, WhatIsEnemy))
            {
                return hit.collider.gameObject.GetComponent<Entity>();
            }
            return null;
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            if (GetUnit() != null)
            {
                OnSelectUnitEvent?.Invoke();
            }
            else if (GetEnemy() != null)
            {
                OnAttackEvent?.Invoke();
            }
            else
            {
                OnClickMoveEvent?.Invoke();
            }
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