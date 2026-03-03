using Code.Core.Events.Bus;
using TMPro;
using UnityEngine;

namespace Code.UI
{
    public class SkillTooltipUI : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Vector2 offset = new(20, -20);
        [SerializeField] private TextMeshProUGUI skillNameText;
        [SerializeField] private TextMeshProUGUI skillDescText;
        [SerializeField] private TextMeshProUGUI skillCostText;

        private RectTransform _rect;
        
        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            
            Bus<SkillUIHoverEvent>.Subscribe(HandleHoverUI);
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Bus<SkillUIHoverEvent>.Unsubscribe(HandleHoverUI);
        }
        
        private void HandleHoverUI(SkillUIHoverEvent evt)
        {
            if (evt.Skill == null)
            {
                gameObject.SetActive(false);
                return;
            }

            skillNameText.text = evt.Skill.skillName;
            skillDescText.text = evt.Skill.SkillDescription;
            skillCostText.text = evt.Skill.SkillCost.ToString();
            
            SetRectPosition(evt.Transform);
            gameObject.SetActive(true);
        }
        
        private void SetRectPosition(RectTransform trm)
        {
            if (trm == null)
                return;

            Vector3 worldPos = trm.TransformPoint(new Vector3(trm.rect.width / 2f, 0));
            Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, worldPos);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
                screenPoint, canvas.worldCamera, out Vector2 localPoint);

            _rect.anchoredPosition = localPoint + offset;
        }
    }
}