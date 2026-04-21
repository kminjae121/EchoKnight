using System;
using Code.Core.Events.Bus;
using Code.UnitSystem.Combat;
using UnityEngine;

namespace Code.SkillSystem.Skill.Archer.ArcherSkill
{
    public class BombArrow : MonoBehaviour
    {
        [Header("BombParticle")]
        [SerializeField] private ParticleSystem bombEffect;

        [Header("CastSetting")]
        [SerializeField] private LayerMask whatIsEnemy;
        [SerializeField] private Transform bombTrm;
        [SerializeField] private Vector3 castSize;
        [SerializeField] private int damage;
        
        private DamageData _damageData;
        
        private void Start()
        {
            _damageData.damage = damage;
        }

        public void Bomb()
        {
            ParticleSystem particle = Instantiate(bombEffect, transform.position, Quaternion.identity);
            particle.Play();
            
            Collider[] cols = Physics.OverlapBox(bombTrm.position, castSize, Quaternion.identity, whatIsEnemy);

            foreach (var col in cols)
            {
                Bus<DamageEvent>.Raise(new DamageEvent(_damageData, null, col.gameObject,0 ,null, false,false,0.2f));
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(bombTrm.position, castSize);
            Gizmos.color = Color.red;
        }
    }
}