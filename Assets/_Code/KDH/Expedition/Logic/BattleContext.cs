using System.Collections.Generic;
using _00.Core._02.Scripts._06.SO;
using Code.Expedition.Data;
using Code.Core;
using Code.UnitSystem;

namespace Code.Expedition.Logic
{
    public class BattleContext : MonoSingleton<BattleContext>
    {
        public List<UnitInfoSO> CurrentEnemies { get; private set; }
        public StageSO CurrentStage { get; private set; }

        public void SetContext(BattleNodeSO data)
        {
            CurrentEnemies = data.enemiesToSpawn;
            CurrentStage = data.stageData;
        }

        public void ClearContext()
        {
            CurrentEnemies = null;
            CurrentStage = null;
        }
    }
}