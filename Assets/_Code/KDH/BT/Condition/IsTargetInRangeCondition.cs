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
        if (Agent.Value == null) return false;

        var unitManager = UnityEngine.Object.FindAnyObjectByType<UnitManager>();
        if (unitManager == null) return false;

        var playerUnits = unitManager.GetPlayerUnits();
        GameObject closest = null;
        float minDst = float.MaxValue;

        // 가장 가까운 플레이어 탐색
        foreach (var unit in playerUnits)
        {
            float dst = Vector3.Distance(Agent.Value.transform.position, unit.transform.position);
            if (dst <= Range.Value && dst < minDst)
            {
                minDst = dst;
                closest = unit.gameObject;
            }
        }

        if (closest != null)
        {
            Target.Value = closest; // 찾은 적 할당
            return true;
        }
        return false;
    }
}
