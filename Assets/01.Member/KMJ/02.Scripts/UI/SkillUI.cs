using System;
using Code.UnitSystem.SkillSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class SkillUI : MonoBehaviour
    {
        [SerializeField] private SkillComponent skillCompnent;

        [SerializeField] private string skillName;

        [SerializeField] private Button skillbtn;

        private void Awake()
        {
            skillbtn.onClick.AddListener(HandleClickRange);
        }

        private void HandleClickRange()
        {
            skillCompnent.StartSkill(skillName);
        }
    }
}