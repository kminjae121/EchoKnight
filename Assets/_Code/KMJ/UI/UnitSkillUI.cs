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
        private SkillComponent skillCompnent;

        [SerializeField] private Sprite basicSprite;

        [SerializeField] private List<Image> skillImage;

        [SerializeField] private List<Button> skillbtn;

        [SerializeField] private List<string> thisSkillName;

        [SerializeField] private List<bool> isCanSkill;
        
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
            if (isCanSkill[idx])
            {
                skillCompnent.StartSkill(thisSkillName[idx]);
                isCanSkill[idx] = false;
            }
            else
            {
                skillCompnent.CancelSkill(thisSkillName[idx]);
                isCanSkill[idx] = true;
            }
        }

        private void HandleSkillUIEvent(SkillUIEvent evt)
        {
            skillCompnent = evt.skillComponent;
            if (evt.skillImage == null)
            {
                skillImage[evt.skillIdx].sprite = basicSprite;
            }
            else
            {
                skillImage[evt.skillIdx].sprite = evt.skillImage;
            }
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