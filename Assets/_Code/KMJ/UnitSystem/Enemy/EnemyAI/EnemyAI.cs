using Unity.Behavior;
using UnityEngine;

namespace EnemySystem
{
    [RequireComponent(typeof(BehaviorGraphAgent))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] private BehaviorGraphAgent _agent;

        private void Awake()
        {
            if (_agent == null) _agent = GetComponent<BehaviorGraphAgent>();
        }

        public void SetTurnState(bool isMyTurn)
        {
            if (_agent != null && _agent.BlackboardReference != null)
            {
                _agent.BlackboardReference.SetVariableValue("IsMyTurn", isMyTurn);
            }
        }

        public void SetTarget(GameObject target)
        {
            if (_agent != null && _agent.BlackboardReference != null)
            {
                _agent.BlackboardReference.SetVariableValue("Target", target);
            }
        }
    }
}