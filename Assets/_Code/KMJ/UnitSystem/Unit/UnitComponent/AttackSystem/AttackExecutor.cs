using System;
using Code.EntityComponent;
using Code.UnitSystem;
using UnitSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Code.AttackSystem
{
    public class AttackExecutor : MonoBehaviour, IUnitComponent
    {
        [SerializeField] private AttackPresenter atkPresenter;
        [SerializeField] private UnitRotation rotationCompo;
        [SerializeField] private MeshRenderer ownCircleMesh;
        [SerializeField] private Material basicMaterial;
        
        public DamageData DamageData;
        public float AtkDamage { get; private set; }
        public float AddDamage { get; private set; }
        
        private const float _attackCost = 15f;
        private CharacterUnit _characterUnit;
        private UnitCostComponent _costCompo;
        
        public UnityEvent<GameObject> attackEvent;
        public UnityEvent attackEndEvent;

        public void Initialize(Unit owner)
        {
            _characterUnit = owner as  CharacterUnit;
        }

        private void Start()
        {
            _costCompo = _characterUnit.GetUnitCompo<UnitCostComponent>();
            AtkDamage = _characterUnit.UnitStatCompo.GetStat<float>(StatInfo.AtkDamage);
        }

        private void Awake()
        {
            DamageData = new DamageData
            {
                damage = AtkDamage
            };
        }

        public DamageData GetDamageData()
        {
            DamageData.damage = AtkDamage + AddDamage;
            return DamageData;
        }
        
        public bool CanAttack()
        {
            return _costCompo != null && _costCompo.GetCurrentCost() >= _attackCost;
        }

        public void SetRotation(GameObject target)
        {
            rotationCompo.SetDir(target.transform.position);
        }
        
        public bool TryExecute(GameObject target)
        {
            if (target == null)
                return false;

            if (!CanAttack())
            {
                atkPresenter.ShowWarning("AP가 부족합니다.");
            }

            _costCompo.RemoveCost(_attackCost);

            rotationCompo.SetDir(target.transform.position);
            
            attackEvent?.Invoke(target);

            ownCircleMesh.material = basicMaterial;

            return true;
        }
    }
}