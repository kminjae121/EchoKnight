using Code.Core.Events.Bus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SkillTooltipUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image skillIconImage;
        [SerializeField] private TextMeshProUGUI skillNameText;
        [SerializeField] private TextMeshProUGUI skillDescText;
        [SerializeField] private TextMeshProUGUI skillCostText;
        [SerializeField] private TextMeshProUGUI skillDamageText;
        [SerializeField] private TextMeshProUGUI skillRangeText;

        private CanvasGroup _canvasGroup;
        
        private void Awake()
        {
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
            
            Show();
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