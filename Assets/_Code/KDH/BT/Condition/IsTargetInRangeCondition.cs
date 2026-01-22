using System;
using Code.Managers;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Is Target In Range", story: "[Target] is within [Range] of [Agent]", category: "Conditions", id: "6a99f9edf7e2ac183289331f8c48156d")]
public partial class IsTargetInRangeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> Range;
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    public override bool IsTrue()
    {
        if (Target.Value == null || Agent.Value == null) return false;

        float distance = Vector3.Distance(Agent.Value.transform.position, Target.Value.transform.position);

        return distance <= Range.Value;
    }
}
