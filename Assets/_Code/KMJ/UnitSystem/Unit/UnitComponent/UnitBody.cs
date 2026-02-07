using Code.UnitSystem;
using UnityEngine;

namespace _Code.KMJ.UnitSystem.Unit.UnitComponent
{
    public enum BodyType
    {
        None,
        Head,
        Back
    }

    public class UnitBody : MonoBehaviour, IUnitComponent
    {
        [SerializeField] private BodyType body;
        public void Initialize(Code.UnitSystem.Unit owner)
        {
            
        }

        public BodyType GetBodyType()
        {
            return this.body;
        }
    }
}