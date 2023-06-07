using Core.Movement.Data;
using Core.StatSystem;
using Core.StatSystem.Enums;
using Core.Tools;
using UnityEngine;

namespace Core.Movement.Controller
{
    public class DirectionalMover
    {
        private readonly Rigidbody2D _rigidbody;
        private readonly Transform _transform;
        private readonly IStatValueGiver _statValueGiver;

        private Vector2 _movement;

        public bool IsMoving => _movement.magnitude > 0;
        public Direction Direction { get; private set; }

        public DirectionalMover(Rigidbody2D rigidbody2D, IStatValueGiver statValueGiver)
        {
            _rigidbody = rigidbody2D;
            _transform = rigidbody2D.transform;
            _statValueGiver = statValueGiver;
            // Direction = true;
        }
        
        public void MoveHorizontally(float direction)
        {
            _movement.x = direction;
            SetDirrection(direction);
            Vector2 velocity = _rigidbody.velocity;
            velocity.x = direction * _statValueGiver.GetStatValue(StatType.Speed);
            _rigidbody.velocity = velocity;
        }

        public void MoveVertically(float direction) 
        {
            _movement.y = direction;

            Vector2 velocity = _rigidbody.velocity;
            velocity.y = direction * _statValueGiver.GetStatValue(StatType.Speed);
            _rigidbody.velocity = velocity;

        }
        
        private void SetDirrection (float direction) 
        {
            if ((Direction == Direction.Right && direction < 0) ||
                (Direction == Direction.Left && direction > 0))
                Flip();
        }

        private void Flip() {
            _transform.Rotate(0,180,0);
            Direction = Direction == Direction.Right ? Direction.Left : Direction.Right;
        }
    }
}