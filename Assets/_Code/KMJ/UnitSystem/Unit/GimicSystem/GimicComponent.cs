using UnityEngine;

namespace Code.UnitSystem.GimicSystem
{
    public class GimicComponent : MonoBehaviour
    {
        [field: SerializeField] public GimicCondition Condition { get; private set; }
        [field: SerializeField] public GimicOperation Operation { get; private set; }
        private GimicEventComponent eventCompo;

        public void SetCondition()
        {
            Condition.SetCondition();

            if (Condition.CheckCondition())
            {
                Condition.RemoveCondition();
                Operation.StartOperation();
            }
        }
    }
}