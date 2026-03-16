using Code.Core.Events.Bus;
using TMPro;
using UnityEngine;

namespace Code.UI
{
    public class SkillTooltipUI : Panel
    {
        [Header("Settings")]
        [SerializeField] private Canvas targetCanvas;
        [SerializeField] private Vector2 offset = new(20, -20);
        
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI skillNameText;
        [SerializeField] private TextMeshProUGUI skillDescText;
        [SerializeField] private TextMeshProUGUI skillCostText;

        private RectTransform _rect;
        
        public override void Awake()
        {
            base.Awake();
            _rect = GetComponent<RectTransform>();
            
            Bus<SkillUIHoverEvent>.Subscribe(HandleHoverUI);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            Bus<SkillUIHoverEvent>.Unsubscribe(HandleHoverUI);
        }
        
        private void HandleHoverUI(SkillUIHoverEvent evt)
        {
            if (evt.Skill == null)
            {
                base.Close();
                return;
            }

            skillNameText.text = evt.Skill.skillName;
            skillDescText.text = evt.Skill.SkillDescription;
            skillCostText.text = evt.Skill.SkillCost.ToString();
            
            SetRectPosition(evt.Transform);
            base.Open();
        }
        
        private void SetRectPosition(RectTransform trm)
        {
            if (trm == null)
                return;

            Vector3 worldPos = trm.TransformPoint(new Vector3(trm.rect.width / 2f, 0));
            Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(targetCanvas.worldCamera, worldPos);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(targetCanvas.transform as RectTransform,
                screenPoint, targetCanvas.worldCamera, out Vector2 localPoint);

            _rect.anchoredPosition = localPoint + offset;
        }
    }
}