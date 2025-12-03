using EnemySystem;
using EntityComponent;
using UnitSystem;
using UnityEngine;

public class Enemy : Unit
{

   
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public void TestDealth()
    {
        gameObject.SetActive(false);
    }
}
