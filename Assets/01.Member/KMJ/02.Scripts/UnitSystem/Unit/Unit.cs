using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using UnityEngine;

namespace UnitSystem
{
    public class Unit : MonoBehaviour, ITurnable
    {
        [field: SerializeField] public UnitSO unitSO { get; private set; }

        public float TurnSpeed { get; private set; }
        public float TurnGauge { get; set; }
        
        public bool IsReadyDoAct => TurnGauge >= 100f;
        public bool IsPlayerUnit {get; private set;}
        
        public Action OnDeathEvent { get; private set; }
        public Action OnHitEvent { get; private set; }

        protected readonly Dictionary<Type,IUnitComponent> _components = new();

        protected virtual void OnEnable()
        {
            AddUnitComponents();
            InitializeUnitComponents();
            AfterInitializeComponents();
            
            TurnSpeed = unitSO.turnSpeed;
            IsPlayerUnit = unitSO.isPlayerUnit;
            TurnGauge = 0f;
        }

      //  protected virtual void Awake()
      //  {
      //      AddUnitComponents();
      //      InitializeUnitComponents();
      //      
      //      turnSpeed = unitSO.turnSpeed;
      //      isPlayerUnit = unitSO.isPlayerUnit;
      //      turnGauge = unitSO.turnGauge;
      //  }

        protected virtual void Dead()
        {
            Bus<UnitDeadEvent>.Raise(new UnitDeadEvent(this));
        }
        
        private void InitializeUnitComponents()
        {
            _components.Values.ToList().ForEach(component => component.Initialize(this));
        }

        private void AfterInitializeComponents()
        {
            _components.Values.OfType<IAfterInitialize>()
                .ToList().ForEach(component => component.AfterInitialize());
        }

        private void AddUnitComponents()
        {
            GetComponentsInChildren<IUnitComponent>().ToList()
                .ForEach(component => _components.Add(component.GetType(), component));
        }
        
        public T GetUnitCompo<T>() where T : IUnitComponent => 
            (T)_components.GetValueOrDefault(typeof(T));

        public IUnitComponent GetUnitCompo(Type type)
            => _components.GetValueOrDefault(type);
    }
}