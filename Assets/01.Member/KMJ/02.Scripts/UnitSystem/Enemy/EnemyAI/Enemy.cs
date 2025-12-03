using EnemySystem;
using EntityComponent;
using UnitSystem;
using UnityEngine;

public class Enemy : Unit
{

    
    protected override void Awake()
    {
        base.Awake();
        //on.AddListener(TestDealth);
        
    }

    public void TestDealth()
    {
        gameObject.SetActive(false);
    }
}
