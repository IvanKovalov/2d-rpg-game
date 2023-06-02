using System.Collections;
using Core.Enums;
using Core.Services.Updater;
using Core.StatSystem;
using Core.StatSystem.Enums;
using NPC.Behaviour;
using Pathfinding;
using UnityEngine;

namespace NPC.Controller
{
    public class KnightEntity : Entity
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


        public KnightEntity(KnightEntityBehaviour entityBehaviour, StatsController statsController) : base(entityBehaviour, statsController)
        {
            _seeker = entityBehaviour.GetComponent<Seeker>();
            _knightEntityBehaviour = entityBehaviour;
            _knightEntityBehaviour.AttackSequenceEnded += OnAttackEnded;
            _coroutine = ProjectUpdater.Instance.StartCoroutine(Coroutine());
            ProjectUpdater.Instance.FixedUpdateCalled += FixedUpdateCalled;
            var speed = StatsController.GetStatValue(StatType.Speed) * Time.fixedDeltaTime;
                    _delta = new Vector2(speed, speed / 2);
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
            if(p.error)
                return;

            _path = p;
            _point = 0;
        }

        private void FixedUpdateCalled()
        {
            if(CheckOnAttack() || _target == null || _path == null || _isAttack  /*|| _point >= _path.vectorPath.Count*/)
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
            if(_coroutine != null)
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
            _coroutine = ProjectUpdater.Instance.StartCoroutine(Coroutine());
        }
    }
}