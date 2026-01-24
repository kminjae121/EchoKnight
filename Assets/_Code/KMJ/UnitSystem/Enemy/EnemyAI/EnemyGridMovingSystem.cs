using System.Collections;
using _01.Member.KMJ._02.Scripts.UnitSystem.Unit.UnitComponent;
using Code.Core.Interfaces;
using Code.UnitSystem;
using UnitSystem;
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

        [SerializeField] private LayerMask whatIsGround;

        [SerializeField] private Transform owner;

        public UnityEvent OnMoveEndEvent = new UnityEvent();

        private GameObject _ownTrm;
        
        private void Awake()
        {
            if (OnMoveEndEvent == null)
                OnMoveEndEvent = new UnityEvent();
        }
    
        public void Initialize(Unit owner)
        {
            
        }
        
        public void Move()
        {
            MoveToRandomGrid();
        }


        private void MoveToRandomGrid()
        {
            int randomDir = Random.Range(1, 5);
            
            Moveing(randomDir);
        }
        
        public void MoveTowardsTarget(Vector3 targetPos)
        {
            Vector3 dir = (targetPos - transform.position).normalized;
            Vector3 step = Vector3.zero;
            
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
                step = new Vector3(Mathf.Sign(dir.x), 0, 0);
            else
                step = new Vector3(0, 0, Mathf.Sign(dir.z));

            StartCoroutine(Move(step));
        }

        private void Moveing(int dir)
        {
            MoveTo(dir);
        }

        private void MoveTo(int dirInt)
        {
            Vector3 step = dirInt switch
            {
                1 => new Vector3(-1f, 0f, 0f), 
                2 => new Vector3( 1f, 0f, 0f), 
                3 => new Vector3( 0f, 0f, 1f), 
                4 => new Vector3( 0f, 0f,-1f), 
                _ => Vector3.zero
            };

            if (step == Vector3.zero) return;
            
            StartCoroutine(Move(step));
        }

        private IEnumerator Move(Vector3 step)
        {
            Vector3 target = owner.position + step;
            
            if(Physics.Raycast(target, Vector3.down, out RaycastHit hit, Mathf.Infinity, whatIsGround))
            {
                if (hit.transform.TryGetComponent(out IMapTile checkTile))
                {
                    if (checkTile.HasObstacle)
                    {
                        OnMoveEndEvent?.Invoke();
                        yield break; 
                    }
                }
                
                if (_ownTrm != null)
                {
                    _ownTrm.GetComponent<IMapTile>().SetObstacle(false);
                }
            
                rotationCompo.SetDir(target);
                while ((owner.position - target).sqrMagnitude > 0.0001f)
                {
                    owner.position = Vector3.MoveTowards(owner.position, target, 2 * Time.deltaTime);
                    yield return null;
                }
                owner.position = target;
        
                if (hit.transform.TryGetComponent(out IMapTile tile))
                {
                    tile.SetObstacle(true);
                    _ownTrm = hit.transform.gameObject;
                }
    
                OnMoveEndEvent?.Invoke();   
            }
            else
            {
                OnMoveEndEvent?.Invoke();
                yield break; 
            }
        }
    }
}