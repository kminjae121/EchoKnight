using System.Collections.Generic;
using Code.Core.Events.Bus;
using Code.Items;
using Code.UI;
using UnityEngine;

namespace Code.Managers
{
    public class RewardManager : MonoBehaviour
    {
        [SerializeField] private List<ItemSO> itemList;
        [SerializeField] BattleRewardUI battleRewardUI;
        
        private void Awake()
        {
            Bus<StageClearEvent>.Subscribe(HandleStageClear);
        }

        private void OnDestroy()
        {
            Bus<StageClearEvent>.Unsubscribe(HandleStageClear);
        }

        private void HandleStageClear(StageClearEvent evt)
        {
            List<ItemSO> rewardItems = new();
            
            // 임시
            rewardItems.Add(itemList[Random.Range(0, itemList.Count)]);
            rewardItems.Add(itemList[Random.Range(0, itemList.Count)]);
            
            battleRewardUI.Open(rewardItems);
        }
    }
}