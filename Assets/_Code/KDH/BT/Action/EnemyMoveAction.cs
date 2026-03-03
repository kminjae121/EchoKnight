using System;
using System.Collections.Generic;
using Code.EntityComponent;
using Code.Map;
using EnemySystem;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Enemy Smart Move To Target", story: "[Agent] smartly moves considering HP towards [Target]", category: "Action", id: "fae195f5aaf05b504e77f14d366dc7bc")]
public partial class EnemyMoveAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    
    [SerializeReference] public BlackboardVariable<int> MaxMoveStep; 
    [SerializeReference] public BlackboardVariable<float> OptimalRange = new BlackboardVariable<float>(5f); 
    [SerializeReference] public BlackboardVariable<float> TileSize = new BlackboardVariable<float>(3.18f);

    private EnemyUnit _enemyUnit;
    private bool _isMoving;

    protected override Status OnStart()
    {
        if (Agent.Value == null || Target.Value == null) return Status.Failure;
        
        _enemyUnit = Agent.Value.GetComponent<EnemyUnit>();
        if (_enemyUnit == null) return Status.Failure;

        var healthCompo = Agent.Value.GetComponent<EntityHealth>();
        bool isLowHealth = healthCompo != null && (healthCompo.CurrentHealth / healthCompo.MaxHealth) <= 0.3f;

        Vector3 myPos = Agent.Value.transform.position;
        Vector3 targetPos = Target.Value.transform.position;
        float tileSize = TileSize.Value;

        var mover = _enemyUnit.GetComponent<EnemyGridMovingSystem>();

        Vector3 bestDest = targetPos; 
        float bestScore = float.MinValue;
        bool foundValidSpot = false;
        List<Vector3> checkedTiles = new List<Vector3>();

        Vector3 validStartPos = myPos;
        if (NavMesh.SamplePosition(myPos, out NavMeshHit startHit, tileSize * 2f, NavMesh.AllAreas))
        {
            validStartPos = startHit.position;
        }

        for (int i = 0; i < 16; i++)
        {
            float angle = i * 22.5f;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            
            float searchRadius = isLowHealth ? MaxMoveStep.Value * tileSize : OptimalRange.Value * tileSize;
            Vector3 candidatePos = targetPos + (dir * searchRadius);

            if (mover != null) candidatePos = mover.GetExactTileCenter(candidatePos);

            if (checkedTiles.Contains(candidatePos)) continue; 
            checkedTiles.Add(candidatePos);

            if (NavMesh.SamplePosition(candidatePos, out NavMeshHit hit, tileSize, NavMesh.AllAreas))
            {
                float score = 0;
                
                NavMeshPath path = new NavMeshPath();
                NavMesh.CalculatePath(validStartPos, hit.position, NavMesh.AllAreas, path);
                
                if (path.status == NavMeshPathStatus.PathInvalid) continue;

                float distFromMeToDest = GetPathLength(path);
                float distFromDestToTarget = Vector3.Distance(hit.position, targetPos);
                float maxMoveDist = MaxMoveStep.Value * tileSize;

                if (distFromMeToDest > maxMoveDist) 
                {
                    score -= distFromMeToDest;
                }
                else
                {
                    score += 1000f;
                    
                    if (isLowHealth)
                    {
                        score += (distFromDestToTarget * 10f);
                    }
                    else
                    {
                        if (distFromDestToTarget <= (OptimalRange.Value * tileSize))
                        {
                            score += 500f; 
                            
                            Vector3 origin = hit.position + Vector3.up * 1.5f;
                            Vector3 dest = targetPos + Vector3.up * 1.5f;
                            if (!Physics.Linecast(origin, dest, LayerMask.GetMask("Obstacle", "Wall")))
                            {
                                score += 300f; 
                            }
                        }
                        else
                        {
                            score -= (distFromDestToTarget * 50f);
                        }
                    }
                }

                if (score > bestScore)
                {
                    bestScore = score;
                    bestDest = hit.position;
                    foundValidSpot = true;
                }
            }
        }

        if (!foundValidSpot) bestDest = targetPos;

        _isMoving = true;
        _enemyUnit.OrderMove(bestDest, MaxMoveStep.Value, OnDone);
        
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