using UnityEngine;
using Core.Enums;
namespace Core.Movement.Controller
{
    public class VelocityMover : NPCDirectionalMover
    {
        private Vector2 _movement;
        public override bool IsMoving { get; }

        public override void MoveHorizontally(float direction)
        {
            _movement.x = direction;
            Vector2 velocity = Rigidbody.velocity;
            velocity.x = direction;
            Rigidbody.velocity = velocity;
            if (direction == 0)
            {
                return;
            }
            
            SetDirection(direction > 0 ? Direction.Right : Direction.Left);
        }

        public override void MoveVertically(float direction) 
        {
            _movement.y = direction;

            Vector2 velocity = Rigidbody.velocity;
            velocity.y = direction;
            Rigidbody.velocity = velocity;

        }

        public VelocityMover(Rigidbody2D rigidbody2D) : base(rigidbody2D)
        {
        }
    }
}