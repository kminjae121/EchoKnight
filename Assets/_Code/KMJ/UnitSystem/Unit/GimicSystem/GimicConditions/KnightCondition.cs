using System.Collections.Generic;
using UnityEngine;

namespace Code.UnitSystem.GimicSystem
{
    public enum KnightPhase
    {
        OnePhase = 0,
        TwoPhase = 1,
        LastPage = 2,
    }

    public class KnightCondition : GimicCondition
    {
        [SerializeField] private int stack = 0;

        [SerializeField] private KnightPhase phase = KnightPhase.OnePhase;
        
        private readonly int[] BonusStacks = { 3, 6, 10 };
        

        public override void SetCondition()
        {
            stack += 1;
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
            phase = KnightPhase.OnePhase;
            stack = 0;
        }
    }
}