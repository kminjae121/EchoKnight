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
        private bool _isActive;
        private float _xValue;
        private int _itemIdx;

        private Image _arrowImage;

        private int SlotCount => items?.Count ?? 0;

        private void Awake()
        {
            _arrowImage = selectArrow != null ? selectArrow.GetComponent<Image>() : null;
            _isActive = false;
        }

        private void OnEnable()
        {
            Bus<SetAtkUIEvent>.Subscribe(SetAtkUI);
            Bus<SkillUIEvent>.Subscribe(SetSkillUIName);

            if (inputSO != null)
                inputSO.OnSelectEvent += SelectItem;
        }

        private void OnDisable()
        {
            Bus<SkillUIEvent>.Unsubscribe(SetSkillUIName);
            Bus<SetAtkUIEvent>.Unsubscribe(SetAtkUI);

            if (inputSO != null)
                inputSO.OnSelectEvent -= SelectItem;
        }

        private void Update()
        {
            if (UnityEngine.Input.GetMouseButtonDown(1))
                switch (_isActive)
                {
                    case false when _isCanOpen:
                        StartAttack();
                        break;
                    case true:
                        EndAttack();
                        break;
                }

            if (UnityEngine.Input.GetKeyDown(KeyCode.E))
                DownItem();
            else if (UnityEngine.Input.GetKeyDown(KeyCode.Q))
                UpItem();

            if (_isActive && selectArrow != null)
            {
                _xValue += 360f * Time.deltaTime;
                selectArrow.localRotation = Quaternion.Euler(_xValue, 0f, 90f);
            }
        }

        private void SelectItem()
        {
            if (!_isActive || _itemIdx < 0 || _itemIdx >= itemBtns.Count)
                return;

            itemBtns[_itemIdx].onClick?.Invoke();
        }

        private void StartAttack()
        {
            if (skillsName == null || skillsName.Count == 0)
                return;

            InitializeUI();

            _itemIdx = FindFirstValidIndex(0, +1);
            _xValue = 0f;

            atkUI.SetActive(true);
            selectArrow.gameObject.SetActive(true);
            _isActive = true;

            ApplySelection();
        }

        private void EndAttack()
        {
            Bus<UnitSetMoveEvent>.Raise(new UnitSetMoveEvent(true));
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(true));

            if (skillUI != null)
                skillUI.CancelSkill();

            Bus<UnitAttackEvent>.Raise(new UnitAttackEvent(false));

            if (selectArrow != null)
                selectArrow.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

            InitializeUI();
        }

        private void InitializeUI()
        {
            if (selectArrow != null)
                selectArrow.anchoredPosition = new Vector3(80f, 186.5f, 0f);
            
            _xValue = 0f;

            if (atkUI != null)
                atkUI.SetActive(false);

            if (selectArrow != null)
                selectArrow.gameObject.SetActive(false);

            _isActive = false;
        }

        private void UpItem()
        {
            if (!_isActive || SlotCount <= 0)
                return;

            _itemIdx = FindNextValidIndex(_itemIdx, -1);
            ApplySelection();
        }

        private void DownItem()
        {
            if (!_isActive || SlotCount <= 0)
                return;

            _itemIdx = FindNextValidIndex(_itemIdx, +1);
            ApplySelection();
        }

        private void ApplySelection()
        {
            if (SlotCount <= 0)
                return;

            _itemIdx = WrapIndex(_itemIdx, SlotCount);

            // 화살표 위치 이동
            if (selectArrow != null && _itemIdx < items.Count && items[_itemIdx] != null)
            {
                Vector3 pos = items[_itemIdx].localPosition;
                pos.x = 80f;
                pos.z = 0f;
                selectArrow.localPosition = pos;
            }

            // 전체 색상 초기화
            foreach (var skill in skillsName)
                if (skill != null)
                    skill.color = Color.white;

            // 전체 설명/선택표시 끄기
            foreach (var text in explainTxt)
                if (text != null)
                    text.gameObject.SetActive(false);

            foreach (var item in selectItem.Where(item => item != null))
                item.SetActive(false);

            // 선택 색상 가져오기
            Color selectedColor = Color.white;

            if (_itemIdx < selectItem.Count && selectItem[_itemIdx] != null)
            {
                var img = selectItem[_itemIdx].GetComponent<Image>();

                if (img != null)
                    selectedColor = img.color;
            }

            // 선택 강조
            if (_itemIdx < skillsName.Count && skillsName[_itemIdx] != null)
                skillsName[_itemIdx].color = selectedColor;

            if (_arrowImage != null)
                _arrowImage.color = selectedColor;

            if (_itemIdx < explainTxt.Count && explainTxt[_itemIdx] != null)
                explainTxt[_itemIdx].gameObject.SetActive(true);

            if (_itemIdx < selectItem.Count && selectItem[_itemIdx] != null)
                selectItem[_itemIdx].SetActive(true);
        }

        private int FindFirstValidIndex(int start, int direction)
        {
            int count = SlotCount;

            if (count <= 0)
                return 0;

            int idx = WrapIndex(start, count);

            for (int i = 0; i < count; i++)
            {
                if (IsValidSlot(idx))
                    return idx;

                idx = WrapIndex(idx + direction, count);
            }

            return start;
        }

        private int FindNextValidIndex(int current, int direction)
        {
            int count = SlotCount;

            if (count <= 0)
                return 0;

            int idx = current;

            for (int i = 0; i < count; i++)
            {
                idx = WrapIndex(idx + direction, count);
                if (IsValidSlot(idx)) return idx;
            }

            return current;
        }

        private bool IsValidSlot(int idx)
        {
            if (idx < 0 || idx >= skillsName.Count)
                return false;

            if (skillsName[idx] == null)
                return false;

            return !string.IsNullOrEmpty(skillsName[idx].text);
        }

        private int WrapIndex(int idx, int count)
        {
            if (count <= 0)
                return 0;

            idx %= count;

            if (idx < 0)
                idx += count;

            return idx;
        }

        private void SetSkillUIName(SkillUIEvent evt)
        {
            int idx = evt.skillIdx + 1;

            if (idx >= 0 && idx < skillsName.Count && skillsName[idx] != null)
                skillsName[idx].text = evt.skillName;

            if (idx >= 0 && idx < explainTxt.Count && explainTxt[idx] != null)
                explainTxt[idx].text = $"코스트 - {evt.skillCost}";

            if (_isActive)
                ApplySelection();
        }

        private void SetAtkUI(SetAtkUIEvent evt)
        {
            _isCanOpen = !evt.isLock;
            InitializeUI();
        }

        public void ActiveUI()
        {
            StartAttack();
        }
    }
}