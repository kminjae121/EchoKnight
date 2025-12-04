using Code.Core.Interfaces;
using Code.UnitSystem;
using UnitSystem;

public class Enemy : Unit, ITurnable
{
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void Dead()
    {
        TestDeath();
        base.Dead();
    }

    public void TestDeath()
    {
        gameObject.SetActive(false);
    }
}
