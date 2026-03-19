using System;
using Code.Core.Events.Bus;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UnitSystem.GimicSystem
{
    public class markComponent : MonoBehaviour
    {
        [SerializeField] private Image markUI;
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
            markUI.gameObject.SetActive(true);
        }
    }
}