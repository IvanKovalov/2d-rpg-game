using System;
using Core.Animations;
using Spine.Unity;
using UnityEngine;

namespace Player.PlayerAnimations
{
    [RequireComponent(typeof(SkeletonAnimation))]
    public class SpineAnimatorController : AnimatorController
    {
        [SpineAnimation, SerializeField] private string _idleAnimationKey;
        [SpineAnimation, SerializeField] private string _walkAnimationKey;
        [SpineAnimation, SerializeField] private string _runAnimationKey;
        [SpineAnimation, SerializeField] private string _jumpAnimationKey;
        [SpineAnimation, SerializeField] private string _attackAnimationKey;
        
        private SkeletonAnimation _skeletonAnimation;
        private void Start()
        {
            _skeletonAnimation = GetComponent<SkeletonAnimation>();
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

        public override void SetAnimationParameter(string parameter, int value)
        {
            throw new NotImplementedException();
        }

        protected override void PlayAnimation(AnimationType animationType)
        {
            string animationName = GetAnimationType(animationType);
            if (_skeletonAnimation.AnimationState.GetCurrent(0).Animation.Name == animationName)
            {
                return;
            }
            
            _skeletonAnimation.AnimationState.SetAnimation(0, GetAnimationType(animationType), true);
            
        }

        private string GetAnimationType(AnimationType animationType)
        {
            return animationType switch
            {
                AnimationType.Idle => _idleAnimationKey,
                AnimationType.Walk => _walkAnimationKey,
                AnimationType.Run => _runAnimationKey,
                AnimationType.Jump => _jumpAnimationKey,
                AnimationType.Attack => _attackAnimationKey,
                _ => _idleAnimationKey
            };
        }
    }
}