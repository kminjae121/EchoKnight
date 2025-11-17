using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool isDead = false;
    private bool _isMyTurn = false;
    public void Die()
    {
        Debug.Log("죽는 애니메이션 실행");
    }
    
    public void Hit()
    {
        if (isDead)
            return;
    }

    public bool IsMyTurn()
    {
        return _isMyTurn;
    }
}
