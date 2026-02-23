using System;
using System.Collections.Generic;
using _Code.KMJ.UnitSystem.Unit.UnitComponent;
using Code.Core.Events.Bus;
using Code.EntityComponent;
using Code.UnitSystem;
using Unity.Cinemachine;
using UnityEngine;

namespace UnitSystem
{
    public class ShootItemAttackManager : MonoBehaviour, IUnitComponent
    {
        [SerializeField] private List<ShootItem> shootItems;
        [SerializeField] private AttackDataSO atkData;
        
        private Unit _unit;
        
        private GameObject _target = null;


        private Dictionary<string, ShootItem> _shootItemDict = new Dictionary<string, ShootItem>();
        private DamageData _damageData;
        private CinemachineImpulseSource impulseSource;

        public Action hitEvent;

        
        public void Initialize(Unit owner)
        {
            _unit = owner;
            impulseSource = GameObject.Find("ImpulseSource").GetComponent<CinemachineImpulseSource>();

            hitEvent += GiveDamage;
            
            
            shootItems.ForEach(item =>
            {
                
                _shootItemDict.Add(item.itemName, item);
            });
        }
        

        private void OnDisable()
        {
            hitEvent -= GiveDamage;
        }

        public void SetTarget(GameObject target)
        {
            _target = target;
        }

        public void SetDamageData(DamageData damageData)
        {
            _damageData = damageData;
        }

        public void CreateShootItem(string itemName, Vector3 pos, Vector3 rotation)
        {
            ShootItem itemCompo = _shootItemDict.GetValueOrDefault(itemName);
            
            
            GameObject item = itemCompo.gameObject;

            if (item == null)
                return;
            
            if (_target == null)
                return;
            
            GameObject shootItem = Instantiate(item, pos ,Quaternion.identity);

            ShootItem shootItemCompo = shootItem.GetComponent<ShootItem>();
            
            shootItemCompo.SetShootItemCompo(this);
            
            shootItemCompo.SetTarget(_target);
            
            shootItem.transform.rotation = Quaternion.Euler(rotation);

            
        }

        private void GiveDamage()
        {
            Bus<HitStopEvent>.Raise(new HitStopEvent(0.2f,0.25f));
            Bus<TurnEndUIEvent>.Raise(new TurnEndUIEvent(false));
                
            impulseSource.GenerateImpulse(0.4f);  
            
            _target.GetComponent<IDamageable>().ApplyDamage(_damageData,transform.position, transform.position,
                atkData,_unit);
        }
        
    }
}