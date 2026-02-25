using System;
using EnemySystem;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Enemy Smart Move To Target", story: "[Agent] smartly moves towards [Target] optimal range [OptimalRange]", category: "Action", id: "fae195f5aaf05b504e77f14d366dc7bc")]
public partial class EnemyMoveAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    [SerializeReference] public BlackboardVariable<int> MaxMoveStep;
    [SerializeReference] public BlackboardVariable<float> OptimalRange;
    [SerializeReference] public BlackboardVariable<float> TileSize;

    private EnemyUnit _enemyUnit;
    private bool _isMoving;

    protected override Status OnStart()
    {
        if (Agent.Value == null || Target.Value == null) return Status.Failure;
        
        _enemyUnit = Agent.Value.GetComponent<EnemyUnit>();
        if (_enemyUnit == null) return Status.Failure;

        Vector3 myPos = Agent.Value.transform.position;
        Vector3 targetPos = Target.Value.transform.position;

        Vector3 bestPosition = targetPos;
        float bestScore = float.MinValue;

        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            Vector3 candidatePos = targetPos + dir * (OptimalRange.Value * TileSize.Value);

            if (NavMesh.SamplePosition(candidatePos, out NavMeshHit hit, TileSize.Value, NavMesh.AllAreas))
            {
                float score = 0;

                NavMeshPath path = new NavMeshPath();
                if (NavMesh.CalculatePath(myPos, hit.position, NavMesh.AllAreas, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    float pathDist = GetPathLength(path);
                    if (pathDist <= (MaxMoveStep.Value * TileSize.Value))
                    {
                        score += 100f;
                    }
                    else continue;
                }
                else continue;
                
                if (!Physics.Linecast(hit.position + Vector3.up * 5f, targetPos + Vector3.up * 5f, LayerMask.GetMask("Obstacle")))
                {
                    score += 50f;
                }

                if (score > bestScore)
                {
                    bestScore = score;
                    bestPosition = hit.position;
                }
            }
        }

        _isMoving = true;

        _enemyUnit.OrderMove(bestPosition, MaxMoveStep.Value, OnDone);
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_isMoving) return Status.Running;
        return Status.Success;
    }

    private void OnDone() { _isMoving = false; }

    private float GetPathLength(NavMeshPath path)
    {
        float length = 0.0f;
        if (path.corners.Length < 2) return length;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            length += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }
        return length;
    }
}