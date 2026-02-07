using System.Collections.Generic;
using System.Linq;
using Code.Core.Events.Bus;
using Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class AttackUI : MonoBehaviour
    {
        [SerializeField] private GameObject atkUI;
        [SerializeField] private UnitSkillUI skillUI;
        [SerializeField] private List<TextMeshProUGUI> skillsName;

        [SerializeField] private RectTransform selectArrow;
        
        [SerializeField] private List<RectTransform> items;

        [SerializeField] private List<Button> itemBtns;

        [SerializeField] private List<TextMeshProUGUI> explainTxt;
        
        [SerializeField] private List<GameObject> selectItem;

        [SerializeField] private InputReader inputSO;

        private bool _isCanOpen = true;

        private bool _isActive = false;

        private float _xValue = 0;

        private int _itemIdx = 0;

        private void Awake()
        {
            Bus<SetAtkUIEvent>.Subscribe(SetAtkUI);
            Bus<SkillUIEvent>.Subscribe(SetSkillUIName);
            inputSO.OnSelectEvent += SelectItem;
            _isActive = false;
        }


        private void OnDestroy()
        {
            Bus<SkillUIEvent>.Unsubscribe(SetSkillUIName);
            Bus<SetAtkUIEvent>.Unsubscribe(SetAtkUI);
            inputSO.OnSelectEvent -= SelectItem;
        }

        private void Update()
        {
            
            if (UnityEngine.Input.GetKeyDown(KeyCode.R) && !_isActive && _isCanOpen)
            {
                StartAttack();
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.R) && _isActive)
            {
                EndAttack();
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.E))
            {
                DownItem();
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.Q))
            {
                UpItem();
            }
            
            selectArrow.transform.rotation = Quaternion.Euler(_xValue += 1f,0,90);
        }

        private void SelectItem()
        {
            if (_isActive == true)
            {
                itemBtns[_itemIdx].onClick?.Invoke();
            }
        }

        private void StartAttack()
        {
            InitializeUI();
            _itemIdx = 0;
            _xValue = 0;
            atkUI.SetActive(true);
            selectArrow.gameObject.SetActive(true);
            _isActive = true;
            
            
            skillsName.ToList().ForEach(txt =>
            {
                txt.color = Color.white;
            });
            
            Image img = selectItem[_itemIdx].GetComponent<Image>();
            
            skillsName[_itemIdx].color = img.color;
            selectArrow.GetComponent<Image>().color = img.color;
            
            selectItem.ToList().ForEach(obj =>
            {
                obj.SetActive(false);
            });
            explainTxt.ToList().ForEach(txt =>
            {
                txt.gameObject.SetActive(false);
            });
            explainTxt[_itemIdx].gameObject.SetActive(true);
            
            selectItem[_itemIdx].SetActive(true);
        }

        private void EndAttack()
        {
            Bus<UnitSetMoveEvent>.Raise(new UnitSetMoveEvent(true));
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(true));
            skillUI.CancelSkill();
            Bus<UnitAttackEvent>.Raise(new UnitAttackEvent(false));
            selectArrow.transform.rotation = Quaternion.Euler(0,0,90);
            InitializeUI();
        }

        private void InitializeUI()
        {
            Vector3 pos = new Vector3(80, 186.5f, 0);
            selectArrow.anchoredPosition = pos;
            _itemIdx = 0;
            _xValue = 0;
            atkUI.SetActive(false);
            selectArrow.gameObject.SetActive(false);
            _isActive = false;
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
                for (int i = _itemIdx; i >= 0; i--)
                {
                    if (skillsName[i].text == null)
                    {
                        _itemIdx -= 1;
                    }
                    else
                        break;
                }
            }

            SetAtkUI();
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
                if (skillsName[_itemIdx].text == null)
                {
                    _itemIdx = 0;
                }
            }
                
            SetAtkUI();
        }

        private void SetAtkUI()
        {
            Vector3 pos = items[_itemIdx].localPosition;
            pos.x = 80;
            pos.z = 0;
            selectArrow.localPosition = pos;
            
            skillsName.ToList().ForEach(txt =>
            {
                txt.color = Color.white;
            });
            
            
            skillsName.ToList().ForEach(txt =>
            {
                txt.color = Color.white;
            });
            
            Image img = selectItem[_itemIdx].GetComponent<Image>();
            
            skillsName[_itemIdx].color = img.color;
            selectArrow.GetComponent<Image>().color = img.color;
            
            explainTxt.ToList().ForEach(txt =>
            {
                txt.gameObject.SetActive(false);
            });
            explainTxt[_itemIdx].gameObject.SetActive(true);
            
            selectItem.ToList().ForEach(obj =>
            {
                obj.SetActive(false);
            });
            
            selectItem[_itemIdx].SetActive(true);
        }

        public void SetSkillUIName(SkillUIEvent evt)
        {
            skillsName[evt.skillIdx + 1].text = evt.skillName;
            explainTxt[evt.skillIdx + 1].text = $"코스트 - {evt.skillCost}";
        }

        public void SetAtkUI(SetAtkUIEvent evt)
        {
            if (evt.isLock)
            {
                _isCanOpen = false;
            }
            else if(evt.isLock == false)
            {
                _isCanOpen = true;
            }
            
            InitializeUI();
            atkUI.SetActive(false);
            selectArrow.gameObject.SetActive(false);
        }
    }
}