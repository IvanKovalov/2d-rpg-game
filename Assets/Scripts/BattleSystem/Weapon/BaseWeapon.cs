namespace BattleSystem.Weapon
{
    public abstract class BaseWeapon
    {
        public abstract void Attack(float damage);
        public abstract void EndAttack();
    }
}