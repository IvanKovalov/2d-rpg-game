using System;
using System.Collections;
using BattleSystem;
using Core.Movement.Controller;
using Core.Movement.Data;
using Core.StatSystem;
using Drawing;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using QuestSystem;
using AnimatorController = Core.Animations.AnimatorController;

namespace Player
{
  [RequireComponent(typeof(Rigidbody2D))]
  public class PlayerEntityBehavior : MonoBehaviour, ILevelGraphicElement, IDamageable
  {
    [SerializeField] private Animator _animator;

    [SerializeField] private JumpData _jumpData;
    [SerializeField] private SortingGroup _sortingGroup;
    [field: SerializeField] public Slider HpBar { get; private set; }
    [field: SerializeField] public Attacker Attacker { get; private set; }

    private Rigidbody2D _rigidbody;
    private bool _isJump;
    private float _startJumpVerticalPosition;

    private DirectionalMover _directionalMover;
    private Jumper _jumper;
    private AnimationType _currenAnimationtype;
    private bool _isDead = false;

    public float VerticalPosition => _rigidbody.position.y;

    public Quest activeQuest;

    public event Action ActionRequested;
    public event Action AnimationEnded;
    public event Action<float> DamageTaken;
    public event Action<ILevelGraphicElement> VerticalPositionChanged;
    // Start is called before the first frame update
    public void Initialize(IStatValueGiver statValueGiver)
    {
      _rigidbody = GetComponent<Rigidbody2D>();
      _directionalMover = new DirectionalMover(_rigidbody, statValueGiver);
      _jumper = new Jumper(_rigidbody, _jumpData, statValueGiver);

    }
    public void MoveHorizontally(float direction) => _directionalMover.MoveHorizontally(direction);

    public void MoveVertically(float direction)
    {

      if (_jumper.isJump)
      {
        return;
      }

      _directionalMover.MoveVertically(direction);

      if (direction != 0)
      {
        VerticalPositionChanged?.Invoke(this);
      }

    }

    public void Jump() => _jumper.Jump();

    public bool PlayAnimation(AnimationType animationType, bool active)
    {
      if (!active)
      {
        if (_currenAnimationtype == AnimationType.Idle || _currenAnimationtype != animationType)
        {
          return false;
        }
        _currenAnimationtype = AnimationType.Idle;
        PlayAnimation(_currenAnimationtype);
        return false;
      }

      if (_currenAnimationtype > animationType)
      {
        return false;
      }
      //Debug.Log("should be good");
      _currenAnimationtype = animationType;
      PlayAnimation(_currenAnimationtype);
      return true;
    }

    public void SetDrawingOrder(int order) => _sortingGroup.sortingOrder = order;


    public void SetSize(Vector2 size)
    {
      Vector2 newSize = new Vector2((float)(size.x * 0.3), (float)(size.y * 0.3));
      transform.localScale = newSize;
    }

    public void SetVerticalPosition(float verticalPosition) =>
        _rigidbody.position = new Vector2(_rigidbody.position.x, verticalPosition);


    protected void PlayAnimation(AnimationType animationType)
    {
      _animator.SetInteger(nameof(AnimationType), (int)animationType);
    }

    public void StartAttck()
    {
      if (!PlayAnimation(AnimationType.Attack, true))
      {
        return;
      }
      ActionRequested += Attack;
      AnimationEnded += EndAttack;
    }

    protected void OnActionRequested() => ActionRequested?.Invoke();
    protected void OnAnimationEnded() => AnimationEnded?.Invoke();

    private void Attack()
    {
      Debug.Log("Attack");
    }

    private void EndAttack()
    {
      ActionRequested -= Attack;
      AnimationEnded -= EndAttack;
      PlayAnimation(AnimationType.Attack, false);
    }
    private IEnumerator Death()
    {
      _isDead = true;
      yield return new WaitForSeconds(1f);
      Destroy(gameObject);
    }
    public void TriggerDeathCor()
    {
      StartCoroutine(Death());
    }

    private void Update()
    {
      if (_jumper.isJump)
      {
        _jumper.UpdateJump();
      }
      UpdateAnimations();
    }

    private void UpdateAnimations()
    {
      PlayAnimation(AnimationType.Idle, true);
      PlayAnimation(AnimationType.Walk, /*_movement.magnitude > 0*/ _directionalMover.IsMoving);
      PlayAnimation(AnimationType.Jump, /*_isJump*/ _jumper.isJump);
      /*PlayAnimation(AnimationType.Idle, true);
      PlayAnimation(AnimationType.Walk, _movement.magnitude > 0);
      PlayAnimation(AnimationType.Jump, _isJump);*/
    }

    public void TakeDamage(float damage)
    {
      DamageTaken?.Invoke(damage);
    }

    public bool IsDead()
    {
      return _isDead;
    }
  }
}

