using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class UnitsHpBar : MonoBehaviour
    {
        [SerializeField] private List<Slider> healthSliders;

        [SerializeField] private List<Sprite> unitCharacterImages;

        private void Awake()
        {
            
        }
    }
}