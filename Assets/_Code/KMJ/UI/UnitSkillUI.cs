using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Events.Bus;
using Code.UnitSystem.SkillSystem;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class UnitSkillUI : MonoBehaviour
    {
        [SerializeField] private SkillComponent skillCompnent;

        [SerializeField] private GameObject skillUI;

        [SerializeField] private Sprite basicSprite;

        [SerializeField] private List<Image> skillImage;

        [SerializeField] private List<Button> skillbtn;

        [SerializeField] private List<string> thisSkillName;
        
        private void Awake()
        {
            Bus<SkillUIEvent>.Subscribe(HandleSkillUIEvent);
        }

        private void OnDestroy()
        {
            Bus<SkillUIEvent>.Unsubscribe(HandleSkillUIEvent);
            
            skillbtn.ToList().ForEach(btn =>
                btn.onClick.RemoveAllListeners());
        }

        private void HandleClickRange(int idx)
        { 
            skillCompnent.CancelAllSkill();
            
            skillCompnent.StartSkill(thisSkillName[idx]);
        }

        public void CancelSkill()
        {
            if (skillCompnent != null)
            {
                skillCompnent.CancelAllSkill();
            }
            
            Bus<UsingSkillEvent>.Raise(new UsingSkillEvent(true));
        }
        
        private void HandleSkillUIEvent(SkillUIEvent evt)
        {
            skillCompnent = evt.skillComponent;
            thisSkillName[evt.skillIdx] = evt.skillName;
            
            
            skillbtn[evt.skillIdx].onClick.RemoveAllListeners();
            
            int capturedIdx = evt.skillIdx;

            if (evt.skillName != null)
            {
                skillbtn[capturedIdx].onClick.AddListener(() => HandleClickRange(capturedIdx));
            }
        }
    }
}