using System;
using GameEventChannel;
using Unity.Cinemachine;
using UnityEngine;

namespace UnitSystem
{
    public class UnitCam : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO selectUnitEventChannel;
        [SerializeField] private CinemachineCamera unitCam;

        private Transform _currentUnitSight;

        private void Awake()
        {
            selectUnitEventChannel.AddListener<UnitSelectEvent>(HandleChangeUnitCam);
        }

        private void HandleChangeUnitCam(UnitSelectEvent evt)
        {
            _currentUnitSight = evt.Unit.transform;
            
            unitCam.Target.TrackingTarget =  _currentUnitSight;
        }
    }
}