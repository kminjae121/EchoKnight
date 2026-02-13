using Code.Core.Events.Bus;
using Input;
using Unity.Cinemachine;
using UnityEngine;

namespace _Code.KMJ.Cam
{
    public class BattleCam : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;
        [SerializeField] private CinemachineCamera battleCam;
        [SerializeField] private float moveSpeed;
        [SerializeField] private CinemachinePositionComposer positionComposer;

        private float _basicSpeed;
        private float _reduceSpeed;
        
        private bool _isLocking;
        
        private Vector3 _movement;

        private void Awake()
        {
            _movement = Vector3.zero;
            
            Bus<UnitCamSettingEvent>.Subscribe(SetTarget);

            _basicSpeed = moveSpeed;
            _reduceSpeed = moveSpeed / 2;
        }

        private void Start()
        {
            battleCam.Target.TrackingTarget = null;
            positionComposer.enabled = false;
        }

        public void SetTarget(UnitCamSettingEvent evt)
        {
            _isLocking = evt.isLocking;
            
            if (evt.target == null)
            {
                positionComposer.enabled = false;
                battleCam.Target.TrackingTarget = null;
            }
            else
            {
                positionComposer.enabled = true;
                positionComposer.Damping = evt.dampingSpeed;
                battleCam.Target.TrackingTarget = evt.target.transform;
            }
        }
        
        private void Update()
        {
            Vector3 camForward = transform.forward;
            Vector3 camRight   = transform.right;

            camForward.y = 0f;
            camRight.y   = 0f;
            
            camForward.Normalize();
            camRight.Normalize();
            
            Vector3 moveDir = (camRight * inputReader.MovementKey.x + camForward * inputReader.MovementKey.y);

            if (moveDir.sqrMagnitude > 1f)
                moveDir.Normalize();

            if (inputReader.MouseUpDownValue.y > 0 && battleCam.Lens.FieldOfView <= 70)
                battleCam.Lens.FieldOfView += 100 * Time.deltaTime;
            else if(inputReader.MouseUpDownValue.y < 0 && battleCam.Lens.FieldOfView >= 20)
                battleCam.Lens.FieldOfView -= 100 * Time.deltaTime;

            if (UnityEngine.Input.GetKeyDown(KeyCode.LeftShift))
                moveSpeed = _reduceSpeed;
            
            if (UnityEngine.Input.GetKeyUp(KeyCode.LeftShift))
                moveSpeed = _basicSpeed;
            
            if (inputReader.MovementKey.x != 0 || inputReader.MovementKey.y != 0)
                if (!_isLocking)
                {
                    battleCam.Target.TrackingTarget = null;
                    
                    _movement = moveDir * moveSpeed;

                    transform.position += _movement * Time.deltaTime;
                }
        }
    }
}