using Code.Core.Events.Bus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class MessagePopupUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button checkButton;

        private void Awake()
        {
            Bus<ShowMessageUIEvent>.Subscribe(HandleShowMessage);
            checkButton.onClick.AddListener(Hide);
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Bus<ShowMessageUIEvent>.Unsubscribe(HandleShowMessage);
            checkButton.onClick.RemoveListener(Hide);
        }
        
        private void HandleShowMessage(ShowMessageUIEvent evt)
        {
            Show(evt.Message);
        }
        
        private void Show(string message)
        {
            gameObject.SetActive(true);
            messageText.text = message;
        }
        
        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}