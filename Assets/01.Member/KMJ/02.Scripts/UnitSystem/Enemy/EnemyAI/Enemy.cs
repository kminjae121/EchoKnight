using EntityComponent;
using UnitSystem;
using UnityEngine;

public class Enemy : Entity
{
    protected override void Awake()
    {
        base.Awake();
        OnDeathEvent.AddListener(TestDealth);
    }

    public void TestDealth()
    {
        gameObject.SetActive(false);
    }
}
