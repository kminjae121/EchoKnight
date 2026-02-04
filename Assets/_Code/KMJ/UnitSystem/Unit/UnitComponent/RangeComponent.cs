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
        private UnitManageRangeCompo _rangeComponent;

        protected Unit _owner;
        
        [SerializeField] private Vector3 _verticalCheckBoxSize;
        [SerializeField] private Vector3 _horizontalCheckBoxSize;

        protected Collider[] _verticalCollider;
        
        protected Collider[] _horizontalCollider;
        
        [SerializeField] protected LayerMask _whatIsTarget;

        protected Action ResetTileEvent;
        
        public bool _isAct { get; set; } = false;

        public bool isMove = false;

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
            if (_horizontalCollider == null)
                return;
            
            
            _horizontalCollider.ToList().ForEach(obj =>
            {
                if (obj.TryGetComponent(out IMapTile tile))
                {
                    if (!isMove)
                    {
                        tile.SetEnemy(false);
                    }
                    else
                    {
                        if (!tile.HasObstacle)
                        {
                            tile.SetWalkable(false);
                        }   
                    }
                }
            });
            
            if(_verticalCollider == null)
                return;
            
            _verticalCollider.ToList().ForEach(obj =>
            {
                if (obj.TryGetComponent(out IMapTile tile))
                {
                    if (!isMove)
                    {
                        tile.SetEnemy(false);
                    }
                    else
                    {
                        if (!tile.HasObstacle)
                        {
                            tile.SetWalkable(false);
                        }   
                    }
                }
            });
            
            _isAct = false;
 
            _horizontalCollider.ToList().Clear();
            _verticalCollider.ToList().Clear();
            if (!isMove)
            {
                _horizontalCollider = null;
                _verticalCollider = null;
            }
            ResetTileEvent?.Invoke();
        }

        public void ReCheckInRange()
        {
            _verticalCollider.ToList().ForEach(obj =>
            {
                if (obj.TryGetComponent(out IMapTile tile))
                {
                    if (!tile.HasObstacle)    
                    {
                        tile.SetWalkable(true);      
                    }
                }
            });
            
            _horizontalCollider.ToList().ForEach(obj =>
            {
                if (obj.TryGetComponent(out IMapTile tile))
                {
                    if (!tile.HasObstacle)
                    {
                        tile.SetWalkable(true);
                    }
                }
            });
            
            _isAct = true;
        }


        public void FindObjectInRange()
        {
            _rangeComponent.RemoveAllRange();
            
            Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(true));
            
            _verticalCollider = Physics.OverlapBox(transform.position, _verticalCheckBoxSize, Quaternion.identity, _whatIsTarget);
            _horizontalCollider = Physics.OverlapBox(transform.position, _horizontalCheckBoxSize, Quaternion.identity, _whatIsTarget);

            _verticalCollider.ToList().ForEach(obj =>
            {
                if (obj.TryGetComponent(out IMapTile tile))
                {
                    if (!isMove)
                    {
                        tile.SetEnemy(true);    
                    }
                    else
                    {
                        if (!tile.HasObstacle)    
                        {
                            tile.SetWalkable(true);      
                        }   
                    }
                }
            });
            
            _horizontalCollider.ToList().ForEach(obj =>
            {
                if (obj.TryGetComponent(out IMapTile tile))
                {
                    if (!isMove)
                    {
                        tile.SetEnemy(true);    
                    }
                    else
                    {
                        if (!tile.HasObstacle)    
                        {
                            tile.SetWalkable(true);      
                        }   
                    }
                }
            });
            
            _isAct = true;
        }

        public void EndAct()
        {
            _isAct = false;
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