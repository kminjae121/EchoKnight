using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.UI
{
    public class SlotHoverClickTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Action OnClick;
        public Action<Vector2> OnLeftClick;
        public Action<Vector2> OnRightClick;
        public Action<Vector2> OnHoverEnter;
        public Action OnHoverExit;

        [Header("Hover Effect")]
        public GameObject hoverImage; 
        public bool useHoverVisuals = true;

        private Image _image;
        private Color _normalColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        private Color _hoverColor = Color.white;
        private bool _isInteractable = true;

        private void Awake()
        {
            _image = GetComponent<Image>();
            
            if (hoverImage != null) 
                hoverImage.SetActive(false);
        }

        public void SetInteractable(bool interactable)
        {
            _isInteractable = interactable;
            
            if (_image != null)
            {
                if (useHoverVisuals && hoverImage == null)
                    _image.color = interactable ? _normalColor : Color.white;
                else
                    _image.color = Color.white;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_isInteractable) return;
            
            if (useHoverVisuals)
            {
                if (hoverImage != null) hoverImage.SetActive(true);
                else if (_image != null) _image.color = _hoverColor;
            }
            
            OnHoverEnter?.Invoke(eventData.position);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_isInteractable) return;
            
            if (useHoverVisuals)
            {
                if (hoverImage != null) hoverImage.SetActive(false);
                else if (_image != null) _image.color = _normalColor;
            }
            
            OnHoverExit?.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isInteractable) return;

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                OnClick?.Invoke();
                OnLeftClick?.Invoke(eventData.position);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                OnRightClick?.Invoke(eventData.position);
            }
        }
    }
}