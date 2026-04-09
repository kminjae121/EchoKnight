using System.Collections.Generic;
using Code.Items;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.UI
{
    public class BattleRewardUI : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button nextButton;

        [Header("Rewards")]
        [SerializeField] private RewardItemButton reawardButtonPrefab;
        [SerializeField] private Transform rewardTrm;

        private readonly List<RewardItemButton> spawnedButtons = new();

        [SerializeField] private GameObject rewardUI;

        private void Awake()
        {
            nextButton.onClick.AddListener(HandleNextButton);
            rewardUI.SetActive(false);
        }

        private void OnDestroy()
        {
            nextButton.onClick.RemoveListener(HandleNextButton);
        }

        public void Open(List<ItemSO> rewards)
        {
            Time.timeScale = 0;
            rewardUI.SetActive(true);

            foreach (var item in rewards)
            {
                var button = Instantiate(reawardButtonPrefab, rewardTrm);
                button.SetItem(item);
                spawnedButtons.Add(button);
            }
        }
        
        private void HandleNextButton()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("ExpeditionMapScene");
        }
    }
}