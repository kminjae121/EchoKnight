using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class UnitUpgradeUI : MonoBehaviour
    {
        [SerializeField] private UnitSO unitInfoSO;

        [SerializeField] private Button unitHealthUpgradeButton;
        [SerializeField] private Button unitDamageUpgradeButton;

        public void SetUnitSO(UnitSO unit)
        {
            unitInfoSO = unit;
        }


        private void MaxHealthUpgrade()
        {
         
            if (unitDamageUpgradeButton == null)
                return;

            if (unitInfoSO == null)
                return;

            unitInfoSO.Maxhealth += 10;
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