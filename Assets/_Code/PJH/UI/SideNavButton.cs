using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.UI
{
    [RequireComponent(typeof(Button))]
    public class SideNavButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Panel Settings")]
        [SerializeField] private string targetPanelId;

        [Header("Hover Animation Settings")]
        [SerializeField] private float animationDuration = 0.3f;
        [SerializeField] private Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);
        [SerializeField] private Ease animationEase = Ease.OutCubic;
        
        [Header("UI Elements")]
        [SerializeField] private CanvasGroup panelNameCanvasGroup;

        private Button _navButton;
        private Vector3 _originalScale;
        private Tween _scaleTween;
        private Tween _fadeTween;

        private void Awake()
        {
            _navButton = GetComponent<Button>();
            _originalScale = transform.localScale;

            if (panelNameCanvasGroup != null)
                panelNameCanvasGroup.alpha = 0f;

            _navButton.onClick.AddListener(HandleNavButtonClick);
        }

        private void OnDestroy()
        {
            _navButton.onClick.RemoveListener(HandleNavButtonClick);
            
            _scaleTween?.Kill();
            _fadeTween?.Kill();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _scaleTween?.Kill();
            _scaleTween = transform.DOScale(hoverScale, animationDuration).SetEase(animationEase);

            if (panelNameCanvasGroup != null)
            {
                _fadeTween?.Kill();
                _fadeTween = panelNameCanvasGroup.DOFade(1f, animationDuration).SetEase(animationEase);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _scaleTween?.Kill();
            _scaleTween = transform.DOScale(_originalScale, animationDuration).SetEase(animationEase);

            if (panelNameCanvasGroup != null)
            {
                _fadeTween?.Kill();
                _fadeTween = panelNameCanvasGroup.DOFade(0f, animationDuration).SetEase(animationEase);
            }
        }

        private void HandleNavButtonClick()
        {
            if (string.IsNullOrEmpty(targetPanelId))
            {
                Debug.LogError("대상 패널 ID가 설정되지 않았습니다.");
                return;
            }

            PanelManager.CloseAll();
            PanelManager.Open(targetPanelId);
        }
    }
}