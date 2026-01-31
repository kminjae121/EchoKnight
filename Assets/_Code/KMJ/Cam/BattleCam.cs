using System;
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
        
        private Vector3 movement;
        [SerializeField] private float moveSpeed;

        private bool isLocking = false;

        [SerializeField] private CinemachinePositionComposer positionCompoer;

        private void Awake()
        {
            movement = Vector3.zero;
            
            Bus<UnitCamSettingEvent>.Subscribe(SetTarget);
        }

        private void Start()
        {
            battleCam.Target.TrackingTarget = null;
            positionCompoer.enabled = false;
        }

        public void SetTarget(UnitCamSettingEvent evt)
        {
            isLocking = evt.isLocking;
            if (evt.target == null)
            {
                positionCompoer.enabled = false;
                battleCam.Target.TrackingTarget = null;
            }
            else
            {
                positionCompoer.enabled = true;
                positionCompoer.Damping = new Vector3(0.1f, 0.1f, 0.1f);
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

            if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

            if (inputReader.MouseUpDownValue.y > 0 && battleCam.Lens.FieldOfView <= 85)
            {
                battleCam.Lens.FieldOfView += 100 * Time.deltaTime;
            }
            else if(inputReader.MouseUpDownValue.y < 0 && battleCam.Lens.FieldOfView >= 45)
            {
                battleCam.Lens.FieldOfView -= 100 * Time.deltaTime;
            }

            if (inputReader.MovementKey.x != 0 || inputReader.MovementKey.y != 0)
            {
                if (!isLocking)
                {
                    battleCam.Target.TrackingTarget = null;
                    
                    movement = moveDir * moveSpeed;

                    transform.position += movement * Time.deltaTime;
                }
            }
        }
    }
}