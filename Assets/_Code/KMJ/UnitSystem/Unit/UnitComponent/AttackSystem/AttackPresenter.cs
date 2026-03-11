using System;
using Code.Core.Events.Bus;
using Code.EntityComponent;
using Code.UnitSystem;
using UnitSystem;
using UnityEngine;

namespace Code.AttackSystem
{
    public class AttackPresenter : MonoBehaviour
    {
        public void ShowAttackUI(bool isLock)
        {
            Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(isLock));
        }

        public void ShowWarning(string message)
        {
            Bus<WarningUIEvent>.Raise(new WarningUIEvent(message));
        }

        public void OnAttackExecuted()
        {
            Bus<UnitAttackControlEvent>.Raise(new UnitAttackControlEvent(true));

            Bus<EnemyHpInfo>.Raise(new EnemyHpInfo(
                0, 0, 0, 0, false, null, true));
        }

        public void SetCamTarget(GameObject attacker)
        {
            Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(
                attacker, true, new Vector3(0.1f, 0.1f, 0.1f)));
        }
    }
}