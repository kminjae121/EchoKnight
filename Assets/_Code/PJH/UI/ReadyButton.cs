using UnityEngine;

namespace Code.UI
{
    public class ReadyButton : MonoBehaviour
    {
        [SerializeField] private GameObject characterSelectUI;
        
        public void ReadyBattle()
        {
            characterSelectUI.SetActive(true);
        }

        public void CancelBattle()
        {
            characterSelectUI.SetActive(false);
        }
    }
}