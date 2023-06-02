using System;
using System.Collections.Generic;
using System.Linq;
using Core.Services.Updater;
using InputReader;

namespace Player
{
    public class PlayerBrain : IDisposable
    {
        private readonly PlayerEntityBehavior playerEntityBehavior;
        private readonly List<IEntityInputSource> _inputSources;

        public PlayerBrain(PlayerEntityBehavior playerEntityBehavior, List<IEntityInputSource> inputSources)
        {
            this.playerEntityBehavior = playerEntityBehavior;
            _inputSources = inputSources;
            ProjectUpdater.Instance.FixedUpdateCalled += OnFixedUpdate;
        }

        public void Dispose() => ProjectUpdater.Instance.FixedUpdateCalled -= OnFixedUpdate;

        private void OnFixedUpdate()
        {
            playerEntityBehavior.MoveHorizontally(GetHorizontalDirection());
            playerEntityBehavior.MoveVertically(GetVerticalDirection());
            
            if(IsJump)
                playerEntityBehavior.Jump();

            if (IsAttack)
                playerEntityBehavior.StartAttck();

            foreach (var inputSource in _inputSources)
            {
                inputSource.ResetOneTimeAction();
            }
        }

        private float GetHorizontalDirection()
        {
            foreach (var inputSource in _inputSources)
            {
                if(inputSource.HorizontalDirection == 0)
                    continue;

                return inputSource.HorizontalDirection;
            }

            return 0;
        }
        
        private float GetVerticalDirection()
        {
            foreach (var inputSource in _inputSources)
            {
                if(inputSource.VerticalDirection == 0)
                    continue;

                return inputSource.VerticalDirection;
            }

            return 0;
        }

        private bool IsJump => _inputSources.Any(source => source.Jump);
        private bool IsAttack => _inputSources.Any(source => source.Attack);


    }
}