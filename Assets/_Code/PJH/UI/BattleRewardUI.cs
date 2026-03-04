using System.Collections.Generic;
using Code.Items;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.UI
{
    public class BattleRewardUI : Panel
    {
        [Header("Buttons")]
        [SerializeField] private Button nextButton;

        [Header("Rewards")]
        [SerializeField] private RewardItemButton reawardButtonPrefab;
        [SerializeField] private Transform rewardTrm;

        private List<RewardItemButton> spawnedButtons = new();

        public override void Awake()
        {
            base.Awake();
            nextButton.onClick.AddListener(HandleNextButton);
        }

        public void SetupRewards(List<ItemSO> rewards)
        {
            base.Open();

            foreach (var item in rewards)
            {
                var button = Instantiate(reawardButtonPrefab, rewardTrm);
                button.SetItem(item);
                spawnedButtons.Add(button);
            }
        }
        
        private void HandleNextButton()
        {
            SceneManager.LoadScene("ExpeditionMapScene");
        }
    }
}