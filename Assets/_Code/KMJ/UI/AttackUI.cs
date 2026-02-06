using System;
using System.Collections.Generic;
using Code.Core.Events.Bus;
using TMPro;
using UnityEngine;

namespace Code.UI
{
    public class AttackUI : MonoBehaviour
    {
        [SerializeField] private GameObject atkUI;
        [SerializeField] private UnitSkillUI skillUI;
        [SerializeField] private List<TextMeshProUGUI> skillsName;

        [SerializeField] private RectTransform selectArrow;
        
        [SerializeField] private List<RectTransform> items;

        private float _xValue = 0;

        private int _itemIdx = 0;

        private void Awake()
        {
            Bus<SetAtkUIEvent>.Subscribe(SetAtkUI);
            Bus<SkillUIEvent>.Subscribe(SetSkillUIName);
        }


        private void OnDestroy()
        {
            Bus<SkillUIEvent>.Unsubscribe(SetSkillUIName);
            Bus<SetAtkUIEvent>.Unsubscribe(SetAtkUI);
        }

        private void Update()
        {
            if (UnityEngine.Input.GetMouseButton(1))
            {
                atkUI.SetActive(true);
                selectArrow.gameObject.SetActive(true);
                selectArrow.transform.rotation = Quaternion.Euler(0,0,90);
                _itemIdx = 0;
                _xValue = 0;
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                Bus<UnitSetMoveEvent>.Raise(new UnitSetMoveEvent(true));
                skillUI.CancelSkill();
                Bus<UnitAttackEvent>.Raise(new UnitAttackEvent(false));
                atkUI.SetActive(false);
                selectArrow.gameObject.SetActive(false);
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow))
            {
                _itemIdx += 1;
                if (_itemIdx >= 4)
                {
                    _itemIdx = 0;
                }
                
                Vector3 pos = items[_itemIdx].localPosition;
                pos.x = 80;
                pos.z = 0;
                selectArrow.localPosition = pos;
            }
            
            selectArrow.transform.rotation = Quaternion.Euler(_xValue += 1f,0,90);
        }

        public void SetSkillUIName(SkillUIEvent evt)
        {
            skillsName[evt.skillIdx].text = evt.skillName;
        }

        public void SetAtkUI(SetAtkUIEvent evt)
        {
            atkUI.SetActive(false);
            selectArrow.gameObject.SetActive(false);
        }
    }
}