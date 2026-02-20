using System;
using System.Linq;
using Code.Managers;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Find Nearest Target", story: "[Agent] finds nearest target to [Target]", category: "Action", id: "860f39d43d067bb1c9305d0c28d8b5a2")]
public partial class FindNearestTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    protected override Status OnStart()
    {
        if (Agent.Value == null) return Status.Failure;

        var unitManager = UnityEngine.Object.FindAnyObjectByType<UnitManager>();
        if (unitManager == null) return Status.Failure;

        var playerUnits = unitManager.GetPlayerUnits();
        if (playerUnits == null || playerUnits.Count() == 0)
        {
            Target.Value = null;
            return Status.Failure;
        }

        GameObject closestUnit = null;
        float minDistance = float.MaxValue;
        Vector3 myPos = Agent.Value.transform.position;

        foreach (var unit in playerUnits)
        {
            if (unit == null || !unit.gameObject.activeInHierarchy) continue;

            float dist = Vector3.SqrMagnitude(unit.transform.position - myPos);
            if (dist < minDistance)
            {
                minDistance = dist;
                closestUnit = unit.gameObject;
            }
        }

        Target.Value = closestUnit;

        return closestUnit != null ? Status.Success : Status.Failure;
    }
}

