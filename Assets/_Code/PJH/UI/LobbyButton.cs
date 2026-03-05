using UnityEngine;

namespace Code.UI
{
    public class LobbyButton : MonoBehaviour
    {
        [Header("Panel IDs")]
        [SerializeField] private string characterSelectPanelId = "PartyPanel";

        public void ReadyBattle() => PanelManager.Open(characterSelectPanelId);

        public void CancelBattle() => PanelManager.Close(characterSelectPanelId);
    }
}