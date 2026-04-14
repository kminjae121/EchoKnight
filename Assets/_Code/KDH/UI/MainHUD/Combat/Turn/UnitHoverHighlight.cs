using Code.Core.Events.Bus;
using Code.Core.Interfaces;
using UnityEngine;

namespace Code.UnitSystem
{
    [RequireComponent(typeof(ITurnable))]
    public class UnitHoverHighlight : MonoBehaviour
    {
        [Header("Highlight Settings")]
        [SerializeField] private UnitOutLineCompo outLineCompo;

        private ITurnable _myTurnable;


        private void Awake()
        {
            _myTurnable = GetComponent<ITurnable>();
            
            Bus<CombatUnitHoverEvent>.Subscribe(HandleHoverEvent);
        }

        private void OnDestroy()
        {
            Bus<CombatUnitHoverEvent>.Unsubscribe(HandleHoverEvent);
        }

        private void HandleHoverEvent(CombatUnitHoverEvent evt)
        {
            if (_myTurnable.Equals(evt.HoveredUnit)) 
                outLineCompo.SetOutLine();
            else
                outLineCompo.ResetOutLine();
        }
    }
}