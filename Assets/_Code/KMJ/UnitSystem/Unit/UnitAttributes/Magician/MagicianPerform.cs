using UnityEngine;

namespace Code.UnitSystem.UnitAttributes
{
    public class MagicianPerform : MonoBehaviour, IUnitPerform
    {
        private Unit _unit;
        [SerializeField] private MagicianCondition condition;
        
        public void Initialize(Unit unit)
        {
            _unit = unit;
        }

        public void Perform(GameObject target)
        {
            if (condition._magicianType == MagicianType.Heal)
            {
                Debug.Log("힐줌");
            }
            else
            {
                Debug.Log("딜줌");
            }
        }
    }
}