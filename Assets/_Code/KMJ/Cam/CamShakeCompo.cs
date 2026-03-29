using System;
using Code.Core.Events.Bus;
using Unity.Cinemachine;
using UnityEngine;

namespace Code.Cam
{
    public class CamShakeCompo : MonoBehaviour
    {
        [SerializeField] private CinemachineImpulseSource source;

        private void Awake()
        {
            Bus<CamShakeEvent>.Subscribe(ShakeCam);
        }

        private void OnDestroy()
        {
            Bus<CamShakeEvent>.Unsubscribe(ShakeCam);
        }

        private void ShakeCam(CamShakeEvent evt)
        {
            source.GenerateImpulse(evt.force);  
        }
    }
}