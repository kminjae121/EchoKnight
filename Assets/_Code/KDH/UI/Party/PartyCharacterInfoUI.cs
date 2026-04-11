using Code.Core.Events.Bus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class PartyCharacterInfoUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image characterImage;
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI characterDescText;

        [Header("Stat Texts (TMP)")]
        [SerializeField] private TextMeshProUGUI maxHealthText;
        [SerializeField] private TextMeshProUGUI atkText;
        [SerializeField] private TextMeshProUGUI defText;
        [SerializeField] private TextMeshProUGUI moveSpeedText;
        [SerializeField] private TextMeshProUGUI turnSpeedText;
        [SerializeField] private TextMeshProUGUI criticalProbabilityText;
        [SerializeField] private TextMeshProUGUI criticalDamageIncreaseText;
        [SerializeField] private TextMeshProUGUI maxSkillCostText;
        [SerializeField] private TextMeshProUGUI recoverySkillCostText;

        private void Awake()
        {
            Bus<PartyCharacterHoverEvent>.Subscribe(HandleHover);
            Bus<PartyCharacterSelectEvent>.Subscribe(HandleSelect);
            Bus<PartyCharacterDeselectEvent>.Subscribe(HandleDeselect);
        }

        private void OnDestroy()
        {
            Bus<PartyCharacterHoverEvent>.Unsubscribe(HandleHover);
            
            Bus<PartyCharacterSelectEvent>.Unsubscribe(HandleSelect);
            Bus<PartyCharacterDeselectEvent>.Unsubscribe(HandleDeselect);
        }

        private void HandleHover(PartyCharacterHoverEvent evt)
        {
            characterImage.gameObject.SetActive(evt.CharacterImage != null);
            characterImage.sprite = evt.CharacterImage;
            
            characterNameText.text = evt.CharacterName ?? string.Empty;
            characterDescText.text = evt.CharacterDesc ?? string.Empty;
        }

        private void HandleSelect(PartyCharacterSelectEvent evt) => UpdateStats(evt.Unit);
        private void HandleDeselect(PartyCharacterDeselectEvent evt) => UpdateStats(evt.Unit);

        private void UpdateStats(UnitSO data)
        {
            if (data == null) return;

            if (maxHealthText != null) maxHealthText.text = data.Maxhealth.ToString("F1");
            if (atkText != null) atkText.text = data.AttackDamage.ToString("F1");
            if (defText != null) defText.text = data.DefensivePower.ToString("F1");
            if (moveSpeedText != null) moveSpeedText.text = data.MoveRange.ToString("F1");
            if (turnSpeedText != null) turnSpeedText.text = data.turnSpeed.ToString("F1");
            
            if (criticalProbabilityText != null) criticalProbabilityText.text = $"{data.CriticalProbability:F1}%";
            if (criticalDamageIncreaseText != null) criticalDamageIncreaseText.text = data.CriticalDamageIncrease.ToString("F1");
            
            if (maxSkillCostText != null) maxSkillCostText.text = data.MaxSkillCost.ToString();
            if (recoverySkillCostText != null) recoverySkillCostText.text = data.RecoverySkillCost.ToString();
        }
    }
}