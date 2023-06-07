using System;
using BattleSystem;
using Core.Animations;
using Core.Movement.Controller;
using Drawing;
using Player;
using UnityEngine;
using UnityEngine.Rendering;

namespace NPC.Behaviour
{
  public class BaseEntityBehaviour : MonoBehaviour, IDamageable
  {
    [SerializeField] protected Animator Animator;
    [SerializeField] private SortingGroup _sortingGroup;

    protected Rigidbody2D Rigidbody;
    protected NPCDirectionalMover DirectionalMover;
    private AnimationType _currentAnimationType;
    protected bool _isDead = false;

    public event Action ActionRequested;
    public event Action AnimationEnded;

    private Action _animationAction;
    private Action _animationEndAction;
    public event Action<ILevelGraphicElement> VerticalPositionChanged;
    public event Action<float> DamageTaken;

    public virtual void Initialize()
    {
      Rigidbody = GetComponent<Rigidbody2D>();
    }

    public float VerticalPosition => transform.position.y;

    public void SetDrawingOrder(int order) => _sortingGroup.sortingOrder = order;
    public void SetSize(Vector2 size) => transform.localScale = size;
    public void MoveHorizontally(float direction) => DirectionalMover.MoveHorizontally(direction);
    public virtual void MoveVertically(float direction) => DirectionalMover.MoveVertically(direction);

    public void SetVerticalPosition(float verticalPosition) =>
        Rigidbody.position = new Vector2(Rigidbody.position.x, verticalPosition);

    public void TakeDamage(float damage)
    {
      DamageTaken?.Invoke(damage);
    }

    public bool IsDead()
    {
      return _isDead;
    }

    protected virtual void UpdateAnimations()
    {
      PlayAnimation(AnimationType.Idle, true);
      PlayAnimation(AnimationType.Walk, DirectionalMover.IsMoving);
    }


    public bool SetAnimationState(AnimationType animationType, bool active, Action animationAction = null, Action endAnimationAction = null)
    {
      if (!active)
      {
        if (_currentAnimationType == AnimationType.Idle || _currentAnimationType != animationType)
          return false;
        _animationAction = null;
        _animationEndAction = null;
        OnAnimationEnded();
        return false;
      }

      if (_currentAnimationType >= animationType)
        return false;

      _animationAction = animationAction;
      _animationEndAction = endAnimationAction;
      SetAnimation(animationType);
      return true;
    }


    private void SetAnimation(AnimationType animationType)
    {
      _currentAnimationType = animationType;
      PlayAnimation(_currentAnimationType);
    }

    public bool PlayAnimation(AnimationType animationType, bool active)
    {
      if (!active)
      {
        if (_currentAnimationType == AnimationType.Idle || _currentAnimationType != animationType)
        {
          return false;
        }
        _currentAnimationType = AnimationType.Idle;
        PlayAnimation(_currentAnimationType);
        return false;
      }

      if (_currentAnimationType > animationType)
      {
        return false;
      }

      _currentAnimationType = animationType;
      PlayAnimation(_currentAnimationType);
      return true;
    }

    protected void PlayAnimation(AnimationType animationType)
    {
      if(Animator != null)
        Animator.SetInteger(nameof(AnimationType), (int)animationType);
    }

    protected void OnActionRequested() => ActionRequested?.Invoke();
    protected void OnAnimationEnded()
    {
      _animationEndAction?.Invoke();
      SetAnimation(AnimationType.Idle);
    }
  }
}