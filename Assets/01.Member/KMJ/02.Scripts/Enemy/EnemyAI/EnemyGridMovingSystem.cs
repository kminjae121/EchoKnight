using UnityEngine;

namespace EnemySystem
{
    public enum Dir
    {
        Left = 1,
        Right = 2,
        Up = 3,
        Down = 4,
    }
    public class EnemyGridMovingSystem : MonoBehaviour
    {
        private bool _isMoveToTarget;
    
        public void Move()
        {
            if (_isMoveToTarget == true)
            {
                MoveToTarget();
            }
            else
            {
                MoveToRandomGrid();
            }
        }

        private void MoveToTarget()
        {
            Debug.Log("플레이어 방향으로 움직임");
        }


        private void MoveToRandomGrid()
        {
            int randomDir = Random.Range(0, 4);
        
            Dir dir = (Dir)randomDir;
        
            Move(dir);
        }

        private void Move(Dir dir)
        {
            switch (dir)
            {
                case Dir.Left:
                    Debug.Log("왼쪽으로 움직임");
                    break;
                case Dir.Right:
                    Debug.Log("오른쪽으로 움직임");
                    break;
                case Dir.Up:
                    Debug.Log("위로 움직임");
                    break;
                case Dir.Down:
                    Debug.Log("아래쪽으로 움직임");
                    break;
            }
        }
    }
}