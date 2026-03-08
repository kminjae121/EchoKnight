using Code.Core.Events.Bus;
using Code.UnitSystem.SkillSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class SkillEquipPopupUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Button equipButton;
        [SerializeField] private Button unequipButton;
        [SerializeField] private Button cancelButton;
        
        private RectTransform _rectTransform;
        private SkillSO _targetSkill;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.pivot = new Vector2(0f, 1f);

            Bus<SkillEquipPopupEvent>.Subscribe(HandlePopupEvent);
            
            equipButton.onClick.AddListener(HandleEquip);
            unequipButton.onClick.AddListener(HandleUnequip);
            cancelButton.onClick.AddListener(Hide);

            Hide();
        }

        private void OnDestroy()
        {
            Bus<SkillEquipPopupEvent>.Unsubscribe(HandlePopupEvent);
            
            equipButton.onClick.RemoveListener(HandleEquip);
            unequipButton.onClick.RemoveListener(HandleUnequip);
            cancelButton.onClick.RemoveListener(Hide);
        }

        private void HandlePopupEvent(SkillEquipPopupEvent evt)
        {
            _targetSkill = evt.Skill;
            
            equipButton.gameObject.SetActive(!evt.IsEquipped);
            unequipButton.gameObject.SetActive(evt.IsEquipped);

            _rectTransform.position = evt.Position;
            gameObject.SetActive(true);
        }

        private void HandleEquip()
        {
            if (_targetSkill != null)
                Bus<SkillEquipEvent>.Raise(new SkillEquipEvent(_targetSkill));
            
            Hide();
        }

        private void HandleUnequip()
        {
            if (_targetSkill != null)
                Bus<SkillUnequipEvent>.Raise(new SkillUnequipEvent(_targetSkill));
            
            Hide();
        }

        private void Hide()
        {
            gameObject.SetActive(false);
            _targetSkill = null;
        }
    }
}