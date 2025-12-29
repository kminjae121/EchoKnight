using System.Collections;
using _01.Member.KMJ._02.Scripts.UnitSystem.Unit.UnitComponent;
using Code.Core.Interfaces;
using UnitSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Unit = Code.UnitSystem.Unit;

namespace EnemySystem
{
    public class EnemyGridMovingSystem : MonoBehaviour
    {
        private bool _isMoveToTarget = false;
        [SerializeField] private UnitRotation rotationCompo;
        [SerializeField] private UnitAnimation animationCompo;

        public UnityEvent OnMoveEndEvent;
    
        public void Initialize(Unit owner)
        {
            
        }
        
        public void Move()
        {
                MoveToRandomGrid();
        }

        private void MoveToTarget()
        {
            Debug.Log("플레이어 방향으로 움직임");
        }


        private void MoveToRandomGrid()
        {
            int randomDir = Random.Range(1, 5);
            Debug.Log(randomDir);
            
            Moveing(randomDir);
        }

        private void Moveing(int dir)
        {
            MoveTo(dir);
        }

        private void MoveTo(int dirInt)
        {
            Debug.Log("움직임");
            
            switch (dirInt)
            {
                case 1:
                    StartCoroutine(Move(Vector3.forward));
                    break;
                case 2:
                    StartCoroutine(Move(Vector3.back));
                    break;
                case 3:
                    StartCoroutine(Move(Vector3.right));
                    break;
                case 4:
                    StartCoroutine(Move(Vector3.left));
                    break;
            }
        }

        private IEnumerator Move(Vector3 dir)
        {
            Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit,100);

            hit.transform.TryGetComponent(out IMapTile maptile);
            maptile.SetObstacle(false);
            
            Physics.Raycast(transform.position, dir, out RaycastHit hits, 0.5f);

            if (hits.transform.TryGetComponent(out IMapTile tile))
            {
                rotationCompo.SetDir(hits.transform.position);
                if (!tile.HasObstacle)
                {
                    animationCompo.PlaySelectAnimation("MOVE");   
                    while (Vector3.Distance(transform.position, hits.transform.position) > 0.01f)
                    {
                        transform.position = Vector3.MoveTowards(
                            transform.position,
                            hits.transform.position,
                            2 * Time.deltaTime
                        );
                        yield return null;
                    }
                
                    Physics.Raycast(transform.position, Vector3.down, out RaycastHit hittor, 100);

                    hittor.transform.TryGetComponent(out IMapTile maptiles);
                    Debug.Log(hittor.transform.name);
                    maptiles.SetObstacle(true);
                
                    OnMoveEndEvent.Invoke();
                    
                    animationCompo.PlaySelectAnimation("IDLE");  
                }
                else
                {
                    MoveToRandomGrid();
                }
            }
        }
    }
}