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
        private Canvas _parentCanvas;
        private int _frameCountOnOpen;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _parentCanvas = GetComponentInParent<Canvas>();

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

        private void Update()
        {
            if (Time.frameCount == _frameCountOnOpen) return;

            if (UnityEngine.Input.GetMouseButtonDown(0) || UnityEngine.Input.GetMouseButtonDown(1))
            {
                Camera cam = null;
                if (_parentCanvas != null && _parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                {
                    cam = _parentCanvas.worldCamera;
                }

                if (!RectTransformUtility.RectangleContainsScreenPoint(_rectTransform, UnityEngine.Input.mousePosition, cam))
                {
                    Hide();
                }
            }
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
            {
                descriptionText.text = _isCurrentlyEquipped ? "스킬을\n해제하시겠습니까?" : "스킬을\n장착하시겠습니까?";
            }

            equipButton.gameObject.SetActive(!_isCurrentlyEquipped && !evt.IsReadOnly);
            unequipButton.gameObject.SetActive(_isCurrentlyEquipped && !evt.IsReadOnly);

            gameObject.SetActive(true);
            transform.SetAsLastSibling();
            _frameCountOnOpen = Time.frameCount;

            if (_parentCanvas != null)
            {
                RectTransform canvasRect = _parentCanvas.transform as RectTransform;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect, 
                    evt.Position, 
                    _parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _parentCanvas.worldCamera, 
                    out Vector2 localPoint);

                float normalizedX = (localPoint.x - canvasRect.rect.xMin) / canvasRect.rect.width;
                float normalizedY = (localPoint.y - canvasRect.rect.yMin) / canvasRect.rect.height;

                float pivotX = normalizedX > 0.5f ? 1f : 0f;
                float pivotY = normalizedY > 0.5f ? 1f : 0f;
                _rectTransform.pivot = new Vector2(pivotX, pivotY);
            }

            if (_parentCanvas != null && _parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    _parentCanvas.transform as RectTransform, evt.Position, _parentCanvas.worldCamera, out Vector3 worldPoint);
                _rectTransform.position = worldPoint;
            }
            else
            {
                _rectTransform.position = evt.Position;
            }
        }

        private void HandleEquip()
        {
            if (_targetSkill != null && !_isCurrentlyEquipped)
            {
                if (SkillSendManager.Instance != null)
                {
                    SkillSendManager.Instance.EquipSkill(_targetSkill);
                }
                Bus<SkillEquipEvent>.Raise(new SkillEquipEvent(_targetSkill));
            }
            Hide();
        }

        private void HandleUnequip()
        {
            if (_targetSkill != null && _isCurrentlyEquipped)
            {
                if (SkillSendManager.Instance != null)
                {
                    SkillSendManager.Instance.RemoveSkill(_targetSkill);
                }
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