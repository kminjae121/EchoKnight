using Code.Core.Events.Bus;
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

        public void ReturnHome()
        {
            Bus<StageClearEvent>.Raise(new StageClearEvent(true));
            SceneManager.LoadScene("ExpeditionMapScene");
        }
    }
}