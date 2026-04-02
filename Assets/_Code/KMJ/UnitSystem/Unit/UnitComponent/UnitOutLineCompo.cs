using System;
using EPOOutline;
using UnityEngine;

namespace Code.UnitSystem
{
    public class UnitOutLineCompo : MonoBehaviour, IUnitComponent
    {
        private Outlinable[] _outLines;

        public void Initialize(Unit owner)
        {
            _outLines = GetComponentsInChildren<Outlinable>();
        }
        private void Start()
        {
            ResetOutLine();
        }

        public void SetOutLine()
        {
            foreach (var outline in _outLines)
            {
                outline.enabled = true;
            }
        }

        public void ResetOutLine()
        {
            foreach (var outline in _outLines)
            {
                outline.enabled = false;
            }
        }

    }
}