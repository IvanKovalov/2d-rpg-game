using System;
using Core.Enums;
using Core.Movement.Controller;
using Player;
using UnityEngine;

namespace NPC.Behaviour
{
    public class KnightEntityBehaviour : BaseEntityBehaviour
    {
        [SerializeField] private float _afterAttackDelay;
        [SerializeField] private Collider2D _collider2D;
        
        [field: SerializeField] public Vector2 SearchBox { get; private set; }
        [field: SerializeField] public LayerMask Tartgets { get; private set; }

        public Vector2 Size => _collider2D.bounds.size;

        public event Action AttackSequenceEnded;

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, SearchBox);
        }

        public override void Initialize()
        {
            base.Initialize();
            DirectionalMover = new PositionMover(Rigidbody);
        }

        private void Update() => UpdateAnimations();
        public void StartAttack()
        {
            if (!PlayAnimation(AnimationType.Attack, true))
            {
                return;
            }
            ActionRequested += Attack;
            AnimationEnded += EndAttack;
        } //Animator.SetAnimationState(AnimationType.Attack, true, Attack, EndAttack);

        public void SetDirection(Direction direction) => DirectionalMover.SetDirection(direction);

        private void Attack()
        {
            Debug.Log("Attack");
        }

        private void EndAttack()
        {
            ActionRequested -= Attack;
            AnimationEnded -= EndAttack;
            PlayAnimation(AnimationType.Attack, false);
            PlayAnimation(AnimationType.Idle);
            Invoke(nameof(EndAttackSequence), _afterAttackDelay);
        }

        private void EndAttackSequence()
        {
            AttackSequenceEnded?.Invoke();
        }

      
        
        
    }
}