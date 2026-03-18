using Code.Core.Events.Bus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SkillTooltipUI : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Canvas targetCanvas;
        [SerializeField] private Vector2 offset = new(20, -20);
        
        [Header("UI Elements")]
        [SerializeField] private Image skillIconImage;
        [SerializeField] private TextMeshProUGUI skillNameText;
        [SerializeField] private TextMeshProUGUI skillDescText;
        [SerializeField] private TextMeshProUGUI skillCostText;
        [SerializeField] private TextMeshProUGUI skillDamageText;
        [SerializeField] private TextMeshProUGUI skillRangeText;

        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
            
            Bus<SkillUIHoverEvent>.Subscribe(HandleHoverUI);

            Hide();
        }

        private void OnDestroy()
        {
            Bus<SkillUIHoverEvent>.Unsubscribe(HandleHoverUI);
        }

        private void Update()
        {
            if (gameObject.activeSelf && UnityEngine.Input.GetMouseButtonDown(0))
            {
                Hide();
            }
        }

        private void HandleHoverUI(SkillUIHoverEvent evt)
        {
            if (evt.Skill == null)
            {
                Hide();
                return;
            }

            if (skillIconImage != null)
            {
                skillIconImage.sprite = evt.Skill.skillUIImage;
                skillIconImage.gameObject.SetActive(true);
            }

            if (skillNameText != null) skillNameText.text = evt.Skill.skillName;
            if (skillDescText != null) skillDescText.text = evt.Skill.SkillDescription;
            if (skillCostText != null) skillCostText.text = evt.Skill.SkillCost.ToString();
            if (skillDamageText != null) skillDamageText.text = evt.Skill.SkillDamage.ToString();
            if (skillRangeText != null) skillRangeText.text = evt.Skill.SkillRange.ToString();
            
            SetRectPosition();
            Show();
        }
        
        private void SetRectPosition()
        {
            if (targetCanvas == null) return;

            Vector2 screenPoint = UnityEngine.Input.mousePosition;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                targetCanvas.transform as RectTransform,
                screenPoint, 
                targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : targetCanvas.worldCamera, 
                out Vector2 localPoint);

            RectTransform canvasRect = targetCanvas.transform as RectTransform;
            float normalizedX = (localPoint.x - canvasRect.rect.xMin) / canvasRect.rect.width;
            float normalizedY = (localPoint.y - canvasRect.rect.yMin) / canvasRect.rect.height;

            float pivotX = normalizedX > 0.5f ? 1f : 0f;
            float pivotY = normalizedY > 0.5f ? 1f : 0f;
            
            _rectTransform.pivot = new Vector2(pivotX, pivotY);

            float offsetX = pivotX == 1f ? -Mathf.Abs(offset.x) : Mathf.Abs(offset.x);
            float offsetY = pivotY == 1f ? -Mathf.Abs(offset.y) : Mathf.Abs(offset.y);
            
            _rectTransform.anchoredPosition = localPoint + new Vector2(offsetX, offsetY);
        }

        private void Show()
        {
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}