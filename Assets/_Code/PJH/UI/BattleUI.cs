using Code.Core.Events.Bus;
using UnityEngine;

namespace Code.UI
{
    public class BattleUI : MonoBehaviour
    {
        private void Awake()
        {
            Bus<SetAtkUIEvent>.Subscribe(SetAttackUI);
        }

        private void OnDestroy()
        {
            Bus<SetAtkUIEvent>.Unsubscribe(SetAttackUI);
        }

        private void SetAttackUI(SetAtkUIEvent evt)
        {
            
        }
    }
}