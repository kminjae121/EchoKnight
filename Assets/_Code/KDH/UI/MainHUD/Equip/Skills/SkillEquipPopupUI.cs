using Code.Core.Events.Bus;
using Code.Core.Managers;
using Code.UnitSystem.SkillSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class SkillEquipPopupUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Button equipButton;
        [SerializeField] private Button unequipButton;
        
        private RectTransform _rectTransform;
        private SkillSO _targetSkill;
        private bool _isCurrentlyEquipped;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();

            Bus<SkillEquipPopupEvent>.Subscribe(HandlePopupEvent);
            
            equipButton.onClick.AddListener(HandleEquip);
            unequipButton.onClick.AddListener(HandleUnequip);

            Hide();
        }

        private void OnDestroy()
        {
            Bus<SkillEquipPopupEvent>.Unsubscribe(HandlePopupEvent);
            equipButton.onClick.RemoveListener(HandleEquip);
            unequipButton.onClick.RemoveListener(HandleUnequip);
        }

        private void HandlePopupEvent(SkillEquipPopupEvent evt)
        {
            if (evt.Skill == null)
            {
                Hide();
                return;
            }

            _targetSkill = evt.Skill;
            _isCurrentlyEquipped = evt.IsEquipped;
            
            if (descriptionText != null)
                descriptionText.text = _isCurrentlyEquipped ? "스킬을\n해제하시겠습니까?" : "스킬을\n장착하시겠습니까?";

            equipButton.gameObject.SetActive(!_isCurrentlyEquipped && !evt.IsReadOnly);
            unequipButton.gameObject.SetActive(_isCurrentlyEquipped && !evt.IsReadOnly);

            if (evt.Pivot != null)
            {
                _rectTransform.position = evt.Pivot.position;
                _rectTransform.anchoredPosition += new Vector2(evt.Offset.x, evt.Offset.y);
            }

            gameObject.SetActive(true);
            transform.SetAsLastSibling();
        }

        private void HandleEquip()
        {
            if (_targetSkill != null && !_isCurrentlyEquipped)
            {
                Bus<SkillEquipEvent>.Raise(new SkillEquipEvent(_targetSkill));
            }
            Hide();
        }

        private void HandleUnequip()
        {
            if (_targetSkill != null && _isCurrentlyEquipped)
            {
                Bus<SkillUnequipEvent>.Raise(new SkillUnequipEvent(_targetSkill));
            }
            Hide();
        }

        private void Hide()
        {
            gameObject.SetActive(false);
            _targetSkill = null;
        }
    }
}