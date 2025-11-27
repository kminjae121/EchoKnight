using System;
using System.Collections.Generic;
using System.Linq;
using UnitSystem;
using UnityEngine;
using UnityEngine.Events;

namespace EntityComponent
{
    public abstract class Entity : MonoBehaviour
    {
        public UnityEvent OnHitEvent;
        public UnityEvent OnDeathEvent;
        
        public bool IsDead { get; set; }
        protected Dictionary<Type, IEntityComponent> _components;

        public void EntityDestroy()
        {
            Destroy(gameObject);
        }
        
        protected virtual void Awake()
        {
            _components = new Dictionary<Type, IEntityComponent>();
            AddComponents();
            InitializeComponents();
            AfterInitializeComponents();
        }


        protected virtual void AddComponents()
        {
            GetComponentsInChildren<IEntityComponent>().ToList()
                .ForEach(component => _components.Add(component.GetType(), component));
        }

        protected virtual void InitializeComponents()
        {
            _components.Values.ToList().ForEach(component => component.Initialize(this));
        }

        private void AfterInitializeComponents()
        {
            _components.Values.OfType<IAfterInitialize>()
                .ToList().ForEach(component => component.AfterInitialize());
        }
        
        public T GetCompo<T>() where T : IEntityComponent
            => (T)_components.GetValueOrDefault(typeof(T));

        public IEntityComponent GetCompo(Type type)
            => _components.GetValueOrDefault(type);

        public void RotateToTarget(Vector3 targetPosition, bool isSmooth = false)
        {
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);

            if (isSmooth)
            {
                const float smoothRotateSpeed = 15f;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                    targetRotation, Time.deltaTime * smoothRotateSpeed);
            }
            else
            {
                transform.rotation = targetRotation;
            }
        }
    }
}