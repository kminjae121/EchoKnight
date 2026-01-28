using System;
using Code.Lobby;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class CarriageCharacterUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI unitNameText;
        [SerializeField] private Image unitIconImage;
        [SerializeField] private Button unitButton;
        
        private RecruitUnitData unitData;

        public event Action<RecruitUnitData> OnUnitButtonClicked;

        private void Start()
        {
            unitButton.onClick.AddListener(HandleUnitButton);
        }

        private void OnDestroy()
        {
            unitButton.onClick.RemoveListener(HandleUnitButton);
        }

        public void SetUnitInfo(RecruitUnitData newInfo)
        {
            unitData = newInfo;
            unitNameText.text = unitData.unitSO.UnitName;
            unitIconImage.sprite = unitData.unitSO.UnitImage;
        }

        private void HandleUnitButton()
        {
            OnUnitButtonClicked?.Invoke(unitData);
            gameObject.SetActive(false);
        }
    }
}