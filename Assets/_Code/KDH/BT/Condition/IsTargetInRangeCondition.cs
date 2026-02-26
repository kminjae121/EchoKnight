using System;
using Code.Managers;
using Unity.Behavior;
using UnityEngine;
using Code.Map;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Is Target In Range", story: "[Target] is within [Range] of [Agent]", category: "Conditions", id: "6a99f9edf7e2ac183289331f8c48156d")]
public partial class IsTargetInRangeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> Range;
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    public override bool IsTrue()
    {
        if (Target.Value == null || Agent.Value == null)
        {
            return false;
        }
        
        var gridMap = GameObject.FindAnyObjectByType<GridMap>();
        if (gridMap != null)
        {
            Vector2Int agentGrid = gridMap.WorldToGridPosition(Agent.Value.transform.position);
            Vector2Int targetGrid = gridMap.WorldToGridPosition(Target.Value.transform.position);
            
            float gridDistance = Vector2.Distance(agentGrid, targetGrid);
            return gridDistance <= Range.Value;
        }
        
        Vector3 agentPos = Agent.Value.transform.position;
        Vector3 targetPos = Target.Value.transform.position;
        agentPos.y = 0f;
        targetPos.y = 0f;

        float distance = Vector3.Distance(agentPos, targetPos);
        return distance <= (Range.Value * 3.18f + 0.5f);
    }
}