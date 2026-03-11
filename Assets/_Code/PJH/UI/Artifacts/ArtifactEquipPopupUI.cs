using Code.Core.Events.Bus;
using Code.UnitSystem.ArtifactSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class ArtifactEquipPopupUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI tierText;
        [SerializeField] private Button equipButton;
        [SerializeField] private Button unequipButton;

        private RectTransform _rectTransform;
        private ArtifactSO _targetArtifact;
        private bool _isCurrentlyEquipped;
        private Canvas _parentCanvas;
        private int _frameCountOnOpen;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _parentCanvas = GetComponentInParent<Canvas>();

            Bus<ArtifactPopupEvent>.Subscribe(HandlePopupEvent);
            
            equipButton.onClick.AddListener(HandleEquip);
            unequipButton.onClick.AddListener(HandleUnequip);

            Hide();
        }

        private void OnDestroy()
        {
            Bus<ArtifactPopupEvent>.Unsubscribe(HandlePopupEvent);
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

        private void HandlePopupEvent(ArtifactPopupEvent evt)
        {
            if (evt.Artifact == null)
            {
                Hide();
                return;
            }

            _targetArtifact = evt.Artifact;
            _isCurrentlyEquipped = evt.IsEquipped;

            nameText.text = _targetArtifact.artifactName;
            descriptionText.text = _targetArtifact.description;

            if (tierText != null)
            {
                tierText.text = _targetArtifact.rarity.ToString();
                SetTierTextColor(_targetArtifact.rarity);
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

        private void SetTierTextColor(ArtifactRarity rarity)
        {
            switch (rarity)
            {
                case ArtifactRarity.Legendary: tierText.color = new Color(1f, 0.84f, 0f); break;
                case ArtifactRarity.Epic: tierText.color = new Color(0.63f, 0.13f, 0.94f); break;
                case ArtifactRarity.Rare: tierText.color = new Color(0f, 0.5f, 1f); break;
                case ArtifactRarity.Uncommon: tierText.color = Color.green; break;
                case ArtifactRarity.Common: default: tierText.color = Color.gray; break;
            }
        }

        private void HandleEquip()
        {
            if (_targetArtifact != null && !_isCurrentlyEquipped)
                Bus<ArtifactEquipEvent>.Raise(new ArtifactEquipEvent(_targetArtifact));
            Hide();
        }

        private void HandleUnequip()
        {
            if (_targetArtifact != null && _isCurrentlyEquipped)
                Bus<ArtifactUnequipEvent>.Raise(new ArtifactUnequipEvent(_targetArtifact));
            Hide();
        }

        private void Hide()
        {
            gameObject.SetActive(false);
            _targetArtifact = null;
        }
    }
}