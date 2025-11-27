using EnemySystem;
using EntityComponent;
using UnitSystem;
using UnityEngine;

public class Enemy : Entity
{
    
    public float turnSpeed { get; set; }

    public bool isPlayerUnit { get; set; } = false;

    public float turnGauge { get; set; }

    public EnemySO enemySO;
    
    protected override void Awake()
    {
        base.Awake();
        OnDeathEvent.AddListener(TestDealth);

        turnSpeed = enemySO.turnSpeed;
        isPlayerUnit = enemySO.isPlayerUnit;
        turnGauge = enemySO.turnGauge;
        
    }

    public void TestDealth()
    {
        gameObject.SetActive(false);
    }
}
