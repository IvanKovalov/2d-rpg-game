using System;
using Player;
using UnityEngine;

namespace Core.Animations
{
    public abstract class AnimatorController : MonoBehaviour
    {

        private AnimationType _currentAnimationType;

        public event Action ActionRequested;
        public event Action AnimationEnded;
        
        private Action _animationAction;
        private Action _animationEndAction;
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
        
            if (_currentAnimationType >= animationType)
            {
                return false;
            }

            _currentAnimationType = animationType;
            PlayAnimation(_currentAnimationType);
            return true;
        }
        
        public abstract void Initialize();

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
        
        public abstract void SetAnimationParameter(string parameter, int value);

        private void SetAnimation(AnimationType animationType)
        {
            _currentAnimationType = animationType;
            PlayAnimation(_currentAnimationType);
        }
        
        protected abstract void PlayAnimation(AnimationType animationType);

        protected void OnActionRequested() => ActionRequested?.Invoke();
        protected void OnAnimationEnded()
        {
            _animationEndAction?.Invoke();
            SetAnimation(AnimationType.Idle);
        }
    }
}