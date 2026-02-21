using System;
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

        private void OnDisable()
        {
            
            returnBtn.onClick.RemoveListener(ReturnHome);
        }

        public void ReturnHome()
        {
            SceneManager.LoadScene("ExpeditionMapScene");
        }
    }
}