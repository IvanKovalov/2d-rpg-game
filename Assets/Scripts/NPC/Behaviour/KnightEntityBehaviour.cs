using System;
using System.Collections;
using BattleSystem;
using Core.Enums;
using Core.Movement.Controller;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace NPC.Behaviour
{
  public class KnightEntityBehaviour : BaseEntityBehaviour
  {
    [SerializeField] private float _afterAttackDelay;
    [SerializeField] private Collider2D _collider2D;
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private float _attackRadius;

    [field: SerializeField] public Vector2 SearchBox { get; private set; }
    [field: SerializeField] public LayerMask Tartgets { get; private set; }
    [field: SerializeField] public Slider HpBar { get; private set; }

    public Vector2 Size => _collider2D.bounds.size;

    public event Action AttackSequenceEnded;
    public event Action<IDamageable> Attacked;

    private void OnDrawGizmos()
    {
      Gizmos.DrawWireCube(transform.position, SearchBox);
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(_attackPoint.position, _attackRadius);
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
      var targetCollider = Physics2D.OverlapCircle(_attackPoint.position, _attackRadius, Tartgets);
      if (targetCollider != null && targetCollider.TryGetComponent(out IDamageable damageable))
        Attacked?.Invoke(damageable);
    }

    private void EndAttack()
    {
      ActionRequested -= Attack;
      AnimationEnded -= EndAttack;
      PlayAnimation(AnimationType.Attack, false);
      PlayAnimation(AnimationType.Idle);
      Invoke(nameof(EndAttackSequence), _afterAttackDelay);
      //AttackSequenceEnded?.Invoke();
    }

    private void EndAttackSequence()
    {
      AttackSequenceEnded?.Invoke();
    }
    private IEnumerator Death()
    {
      yield return new WaitForSeconds(1f);
      Destroy(gameObject);
    }
    public void TriggerDeathCor()
    {
      StartCoroutine(Death());
    }
  }
}