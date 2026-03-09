using System;
using System.Linq;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using NUnit.Framework.Constraints;
using UnitSystem;
using UnityEngine;

namespace Code.UnitSystem
{
    public class RangeComponent : MonoBehaviour, IUnitComponent
    {
        [SerializeField] protected Vector3 _verticalCheckBoxSize;
        [SerializeField] protected Vector3 _horizontalCheckBoxSize;
        [SerializeField] protected LayerMask _whatIsTarget;

        public bool IsActive { get; protected set; }
        public bool isMove;

        protected Collider[] _verticalCollider;
        protected Collider[] _horizontalCollider;

        protected Action _resetTileEvent;
        protected Unit _owner;

        private UnitManageRangeCompo _rangeComponent;

        public void Initialize(Unit owner)
        {
            _owner = owner;
            _rangeComponent = owner.GetUnitCompo<UnitManageRangeCompo>();
        }

        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnDestroy()
        {
        }

        public void ResetTile()
        {
            if (_verticalCollider == null || _horizontalCollider == null)
                return;
            
            ProcessTiles(_verticalCollider, false);
            ProcessTiles(_horizontalCollider, false);

            IsActive = false;

            if (!isMove)
            {
                _horizontalCollider = null;
                _verticalCollider = null;
            }

            _resetTileEvent?.Invoke();
        }

        public void ReCheckInRange()
        {
            if (_verticalCollider != null)
                ReEnableTiles(_verticalCollider);

            if (_horizontalCollider != null)
                ReEnableTiles(_horizontalCollider);

            IsActive = true;
        }

        public void FindObjectInRange()
        {
            _rangeComponent.RemoveAllRange();

            Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(true));

            _verticalCollider = Physics.OverlapBox(transform.position,
                _verticalCheckBoxSize, Quaternion.identity, _whatIsTarget);
            _horizontalCollider = Physics.OverlapBox(transform.position,
                _horizontalCheckBoxSize, Quaternion.identity, _whatIsTarget);

            ProcessTiles(_verticalCollider, true);
            ProcessTiles(_horizontalCollider, true);

            IsActive = true;
        }

        public void EndAct()
        {
            IsActive = false;
        }

        private void ProcessTiles(Collider[] colliders, bool enable)
        {
            foreach (var col in colliders)
            {
                if (!col.TryGetComponent(out IMapTile tile))
                    continue;

                if (!isMove)
                    tile.SetEnemy(enable);
                else
                {
                    if (tile.HasObstacle)
                        continue;

                    tile.SetWalkable(enable);
                }
            }
        }

        private void ReEnableTiles(Collider[] colliders)
        {
            foreach (var col in colliders)
            {
                if (!col.TryGetComponent(out IMapTile tile))
                    continue;

                if (!tile.HasObstacle)
                    tile.SetWalkable(true);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, _verticalCheckBoxSize);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, _horizontalCheckBoxSize);
        }
    }
}