using Code.UnitSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class UnitSlotUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI unitNameText;
        //[SerializeField] private Image unitIconImage;

        public void SetUnit(UnitInfoSO unit)
        {
            unitNameText.text = unit.UnitName;
            //unitIconImage.sprite = unit.UnitImage;
        }
    }
}