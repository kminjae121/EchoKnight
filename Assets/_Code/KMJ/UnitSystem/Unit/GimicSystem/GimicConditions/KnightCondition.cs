using System.Collections.Generic;
using UnityEngine;

namespace Code.UnitSystem.GimicSystem
{
    // enum의 값이 곧 stackValues 인덱스가 되도록 맞춤
    public enum KnightPhase
    {
        None = 0,
        OnePhase = 1,
        TwoPhase = 2,
        ThreePhase = 3,
        FourPhase = 4,
    }

    public class KnightCondition : GimicCondition
    {
        [SerializeField] private int stack = 0;
        
        [SerializeField] private List<float> stackValues = new List<float>();

        [SerializeField] private KnightPhase phase = KnightPhase.None;

        public override void SetCondition()
        {
            stack += 1;
        }

        public override bool CheckCondition()
        {
            int idx = (int)phase;
            
            if (stackValues == null || idx < 0 || idx >= stackValues.Count)
                return false;

            return stack >= stackValues[idx];
        }

        public override void RemoveCondition()
        {
            stack = 0;
        }
    }
}