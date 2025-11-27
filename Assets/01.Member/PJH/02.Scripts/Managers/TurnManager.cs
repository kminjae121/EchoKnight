using System.Collections.Generic;
using System.Linq;
using UnitSystem;
using UnityEngine;

namespace Code.Managers
{
    public class TurnManager : MonoBehaviour
    {
        [SerializeField] private int showFutureTurnCount = 5;

        private List<Unit> activeUnits = new();
        private Queue<Unit> turnQueue = new();
        private Unit _currentUnit;

        public void RegisterUnit(Unit unit)
        {
            if (!activeUnits.Contains(unit))
                activeUnits.Add(unit);
        }
        
        public void RemoveUnit(Unit unit)
        {
            if (activeUnits.Contains(unit))
                activeUnits.Remove(unit);
        }
        
        public void StartBattle()
        {
            StartTurn();
        }
        
        private void StartTurn()
        {
            // var orderedUnits = activeUnits
            //     .Where(u => !u.IsDead)
            //     .OrderByDescending(u => u.speed)
            //     .ToList();

            StartNextUnitTurn();
        }

        private void StartNextUnitTurn()
        {
            if (turnQueue.Count == 0)
            {
                StartTurn();
                return;
            }

            _currentUnit = turnQueue.Dequeue();
            
            // 플레이어 유닛인지 아닌지 검사해서 함수 실행
        }

        private void EndUnitTurn()
        {
            StartNextUnitTurn();
        }
        
        private void StartPlayerTurn()
        {
            
        }
        
        private void StartEnemyTurn()
        {
            
        }
        
        private void UpdateFutureTurnUI()
        {
            
        }
        
        private List<Unit> PredictFutureTurns(List<Unit> units)
        {
            // 속도 기반으로 예측해서 정렬 뒤 리턴해주기
            
            return null;
        }
    }
}