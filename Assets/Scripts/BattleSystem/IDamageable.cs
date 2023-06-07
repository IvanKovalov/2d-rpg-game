using System;

namespace BattleSystem
{
    public interface IDamageable
    {
        event Action<float> DamageTaken;
        void TakeDamage(float damage);
        bool IsDead();
    }
}