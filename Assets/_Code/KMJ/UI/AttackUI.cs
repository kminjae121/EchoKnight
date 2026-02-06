using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
                StartAttack();
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                EndAttack();
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow))
            {
                DownItem();
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow))
            {
                UpItem();
            }
            
            selectArrow.transform.rotation = Quaternion.Euler(_xValue += 1f,0,90);
        }

        private void StartAttack()
        {
            atkUI.SetActive(true);
            selectArrow.gameObject.SetActive(true);
            selectArrow.transform.rotation = Quaternion.Euler(0,0,90);
            _itemIdx = 0;
            _xValue = 0;
        }

        private void EndAttack()
        {
            Bus<UnitSetMoveEvent>.Raise(new UnitSetMoveEvent(true));
            skillUI.CancelSkill();
            Bus<UnitAttackEvent>.Raise(new UnitAttackEvent(false));
            atkUI.SetActive(false);
            selectArrow.gameObject.SetActive(false);
        }

        private void UpItem()
        {
            _itemIdx -= 1;
            if (_itemIdx <= -1)
            {
                _itemIdx = 3;
            }
            if (_itemIdx >= 1)
            {
                for (int i = _itemIdx - 1; i >= 0; i--)
                {
                    Debug.Log(i);
                    Debug.Log(skillsName[i].text);
                    if (skillsName[i].text == null)
                    {
                        _itemIdx -= 1;
                    }
                    else
                        break;
                }
            }
            

            
            Debug.Log(_itemIdx);

            Vector3 pos = items[_itemIdx].localPosition;
            pos.x = 80;
            pos.z = 0;
            selectArrow.localPosition = pos;
        }

        private void DownItem()
        {
            _itemIdx += 1;
            if (_itemIdx >= 4)
            {
                _itemIdx = 0;
            }
            if (_itemIdx >= 1)
            {
                if (skillsName[_itemIdx - 1].text == null)
                {
                    Debug.Log(skillsName[_itemIdx -1].text);
                    _itemIdx = 0;
                }
            }
                
            Vector3 pos = items[_itemIdx].localPosition;
            pos.x = 80;
            pos.z = 0;
            selectArrow.localPosition = pos;
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