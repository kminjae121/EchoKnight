using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    [RequireComponent(typeof(Button))]
    public class CloseAllPanelsButton : MonoBehaviour
    {
        private Button _closeButton;

        private void Awake()
        {
            _closeButton = GetComponent<Button>();
            _closeButton.onClick.AddListener(HandleCloseAll);
        }

        private void OnDestroy()
        {
            _closeButton.onClick.RemoveListener(HandleCloseAll);
        }

        private void HandleCloseAll()
        {
            PanelManager.CloseAll();
        }
    }
}