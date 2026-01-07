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

        private bool isCanSkill = true;

        private void Awake()
        {
            skillbtn.onClick.AddListener(HandleClickRange);
        }

        private void OnDestroy()
        {
            skillbtn.onClick.RemoveListener(HandleClickRange);
        }

        private void HandleClickRange()
        {
            if (isCanSkill)
            {
                skillCompnent.StartSkill(skillName);
                isCanSkill = false;
            }
            else
            {
                skillCompnent.CancelSkill(skillName);
                isCanSkill = true;
            }
                
        }
    }
}