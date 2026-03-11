using System;
using Code.Core.Events.Bus;
using Input;
using Unity.Cinemachine;
using UnityEngine;

namespace Code.AttackSystem
{
    public class SetUnitCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera unitCam;

        private GameObject ownCam;
        [SerializeField] private InputReader inputSO;

        private void Start()
        {
            inputSO.OnInteractionEvent += HandleCam;
        }

        private void OnEnable()
        {
            Bus<TopCamEvent>.Subscribe(HandleCamEvent);
        }

        private void HandleCamEvent(TopCamEvent obj)
        {
            ownCam = obj.cam;
        }

        private void OnDisable()
        {
            inputSO.OnInteractionEvent -= HandleCam;
        }
        
        private void HandleCam()
        {
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(this.gameObject, false,new Vector3(1.5f,1.5f,1.5f)));
        }
    }
}