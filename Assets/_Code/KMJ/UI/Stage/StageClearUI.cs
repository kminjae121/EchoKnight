using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.UI
{
    public class StageClearUI : MonoBehaviour
    {
        [SerializeField] private Button returnBtn;

        private void Awake()
        {
            returnBtn.onClick.AddListener(ReturnHome);
        }

        private void OnDisable()
        {
            returnBtn.onClick.RemoveListener(ReturnHome);
        }

        public void ReturnHome()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("ExpeditionMapScene");
        }
    }
}