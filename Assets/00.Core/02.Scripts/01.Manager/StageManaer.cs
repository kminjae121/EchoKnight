using System;
using _00.Core._02.Scripts._01.Manager;
using _00.Core._02.Scripts._06.SO;
using Code.Core.Events.Bus;
using UnityEngine;

namespace Code.Core
{
    public class StageManaer : MonoBehaviour
    {
        [SerializeField] private StageSO stageSO;

        private int _stageClearCount => stageSO.endCount;

        private int _currentStageCount;
        
        private void Awake()
        {
            Bus<EnemyDieEvent>.Subscribe(EndStage);
        }

        private void Start()
        {
            Bus<GageEvent>.Raise(new GageEvent(stageSO.behaviorCost));
        }

        private void EndStage(EnemyDieEvent evt)
        {
            if (_stageClearCount <= _currentStageCount)
            {
                Bus<StageClearEvent>.Raise(new StageClearEvent(true));
                return;
            }

            _currentStageCount+=1;
        }
    }
}