using Core.Movement.Data;
using Core.StatSystem;
using Core.StatSystem.Enums;
using UnityEngine;

namespace Core.Movement.Controller
{
    public class Jumper
    {
        private readonly JumpData _jumpData;
        private readonly Rigidbody2D _rigidbody;
       
        private readonly Transform _transform;
        private readonly IStatValueGiver _statValueGiver;

        private float _startJumpVerticalPosition;
        
        public bool isJump { get; private set; }

        public Jumper(Rigidbody2D rigidbody2D, JumpData jumpData,  IStatValueGiver statValueGiver)
        {
            _rigidbody = rigidbody2D;
            _jumpData = jumpData;
            _transform = _rigidbody.transform;
            _statValueGiver = statValueGiver;
        }
        
        
        public void Jump () {
            if (isJump) {
                return;
            }

            isJump = true;
            _rigidbody.AddForce(Vector2.up * _statValueGiver.GetStatValue(StatType.JumpForce));
            _rigidbody.gravityScale = _jumpData.GravityScale;
            _startJumpVerticalPosition = _rigidbody.position.y;

        }
        
        public void UpdateJump() {
            if(_rigidbody.velocity.y < 0 && _rigidbody.position.y <= _startJumpVerticalPosition){
                ResetJump();
                return;
            }
        }

        private void ResetJump() {
            isJump = false;
            _rigidbody.position = new Vector2(_rigidbody.position.x, _startJumpVerticalPosition);
            _rigidbody.gravityScale = 0;
        }
    }
}