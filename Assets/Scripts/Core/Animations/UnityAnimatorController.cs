using Player;
using UnityEngine;

namespace Core.Animations
{
    [RequireComponent(typeof(Animator))]
    public class UnityAnimatorController : AnimatorController
    {
        private Animator _animator;
        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public override void Initialize() => _animator = GetComponent<Animator>();
        public override void SetAnimationParameter(string parameter, int value) => 
            _animator.SetInteger(parameter, value);

        protected override void PlayAnimation(AnimationType animationType)
        {
            _animator.SetInteger(nameof(AnimationType), (int)animationType);
        }
        
        
        
    }
}