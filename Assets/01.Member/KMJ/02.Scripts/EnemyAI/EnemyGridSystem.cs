using NUnit.Framework.Interfaces;
using UnityEngine;

public enum Dir
{
    left = 1,
    right = 2,
    up = 3,
    down = 4,
}
public class EnemyGridSystem : MonoBehaviour
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
            case Dir.left:
                Debug.Log("왼쪽으로 움직임");
                break;
            case Dir.right:
                Debug.Log("오른쪽으로 움직임");
                break;
            case Dir.up:
                Debug.Log("위로 움직임");
                break;
            case Dir.down:
                Debug.Log("아래쪽으로 움직임");
                break;
        }
    }
    
    
}
