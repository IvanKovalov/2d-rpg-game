using Unity.VisualScripting;
using UnityEngine;

namespace BattleSystem.Weapon
{
    public class SingleTargetWeapon : BaseWeapon
    {
        private readonly Attacker _attacker;
        private float _damage;
        public SingleTargetWeapon(Attacker attacker)
        {
            
            //_attacker.TryGetComponent(out PolygonCollider2D collider);
            //collider.isTrigger = true;
            _attacker = attacker;
        }
        public override void Attack(float damage)
        {
           
            _attacker.gameObject.SetActive(true);
            _damage = damage;
        }

        public override void EndAttack()
        {
            
            if(_attacker.Targets.Count < 1)
                return;
            
            var target = _attacker.Targets.Find(target => target != null && !target.IsDead());
            //_attacker.Targets[0].TakeDamage(_damage);
            target.TakeDamage(_damage);
        }
    }
}