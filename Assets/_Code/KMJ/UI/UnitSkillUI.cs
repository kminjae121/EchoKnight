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

        [SerializeField] private GameObject skillUI;

        [SerializeField] private Sprite basicSprite;

        [SerializeField] private List<Image> skillImage;

        [SerializeField] private List<Button> skillbtn;

        [SerializeField] private List<string> thisSkillName;

        [SerializeField] private List<bool> isCanSkill;
        
        private void Awake()
        {
            Bus<SkillUIEvent>.Subscribe(HandleSkillUIEvent);
            
            Bus<UnitSkilStartEvent>.Subscribe(HandleSkillBool);
            Bus<UnitSkilStartEvent>.Subscribe(HandleSkillUIObject);
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
                Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(true));
                skillCompnent.CancelAllSkill();
                
                for (int i = 0; i < isCanSkill.Count; i++)
                {
                    isCanSkill[i] = true;
                }

                isCanSkill[idx] = false;
                
                skillCompnent.StartSkill(thisSkillName[idx]);

            }
            else
            {
                Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false));
                
                skillCompnent.CancelAllSkill();
                Bus<UsingSkillEvent>.Raise(new UsingSkillEvent(true));
                
                for (int i = 0; i < isCanSkill.Count; i++)
                {
                    isCanSkill[i] = true;
                }
            }
        }

        private void HandleSkillBool(UnitSkilStartEvent evt)
        {
            if (evt.isStart == true)
            {
                for (int i = 0; i < isCanSkill.Count; i++)
                {
                    isCanSkill[i] = true;
                } 
            }
        }

        private void HandleSkillUIObject(UnitSkilStartEvent evt)
        {
            if (evt.isStart == true)
            {
                skillUI.SetActive(false);
            }
            else
            {
                skillUI.SetActive(true);
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
            
            isCanSkill[evt.skillIdx] = true;
        }
    }
}