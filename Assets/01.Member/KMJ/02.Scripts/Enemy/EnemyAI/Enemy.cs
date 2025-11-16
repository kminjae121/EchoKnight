using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool isDead = false;
    private bool _isMyTurn = false;
    public void Die()
    {
        gameObject.SetActive(false);
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
