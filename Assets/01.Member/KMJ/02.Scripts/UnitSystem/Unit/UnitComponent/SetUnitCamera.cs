using Unity.Cinemachine;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts.UnitSystem.Unit.UnitComponent
{
    public class SetUnitCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera unitCam;

        public void SetThisUnit()
        {
            unitCam.Priority = 2;
        }

        public void EndThisUnit()
        {
            unitCam.Priority = -1;
        }
    }
}