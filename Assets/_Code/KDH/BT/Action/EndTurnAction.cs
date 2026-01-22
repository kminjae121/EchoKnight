using System;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "End Turn", story: "[Agent] ends turn", category: "Action", id: "c12108ed31ee858096d57d096d28a50a")]
public partial class EndTurnAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<bool> IsMyTurn;

    protected override Status OnStart()
    {
        IsMyTurn.Value = false; // BT 정지
        var unit = Agent.Value.GetComponent<Unit>();
        Bus<UnitTurnEndEvent>.Raise(new UnitTurnEndEvent(unit));
        return Status.Success;
    }
}

