using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Events.Bus;
using Code.Expedition.Logic;
using Code.UnitSystem;
using UnitSystem;
using UnityEngine;

namespace Code.Managers
{
    public class UnitManager : MonoBehaviour
    {
        private readonly HashSet<Unit> activeUnits = new();

        private void Awake()
        {
            Bus<UnitSpawnEvent>.Subscribe(RegisterUnit);
            Bus<UnitDeadEvent>.Subscribe(RemoveUnit);
        }

        private void Start()
        {
            if (BattleContext.Instance != null &&
                BattleContext.Instance.CurrentEnemies != null)
            {
                SpawnEnemiesFromContext();
            }
        }

        private void OnDestroy()
        {
            Bus<UnitSpawnEvent>.Unsubscribe(RegisterUnit);
            Bus<UnitDeadEvent>.Unsubscribe(RemoveUnit);
        }
        
        private void SpawnEnemiesFromContext()
        {
            var enemies = BattleContext.Instance.CurrentEnemies;

            Vector3 startPosition = new Vector3(5, 0, 0);
            float spacing = 2.0f;

            for (int i = 0; i < enemies.Count; i++)
            {
                UnitInfoSO enemyData = enemies[i];
                
                if (enemyData.UnitPrefab != null) 
                {
                    Vector3 spawnPos = startPosition + new Vector3(i * spacing, 0, 0);
                    GameObject enemyObj = Instantiate(enemyData.UnitPrefab, spawnPos, Quaternion.identity);
                    
                }
                else
                {
                    Debug.LogError($"적 데이터({enemyData.name})에 프리팹이 연결되지 않았습니다.");
                }
            }
        }

        #region Public Functions

        public IReadOnlyCollection<Unit> GetAllUnits()
            => activeUnits;

        public IEnumerable<Unit> GetPlayerUnits()
            => activeUnits.Where(unit => unit.IsPlayerUnit);

        public IEnumerable<Unit> GetEnemyUnits()
            => activeUnits.Where(unit => !unit.IsPlayerUnit);
        
        #endregion
        
        private void RegisterUnit(UnitSpawnEvent evt)
            => activeUnits.Add(evt.Unit);

        private void RemoveUnit(UnitDeadEvent evt)
            => activeUnits.Remove(evt.Unit);
    }
}