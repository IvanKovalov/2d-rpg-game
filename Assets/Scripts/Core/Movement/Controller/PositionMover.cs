
using UnityEngine;
using Core.Enums;
namespace Core.Movement.Controller
{
    public class PositionMover : NPCDirectionalMover
    {
        private Vector2 _destination;
        public override bool IsMoving => _destination != Rigidbody.position;
        
        public PositionMover(Rigidbody2D rigidbody2D) : base(rigidbody2D){ }

        public override void MoveHorizontally(float horizontalMovement)
        {
            _destination.x = horizontalMovement;
            var newPosition = new Vector2(horizontalMovement, Rigidbody.position.y);
            Rigidbody.MovePosition(newPosition);
            if (_destination.x != Rigidbody.position.x) 
                SetDirection(_destination.x > Rigidbody.position.x ? Direction.Right : Direction.Left);
            
        }

        public override void MoveVertically(float verticalMovement)
        {
            _destination.y = verticalMovement;
            var newPosition = new Vector2(Rigidbody.position.x , verticalMovement);
            Rigidbody.MovePosition(newPosition);
        }
    }
}