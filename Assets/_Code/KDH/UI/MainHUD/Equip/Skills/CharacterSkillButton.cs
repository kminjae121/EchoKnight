using Code.Core.Events.Bus;
using Code.UnitSystem.SkillSystem;
using GondrLib.ObjectPool.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.UI
{
    public class CharacterSkillButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPoolable
    {
        [Header("Pooling Settings")]
        [SerializeField] private PoolingItemSO poolingType;

        [Header("UI Elements")]
        [SerializeField] private Image iconImage;
        [SerializeField] private GameObject hoverImage;

        [Header("Hover Settings")]
        [SerializeField] private GameObject customHoverArea;

        [Header("Popup Settings")]
        [SerializeField] private RectTransform popupPivot;
        [SerializeField] private Vector2 popupOffset;

        private SkillSO _skill;
        private bool _isEquipped;
        private bool _isTooltipSuppressed;
        private bool _isHovering;
        private GondrLib.ObjectPool.Runtime.Pool _pool;
        private HoverDetector _hoverDetector;
        private RectTransform _rectTransform;

        public PoolingItemSO PoolingType => poolingType;
        public GameObject GameObject => gameObject;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();

            if (customHoverArea != null)
            {
                _hoverDetector = customHoverArea.GetComponent<HoverDetector>();
                if (_hoverDetector == null)
                    _hoverDetector = customHoverArea.AddComponent<HoverDetector>();

                _hoverDetector.OnEnter += HandleHoverEnter;
                _hoverDetector.OnExit += HandleHoverExit;
            }
        }

        private void OnDestroy()
        {
            if (_hoverDetector != null)
            {
                _hoverDetector.OnEnter -= HandleHoverEnter;
                _hoverDetector.OnExit -= HandleHoverExit;
            }
        }

        public RectTransform GetPivot() => popupPivot != null ? popupPivot : _rectTransform;

        public void SetUpPool(GondrLib.ObjectPool.Runtime.Pool pool) => _pool = pool;

        public void ResetItem()
        {
            _skill = null;
            _isEquipped = false;
            _isTooltipSuppressed = false;
            _isHovering = false;
            UpdateHoverState();
        }

        public void ReturnToPool()
        {
            if (_pool != null) _pool.Push(this);
            else Destroy(gameObject);
        }

        private void OnDisable()
        {
            _isHovering = false;
            _isTooltipSuppressed = false;
            UpdateHoverState();
            
            if (_skill != null)
            {
                Bus<SkillUIHoverEvent>.Raise(new SkillUIHoverEvent(null, null));
            }
        }

        public void SetSkill(SkillSO skill, bool isEquipped)
        {
            _skill = skill;
            if (iconImage != null)
            {
                iconImage.sprite = skill.skillUIImage;
                iconImage.color = Color.white;
            }
            _isEquipped = isEquipped;
            UpdateHoverState();
        }

        private void UpdateHoverState()
        {
            if (hoverImage != null) hoverImage.SetActive(_isEquipped || _isHovering);
        }

        private void HandleHoverEnter()
        {
            if (_skill != null) 
            {
                _isHovering = true;
                UpdateHoverState();
                
                if (!_isTooltipSuppressed)
                {
                    Bus<SkillUIHoverEvent>.Raise(new SkillUIHoverEvent(_skill, GetPivot(), popupOffset));
                }
            }
        }

        private void HandleHoverExit()
        {
            if (_skill != null) 
            {
                _isHovering = false;
                UpdateHoverState();
                
                _isTooltipSuppressed = false;
                Bus<SkillUIHoverEvent>.Raise(new SkillUIHoverEvent(null, null));
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (customHoverArea == null) HandleHoverEnter();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (customHoverArea == null) HandleHoverExit();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_skill == null) return;

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Bus<SkillUIHoverEvent>.Raise(new SkillUIHoverEvent(null, null));
                Bus<SkillEquipPopupEvent>.Raise(new SkillEquipPopupEvent(_skill, _isEquipped, GetPivot(), popupOffset));
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (!_isTooltipSuppressed)
                {
                    _isTooltipSuppressed = true;
                    Bus<SkillUIHoverEvent>.Raise(new SkillUIHoverEvent(null, null));
                }
                else
                {
                    Bus<SkillEquipPopupEvent>.Raise(new SkillEquipPopupEvent(_skill, _isEquipped, GetPivot(), popupOffset));
                }
            }
        }
    }
}