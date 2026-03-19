using Code.Core.Events.Bus;
using Input;
using Unity.Cinemachine;
using UnityEngine;

namespace Code.Cam
{
    public class BattleCam : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;
        [SerializeField] private CinemachineCamera battleCam;
        [SerializeField] private float moveSpeed;
        [SerializeField] private CinemachinePositionComposer positionComposer;

        [Header("Move Limit (World XZ AABB)")]
        [SerializeField] private bool useMoveLimit = true;
        [SerializeField] private float minX;
        [SerializeField] private float maxX;
        [SerializeField] private float minZ;
        [SerializeField] private float maxZ;

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
            
            if (positionComposer == null)
                positionComposer = GetComponent<CinemachinePositionComposer>();

            minX = transform.position.x - 50;
            maxX = transform.position.x + 50;
            minZ = transform.position.z - 50;
            maxZ = transform.position.z + 50;
        }

        private void Start()
        {
            battleCam.Lens.NearClipPlane = -15;
            battleCam.Target.TrackingTarget = null;

            if (positionComposer != null)
                positionComposer.enabled = false;

            Bus<TopCamEvent>.Raise(new TopCamEvent(gameObject));
        }

        private void OnDisable()
        {
            Bus<UnitCamSettingEvent>.Unsubscribe(SetTarget);
        }

        public void SetTarget(UnitCamSettingEvent evt)
        {
            _isLocking = evt.isLocking;

            if (evt.target == null)
            {
                if (positionComposer != null && positionComposer.enabled)
                    positionComposer.enabled = false;

                battleCam.Target.TrackingTarget = null;
            }
            else
            {
                if (positionComposer != null)
                {
                    positionComposer.enabled = true;
                    positionComposer.Damping = evt.dampingSpeed;
                }

                battleCam.Target.TrackingTarget = evt.target.transform;
            }
        }

        private void Update()
        {
            Vector3 camForward = transform.forward;
            Vector3 camRight = transform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDir = (camRight * inputReader.MovementKey.x + camForward * inputReader.MovementKey.y);

            if (moveDir.sqrMagnitude > 1f)
                moveDir.Normalize();

       
            if (inputReader.MouseUpDownValue.y > 0 && battleCam.Lens.OrthographicSize >= 10)
                battleCam.Lens.OrthographicSize -= 100 * Time.deltaTime;
            else if (inputReader.MouseUpDownValue.y < 0 && battleCam.Lens.OrthographicSize <= 35)
                battleCam.Lens.OrthographicSize += 100 * Time.deltaTime;

            if (UnityEngine.Input.GetKeyDown(KeyCode.LeftShift))
                moveSpeed = _reduceSpeed;

            if (UnityEngine.Input.GetKeyUp(KeyCode.LeftShift))
                moveSpeed = _basicSpeed;

    
            if ((inputReader.MovementKey.x != 0 || inputReader.MovementKey.y != 0) && !_isLocking)
            {
                battleCam.Target.TrackingTarget = null;

                _movement = moveDir * moveSpeed;
                
                Vector3 newPos = transform.position + (_movement * Time.deltaTime);

                if (useMoveLimit)
                {
                    newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
                    newPos.z = Mathf.Clamp(newPos.z, minZ, maxZ);
                }

                transform.position = newPos;
            }
        }
        
    }
}