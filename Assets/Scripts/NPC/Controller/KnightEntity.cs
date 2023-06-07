using System.Collections;
using BattleSystem;
using Core.Enums;
using Core.Services.Updater;
using Core.StatSystem;
using Core.StatSystem.Enums;
using NPC.Behaviour;
using Pathfinding;
using Player;
using UnityEngine;

namespace NPC.Controller
{
  public sealed class KnightEntity : Entity
  {
    private readonly Seeker _seeker;
    private readonly KnightEntityBehaviour _knightEntityBehaviour;
    private Coroutine _coroutine;
    private Vector2 _delta;
    private Collider2D _target;
    private Vector3 _previousTargetPos;
    private Vector3 _destination;
    private bool _isAttack;
    private float _stoppingDistance;
    private Path _path;
    private int _point;
    private float _hp;

    public KnightEntity(KnightEntityBehaviour entityBehaviour, StatsController statsController) : base(entityBehaviour, statsController)
    {
      _seeker = entityBehaviour.GetComponent<Seeker>();
      _knightEntityBehaviour = entityBehaviour;
      _knightEntityBehaviour.AttackSequenceEnded += OnAttackEnded;
      _knightEntityBehaviour.Attacked += OnAttacked;
      VisualiseHp(StatsController.GetStatValue(StatType.Health));
      _hp = StatsController.GetStatValue(StatType.Health);
      _knightEntityBehaviour.DamageTaken += OnDamageTaken;
      _coroutine = ProjectUpdater.Instance.StartCoroutine(Coroutine());
      ProjectUpdater.Instance.FixedUpdateCalled += FixedUpdateCalled;
      var speed = StatsController.GetStatValue(StatType.Speed) * Time.fixedDeltaTime;
      _delta = new Vector2(speed, speed / 2);
      ObjectDied += OnDead;
      SubscribeToKillingQuest(_knightEntityBehaviour.GetPlayer());
    }

    private void OnDamageTaken(float damage)
    {
      damage -= StatsController.GetStatValue(StatType.Defence);
      Debug.Log(damage);
      if (damage < 0)
      {
        return;
      }

      _hp = Mathf.Clamp(_hp - damage, 0, _hp);
      Debug.Log(_hp);
      VisualiseHp(_hp);
      // if (_hp <= 0)
      // {
      //   //OnDead(this);

      // }
    }

    private void OnAttacked(IDamageable target)
    {
      target.TakeDamage(StatsController.GetStatValue(StatType.Damage));
    }

    private void OnDead(Entity entity)
    {
      _knightEntityBehaviour.PlayAnimation(AnimationType.Death, true);
      _knightEntityBehaviour.TriggerDeathCor();
    }

    private IEnumerator Coroutine()
    {
      while (!_isAttack)
      {
        if (GetTarget(out _target))
        {
          //ResetMovement();
          Vector2 position = _target.transform.position;
          _previousTargetPos = position;
          _stoppingDistance = (_target.bounds.size.x + _knightEntityBehaviour.Size.x) / 2;
          var delta = position.x < _knightEntityBehaviour.transform.position.x ? 1 : -1;
          _destination = position + new Vector2(_stoppingDistance * delta, 0);
          _seeker.StartPath(_knightEntityBehaviour.transform.position, _destination, OnPathCalculated);

        }
        /*else if(_target.transform.position != _previousTargetPos)
        {
            Vector2 position = _target.transform.position;
            _previousTargetPos = position;
            _stoppingDistance = (_target.bounds.size.x + _knightEntityBehaviour.Size.x) / 2;
            var delta = position.x < _knightEntityBehaviour.transform.position.x ? 1 : -1;
            _destination = position + new Vector2(_stoppingDistance * delta, 0);
            _seeker.StartPath(_knightEntityBehaviour.transform.position, _destination, OnPathCalculated);
        }*/
        yield return new WaitForSeconds(0.5f);
      }
    }



    private void OnPathCalculated(Path p)
    {
      if (p.error)
        return;

      _path = p;
      _point = 0;
    }

