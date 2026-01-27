using Code.UnitSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class CharacterRecruitmentUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI DescriptionText;
        [SerializeField] private Button agreeButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private UnitStorageSO unitStorage;
        [SerializeField] private UnitListUI unitListUI;
        
        private UnitInfoSO unitInfo;

        private void Start()
        {
            agreeButton.onClick.AddListener(HandleAgreeButton);
            cancelButton.onClick.AddListener(HandleCancelButton);
            
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            agreeButton.onClick.RemoveListener(HandleAgreeButton);
            cancelButton.onClick.RemoveListener(HandleCancelButton);
        }

        public void SetCharacter(UnitInfoSO newInfo)
        {
            unitInfo = newInfo;
            DescriptionText.text = $"{newInfo.UnitName} 를 영입하시겠습니까?";
        }

        private void HandleAgreeButton()
        {
            unitStorage.units.Add(unitInfo);
            unitListUI.Refresh();
            gameObject.SetActive(false);
        }
        
        private void HandleCancelButton()
        {
            gameObject.SetActive(false);
        }
    }
}