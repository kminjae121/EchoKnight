using System;
using Code.Core.Events.Bus;
using Input;
using Unity.Cinemachine;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts.UnitSystem.Unit.UnitComponent
{
    public class SetUnitCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera unitCam;

        private GameObject ownCam;
        private CinemachineCamera OwnCamCompo;
        [SerializeField] private InputReader inputSO;

        private void Start()
        {
            inputSO.OnInteractionEvent += HandleCam;
            OwnCamCompo = ownCam.GetComponent<CinemachineCamera>();   
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

        public void SetThisUnit()
        {
           // Bus<CamMovingEvent>.Raise(new CamMovingEvent(unitCam.gameObject));
           // unitCam.Priority = 2;
        }

        public void EndThisUnit()
        {
//            Bus<CamMovingEvent>.Raise(new CamMovingEvent(ownCam.gameObject));
            //unitCam.Priority = -1;
        //   OwnCamCompo.Priority = 1;
        }
        
        private void HandleCam()
        {
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(this.gameObject, false,new Vector3(1.5f,1.5f,1.5f)));
        }
    }
}