    private void FixedUpdateCalled()
    {
      if (_target == null || _path == null || _isAttack || CheckOnAttack()  /*|| _point >= _path.vectorPath.Count*/)
        return;

      var curPos = _knightEntityBehaviour.transform.position;
      var pointPos = _path.vectorPath[_point];
      var pointDirection = pointPos - curPos;

      if (Vector2.Distance(pointPos, curPos) < 0.05f)
      {
        _point++;
        return;
      }


      if (pointDirection.y != 0)
      {
        pointDirection.y = pointDirection.y > 0 ? 1 : -1;
        var newVerticalPosition = curPos.y + _delta.y * pointDirection.y;
        if (pointDirection.y > 0 && pointPos.y < newVerticalPosition ||
            pointDirection.y < 0 && pointPos.y > newVerticalPosition)
          newVerticalPosition = pointPos.y;


        if (newVerticalPosition != _knightEntityBehaviour.transform.position.y)
        {
          _knightEntityBehaviour.MoveVertically(newVerticalPosition);
          OnVerticalPositionChanged();
        }


      }


      if (pointDirection.x != 0)
      {
        pointDirection.x = pointDirection.x > 0 ? 1 : -1;
        var newHorizontalPosition = curPos.x + _delta.x * pointDirection.x;
        if (pointDirection.x > 0 && pointPos.x < newHorizontalPosition ||
            pointDirection.x < 0 && pointPos.x > newHorizontalPosition)
          newHorizontalPosition = pointPos.x;


        if (newHorizontalPosition != _knightEntityBehaviour.transform.position.x)
        {
          _knightEntityBehaviour.MoveHorizontally(newHorizontalPosition);
          //OnVerticalPositionChanged();
        }
        // _knightEntityBehaviour.MoveHorizontally(newHorizontalPosition);
      }

      if (Vector2.Distance(pointPos, curPos) < 0.05f)
        _point++;
    }

    private bool CheckOnAttack()
    {

      var distanceToObject = _destination - _knightEntityBehaviour.transform.position;

      if (Mathf.Abs(distanceToObject.x) > 0.2f || Mathf.Abs(distanceToObject.y) > 0.2f)
      {
        return false;
      }


      _knightEntityBehaviour.MoveHorizontally(_destination.x);
      _knightEntityBehaviour.MoveVertically(_destination.y);
      _knightEntityBehaviour.SetDirection(_knightEntityBehaviour.transform.position.x > _target.transform.position.x ? Direction.Left : Direction.Right);
      //ResetMovement();
      _isAttack = true;
      _knightEntityBehaviour.StartAttack();
      if (_coroutine != null)
        ProjectUpdater.Instance.StopCoroutine(_coroutine);

      return true;
    }

    private void ResetMovement()
    {
      _target = null;
      _path = null;
      var pos = _knightEntityBehaviour.transform.position;
      _knightEntityBehaviour.MoveVertically(pos.y);
      _knightEntityBehaviour.MoveHorizontally(pos.x);
      _previousTargetPos = Vector2.negativeInfinity;
    }

    private bool GetTarget(out Collider2D target)
    {
      target = Physics2D.OverlapBox(_knightEntityBehaviour.transform.position, _knightEntityBehaviour.SearchBox,
          0, _knightEntityBehaviour.Tartgets);
      return target != null;
    }

    private void OnAttackEnded()
    {
      _isAttack = false;
      /*ProjectUpdater.Instance.Invoke(() =>
      {*/
      _coroutine = ProjectUpdater.Instance.StartCoroutine(Coroutine());
      // }, StatsController.GetStatValue(StatType.AfterAttackDelay));

    }

    protected sealed override void VisualiseHp(float hp)
    {
      if (_knightEntityBehaviour.HpBar.maxValue < hp)
        _knightEntityBehaviour.HpBar.value = hp;

      _knightEntityBehaviour.HpBar.value = hp;
    }
    public void SubscribeToKillingQuest(PlayerEntityBehavior player)
    {
      ObjectDied -= player.activeQuest.goal.EnemyKilled;
      if (player.activeQuest == null || !player.activeQuest.isActive)
        return;
      ObjectDied += player.activeQuest.goal.EnemyKilled;

    }
  }
}