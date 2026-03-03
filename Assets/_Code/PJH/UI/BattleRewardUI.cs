using System;
using System.Collections.Generic;
using Code.Items;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.UI
{
    public class BattleRewardUI : MonoBehaviour
    {
        [SerializeField] private Button nextButton;
        [SerializeField] private RewardItemButton reawardButtonPrefab;
        [SerializeField] private Transform rewardTrm;

        private List<RewardItemButton> spawnedButtons = new();

        private void Awake()
        {
            nextButton.onClick.AddListener(HandleNextButton);
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }
        
        public void Open(List<ItemSO> rewards)
        {
            gameObject.SetActive(true);

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