using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace UnitSystem
{
    public class Unit : MonoBehaviour
    {
        public UnityEvent OnHitEvent = null;
        public UnityEvent OnDeadEvent = null;

        public bool isDead { get; private set; } = false;
        
        protected Dictionary<Type,IUnitComponent> _components = new Dictionary<Type, IUnitComponent>();


        protected virtual void Awake()
        {
            AddComponents();
            InitializeComponents();
            AfterInitialize();
            
            OnDeadEvent.AddListener(Dead);
        }

        protected virtual void Dead()
        {
            
        }
        private void InitializeComponents()
        {
            _components.Values.ToList().ForEach(component => component.Initialize(this));
        }

        private void AfterInitialize()
        {
            _components.Values.OfType<IAfterInitialize>()
                .ToList().ForEach(component => component.AfterInitialize());
        }

        private void AddComponents()
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