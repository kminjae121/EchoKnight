using System;
using System.Collections.Generic;
using Code.Core.Debugs;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using Code.Managers;
using Code.Map;
using Code.UnitSystem.Combat;
using GondrLib.Dependencies;
using UnityEngine;

namespace Code.UnitSystem.UnitAttributes
{
    public class KnightDefenseRange : MonoBehaviour, IUnitComponent
    {
        [SerializeField] private int rangeSize;

        private CharacterUnit _unit;
        private UnitHealth _unitHealthCompo;
        [Inject] protected TurnManager _turnManager;
        
        public HashSet<CharacterUnit> Targets { get; private set; } = new();

        public void Initialize(Unit owner)
        {
            _unit = owner as CharacterUnit;

            if (_unit == null)
            {
                UnityLogger.LogWarning("유닛이 존재하지 않습니다");
                return;
            }

            _unitHealthCompo = _unit.GetUnitCompo<UnitHealth>();
            if (_unitHealthCompo == null)
            {
                UnityLogger.LogWarning("UnitHealth 컴포넌트가 없습니다");
                return;
            }

            _unitHealthCompo.OnDefenseEvent += ReduceDamage;
            
            Injector.InjectInto(this);


            if (_turnManager != null)
            {
                _turnManager.OnTurnStart += FindUnitInDefenseRange;
            }
        }

        private void OnDestroy()
        {
            foreach (var target in Targets)
            {
                if (target != null && target.HealthCompo != null)
                    target.HealthCompo.OnDefenseEvent -= ReduceDamage;
            }
            Targets.Clear();

            if (_unitHealthCompo != null)
                _unitHealthCompo.OnDefenseEvent -= ReduceDamage;
            
            if(_turnManager != null)
                _turnManager.OnTurnStart -= FindUnitInDefenseRange;
        }
        
        /// <summary>
        /// 방어할 수 있는 유닛을 구하는 코드
        /// </summary>
        public void FindUnitInDefenseRange()
        {
            foreach (var target in Targets)
            {
                if (target != null && target.HealthCompo != null)
                    target.HealthCompo.OnDefenseEvent -= ReduceDamage;
            }

            Targets.Clear();

            if (_unit?.MoveCompo?.CurrentMapTile == null) return;

            CalculateRange();
        }

        /// <summary>
        /// 방어할 범위를 계산하는 코드
        /// </summary>
        private void CalculateRange()
        {
            Vector2Int center = _unit.MoveCompo.CurrentMapTile.GridPos;
            
            int result = -(rangeSize - 1) / 2;

            for (int x = result; x <= -result; x++)
            {
                for (int y = result; y <= -result; y++)
                {   
                    IMapTile tile = GridMap.Instance.GetTile(center + new Vector2Int(x, y));
                    if (tile == null) continue;

                    CharacterUnit characterUnit = tile.GetTileUnit() as CharacterUnit;
                    
                    if (characterUnit == null) continue;
                    if (characterUnit == _unit) continue;
                    if (characterUnit.HealthCompo == null) continue;
    
                    if (Targets.Add(characterUnit))
                    {
                        characterUnit.HealthCompo.OnDefenseEvent += ReduceDamage;
                    }
                }
            }
        }
    
        /// <summary>
        /// 데이지를 감소시켜주는 코드
        /// </summary>
        /// <param name="damage"></param>
        private void ReduceDamage(ref int damage)
        {
            damage = Mathf.RoundToInt(damage * 0.5f);
            
            Bus<UseGimicEvent>.Raise(new UseGimicEvent(UnitType.Knight, null));
        }

        private void OnValidate()
        {
            if (rangeSize % 2 == 0)
            {
                rangeSize += 1;
                Debug.LogError("홀수만 입력 가능합니다.");
            }
        }
    }
}