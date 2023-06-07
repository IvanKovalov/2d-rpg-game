using Core.Enums;
using Core.StatSystem;
using Core.StatSystem.Enums;
using UnityEngine;

namespace Core.Movement.Controller
{
    public abstract class NPCDirectionalMover
    {
        protected readonly Rigidbody2D Rigidbody;
        private readonly Transform _transform;

        private Vector2 _movement;
        
        public abstract bool IsMoving { get;  }
        public Direction Direction { get; private set; }

        public NPCDirectionalMover(Rigidbody2D rigidbody2D)
        {
           Rigidbody = rigidbody2D;
           _transform = rigidbody2D.transform;
            Direction = Direction.Right;
        }

        public abstract void MoveHorizontally(float direction);


        public abstract void MoveVertically(float direction);



        public void SetDirection(Direction newDirection)
        {
            if (Direction == newDirection)
                return;

            Rigidbody.transform.Rotate(0, 180, 0);
            Direction = Direction == Direction.Right ? Direction.Left : Direction.Right;
        }

    }
}