using System;
using System.Collections.Generic;
using Code.Core.Events.Bus;
using Code.UnitSystem.GimicSystem;
using Unity.Cinemachine;
using UnityEngine;

namespace Code.UnitSystem.Combat
{
    public class ShootItemAttackManager : MonoBehaviour, IUnitComponent
    {
        [SerializeField] private List<ShootItem> shootItems;
        [SerializeField] private AttackDataSO atkData;
        
        private Unit _unit;
        
        private GameObject _target = null;

        private Dictionary<string, ShootItem> _shootItemDict = new Dictionary<string, ShootItem>();
        private DamageData _damageData;

        public Action hitEvent;

        private float _addDamage;

        
        public void Initialize(Unit owner)
        {
            _unit = owner;
            
            hitEvent += GiveDamage;
            
            shootItems.ForEach(item =>
            {
                _shootItemDict.Add(item.itemName, item);
            });
        }

        private void Start()
        {
            if (_unit as CharacterUnit)
            {
                CharacterUnit characterUnit = _unit as CharacterUnit;
            }
        }

        private void OnDisable()
        {
            hitEvent -= GiveDamage;
        }

        public void SetTarget(GameObject target)
        {
            _target = target;
        }

        public void SetDamageData(DamageData damageData, float addDamage)
        {
            _damageData = damageData;
            _addDamage = addDamage;
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
            shootItemCompo.SetTarget(_target.gameObject);
            shootItem.transform.rotation = Quaternion.Euler(rotation);
        }

        private void GiveDamage()
        {
            Bus<CamShakeEvent>.Raise(new CamShakeEvent(0.4f));

            Bus<DamageEvent>.Raise(new DamageEvent(_damageData,atkData,_target.gameObject,_addDamage,_unit, false));
        }
    }
}