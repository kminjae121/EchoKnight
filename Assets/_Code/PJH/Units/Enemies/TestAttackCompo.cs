using System;
using Code.Core.Debugs;
using Code.Map;
using Code.Utils;
using DG.Tweening;
using UnityEngine;

namespace Code.UnitSystem.Enemies
{
    public class TestAttackCompo : MonoBehaviour, IUnitComponent
    {
        [SerializeField] private int attackTileRange = 1;

        public Unit Owner { get; private set; }

        public event Action OnAttackEnd;

        private GridMap _gridMap;
        private UnitRotation _rotationCompo;

        public void Initialize(Unit owner)
        {
            Owner = owner;
            _gridMap = GridMap.Instance;
            _rotationCompo = owner.GetUnitCompo<UnitRotation>();
        }

        public bool CanAttackToTarget(GameObject target)
        {
            if (target == null || Owner == null)
                return false;

            _gridMap ??= GridMap.Instance;

            if (_gridMap == null)
                return false;

            Vector2Int myPos = _gridMap.WorldToGridPosition(Owner.transform.position);
            Vector2Int targetPos = _gridMap.WorldToGridPosition(target.transform.position);
            float distance = DistanceUtils.GetEuclideanDistance(myPos, targetPos);

            UnityLogger.Log(
                $"myPos : {myPos} targetPos : {targetPos}, {distance <= attackTileRange}");
            return distance <= attackTileRange;
        }

        public void Attack(GameObject target)
        {
            if (target == null || Owner == null)
            {
                OnAttackEnd?.Invoke();
                return;
            }

            _rotationCompo?.SetDir(target.transform.position);

            Owner.transform.DOShakePosition(0.3f, 0.4f).WaitForCompletion();
            OnAttackEnd?.Invoke();
        }
    }
}
