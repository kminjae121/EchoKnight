namespace Code.UnitSystem.Combat
{
    public class NormalShootItem : ShootItem
    {
        public override void GiveDamage()
        {
            gameObject.SetActive(false);
        }
    }
}