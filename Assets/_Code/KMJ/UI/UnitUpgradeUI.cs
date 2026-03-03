using System;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using GameEventChannel;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class UnitUpgradeUI : MonoBehaviour
    {
        private UnitInGameSO unitInfoSO;

        [SerializeField] private Image unitImage;
        [SerializeField] private Button unitHealthUpgradeButton;
        [SerializeField] private Button unitDamageUpgradeButton;
        [SerializeField] private Button unitSkillDamageUpgradeBtn;

        private void Awake()
        {
            Bus<SendUnitInfoEvent>.Subscribe(SetUnitSO);
        }

        private void OnDestroy()
        {
            Bus<SendUnitInfoEvent>.Unsubscribe(SetUnitSO);
        }

        public void SetUnitSO(SendUnitInfoEvent unit)
        {
            unitInfoSO = unit.unitState.Data.unitInGame;

            unitImage.sprite = unit.unitState.Data.UnitImage;
            
            unitHealthUpgradeButton.onClick.RemoveAllListeners();
            unitDamageUpgradeButton.onClick.RemoveAllListeners();
            unitSkillDamageUpgradeBtn.onClick.RemoveAllListeners();
            
            unitHealthUpgradeButton.onClick.AddListener(MaxHealthUpgrade);
            unitDamageUpgradeButton.onClick.AddListener(DamageUpgrade);
            unitSkillDamageUpgradeBtn.onClick.AddListener(SkillDamageUpgrade);
        }


        private void MaxHealthUpgrade()
        {
         
            if (unitDamageUpgradeButton == null)
                return;

            if (unitInfoSO == null)
                return;

            unitInfoSO.Maxhealth += 10;
        }

        private void SkillDamageUpgrade()
        {
            if (unitSkillDamageUpgradeBtn == null)
                return;

            if (unitInfoSO == null)
                return;
            
            unitInfoSO.SkillDamage += 10;
        }

        private void DamageUpgrade()
        {
            if (unitDamageUpgradeButton == null)
                return;

            if (unitInfoSO == null)
                return;


            unitInfoSO.AtkDamage += 10;
        }
    }
}