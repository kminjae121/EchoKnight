using System;
using UnitSystem;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace EnemySystem
{
    public class EnemyFightStart : MonoBehaviour
    {
        [SerializeField] private PlayableDirector director;

        private void Awake()
        {
            director.Stop();
        }
        public void Initialize(Unit owner)
        {
                
        }

        public void StartAttack()
        {
            //턴을 공격턴으로 바꿔줌
            director.time = 0;
            director.Evaluate();
            director.Play();   
        }

        public void StopActionTimeLine()
        {
            director.Stop();
            //턴을 바꿔줌
        }

    }
}