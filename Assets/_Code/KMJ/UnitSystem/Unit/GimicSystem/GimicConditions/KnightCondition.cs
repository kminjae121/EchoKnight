using System.Collections.Generic;
using Code.Core.Events.Bus;
using UnityEngine;

namespace Code.UnitSystem.GimicSystem
{
    public enum KnightPhase
    {
        OnePhase = 0,
        TwoPhase = 1,
        ThreePhase = 2,
        LastPage = 3,
    }

    public class KnightCondition : GimicCondition
    {
        [SerializeField] private int stack = 0;

        [SerializeField] private KnightPhase phase = KnightPhase.OnePhase;
        
        private readonly int[] BonusStacks = { 3, 6, 10 };
        

        public override void SetCondition()
        {
            if (stack >= 10)
                return;
            
            stack += 1;
            Bus<KnightGimicBarEvent>.Raise(new KnightGimicBarEvent(stack));
        }

        public override bool CheckCondition()
        {
            int idx = (int)phase; 
            
            if (phase == KnightPhase.LastPage)
                return false;
            
            if (stack >= BonusStacks[idx])
            {
                phase = (KnightPhase)(idx + 1);
                return true;
            }
            
            return false;
        }

        public override void RemoveCondition()
        {
            Bus<KnightGimicBarEvent>.Raise(new KnightGimicBarEvent(0));
            phase = KnightPhase.OnePhase;
            stack = 0;
        }
    }
}