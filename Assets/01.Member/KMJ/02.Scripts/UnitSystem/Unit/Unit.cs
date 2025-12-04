using System;
using System.Collections.Generic;
using System.Linq;
using EntityComponent;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace UnitSystem
{
    public class Unit : MonoBehaviour
    {
        
        [field: SerializeField] public UnitSO unitSO { get; private set; }

        public float turnSpeed { get; private set; }
        public bool isPlayerUnit {get; private set;}
        public float turnGauge {get; private set;}
        public Action OnDeathEvent { get; private set; }
        public Action OnHitEvent { get; private set; }

        protected Dictionary<Type,IUnitComponent> _components = new Dictionary<Type, IUnitComponent>();


        protected virtual void OnEnable()
        {
            AddUnitComponents();
            InitializeUnitComponents();
            AfterInitializeComponents();
            
            turnSpeed = unitSO.turnSpeed;
            isPlayerUnit = false;
            turnGauge = 0;
        }

        public void SetThisUnit(bool isSelect)
        {
            isPlayerUnit = isSelect;
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