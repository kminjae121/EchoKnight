using Code.Core.Events.Bus;
using TMPro;
using UnityEngine;

namespace Code.UI
{
    public class SkillTooltipUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI skillNameText;
        [SerializeField] private TextMeshProUGUI skillDescText;
        [SerializeField] private TextMeshProUGUI skillCostText;

        private void Awake()
        {
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
            //skillDescText.text 
        }
    }
}