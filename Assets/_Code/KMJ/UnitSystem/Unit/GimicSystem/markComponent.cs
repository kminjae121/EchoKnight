using System;
using Code.Core.Events.Bus;
using UnityEngine;

namespace Code.UnitSystem.GimicSystem
{
    public class markComponent : MonoBehaviour
    {
        public bool isMarking { get; private set; } = false;

        private void Start()
        {
            Bus<SetMarkEvent>.Subscribe(SetMark);
        }

        private void OnDisable()
        {
            Bus<SetMarkEvent>.Unsubscribe(SetMark);
        }

        private void SetMark(SetMarkEvent evt)
        {
            isMarking = true;
        }
    }
}