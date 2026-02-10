using Code.Core.Events.Bus;
using Code.UnitSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class CharacterInfoUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI unitNameText;
        [SerializeField] private Image unitImage;
        [SerializeField] private Button exitButton;
        
        private UnitState _unit;

        private void Awake()
        {
            exitButton.onClick.AddListener(HandleExitButton);
            Bus<CharacterInfoEvent>.Subscribe(HandleUnitInfo);
            
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            exitButton.onClick.RemoveListener(HandleExitButton);
            Bus<CharacterInfoEvent>.Unsubscribe(HandleUnitInfo);
        }
        
        public void ActivePanel()
        {
            gameObject.SetActive(true);
        }
        
        private void HandleUnitInfo(CharacterInfoEvent evt)
        {
            _unit = evt.Unit;
            
            unitNameText.text = _unit.Data.UnitName;
            unitImage.sprite = _unit.Data.UnitImage;
        }
        
        private void HandleExitButton()
        {
            gameObject.SetActive(false);
        }
    }
}