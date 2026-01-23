using System;
using Code.Core.Events.Bus;
using Unity.Cinemachine;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts.UnitSystem.Unit.UnitComponent
{
    public class SetUnitCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera unitCam;

        private GameObject ownCam;

        private void Start()
        {
            ownCam = GameObject.Find("TopCam").gameObject;
        }

        public void SetThisUnit()
        {
            Bus<CamMovingEvent>.Raise(new CamMovingEvent(unitCam.gameObject));
            unitCam.Priority = 2;
        }

        public void EndThisUnit()
        {
            Bus<CamMovingEvent>.Raise(new CamMovingEvent(ownCam.gameObject));
            unitCam.Priority = -1;
        }
    }
}