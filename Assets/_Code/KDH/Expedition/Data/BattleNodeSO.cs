using System.Collections.Generic;
using _00.Core._02.Scripts._06.SO;
using Code.UnitSystem;
using UnityEngine;

namespace Code.Expedition.Data
{
    [CreateAssetMenu(fileName = "NewBattleNode", menuName = "SO/Expedition/BattleNode")]
    public class BattleNodeSO : ExpeditionNodeSO
    {
        [Header("Battle Config")]
        public StageSO stageData;
        public List<UnitSpawnSO> enemiesToSpawn;
        public string battleSceneName = "BattleScene";

        private void OnEnable()
        {
            nodeType = ExpeditionNodeType.Battle;
        }
    }
}