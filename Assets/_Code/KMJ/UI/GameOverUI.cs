using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.UI
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private Button restartBtn;
        [SerializeField] private Button returnBtn;

        private void Awake()
        {
            restartBtn.onClick.AddListener(RestartBtn);
            returnBtn.onClick.AddListener(ReturnHome);
        }

        private void OnDisable()
        {
            restartBtn.onClick.RemoveListener(RestartBtn);
            returnBtn.onClick.RemoveListener(ReturnHome);
        }

        public void ReturnHome()
        {
            SceneManager.LoadScene("Lobby");
        }
        public void RestartBtn()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}