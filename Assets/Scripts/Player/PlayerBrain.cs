using System;
using System.Collections.Generic;
using System.Linq;
using BattleSystem;
using Core.Services.Updater;
using Core.StatSystem;
using Core.StatSystem.Enums;
using InputReader;
using UnityEngine;

namespace Player
{
    public sealed class PlayerBrain : IDisposable
    {
        private readonly PlayerEntityBehavior playerEntityBehavior;
        private readonly List<IEntityInputSource> _inputSources;
        private StatsController _statsController;

        private float _hp;
        private bool _isAttacking;
        private bool _canAttack;
        
        public event Action<PlayerBrain> ObjectDied;

        public PlayerBrain(PlayerEntityBehavior playerEntityBehavior, List<IEntityInputSource> inputSources, StatsController statsController)
        {
            this.playerEntityBehavior = playerEntityBehavior;
            this.playerEntityBehavior.AttackEnded += EndAttack;
            this.playerEntityBehavior.AttackRequested += OnAttackRequested;
            _inputSources = inputSources;
            _statsController = statsController;
            _hp = statsController.GetStatValue(StatType.Health);
            this.playerEntityBehavior.DamageTaken += OnDamageTaken;
            VisualiseHp(statsController.GetStatValue(StatType.Health));
            ProjectUpdater.Instance.FixedUpdateCalled += OnFixedUpdate;
        }

        private void OnAttackRequested()
        {
            //Attack
        }

        private void EndAttack()
        {
            _isAttacking = false;
            ProjectUpdater.Instance.Invoke(()=>
                _canAttack = true, _statsController.GetStatValue(StatType.AfterAttackDelay));
        }

        private void OnDamageTaken(float damage)
        {
            damage -= _statsController.GetStatValue(StatType.Defence);
            Debug.Log(damage);
            if (damage < 0)
            {
                return;
            }

            _hp = Mathf.Clamp(_hp - damage, 0, _hp);
            Debug.Log(_hp);
            VisualiseHp(_hp);
            if (_hp <= 0)
            {
                ObjectDied?.Invoke(this);
            }
        }

        public void Dispose() => ProjectUpdater.Instance.FixedUpdateCalled -= OnFixedUpdate;

        private void OnFixedUpdate()
        {
            if (_isAttacking)
                return;
            
            playerEntityBehavior.MoveHorizontally(GetHorizontalDirection());
            playerEntityBehavior.MoveVertically(GetVerticalDirection());
            
            if(IsJump)
                playerEntityBehavior.Jump();

            if (IsAttack && _canAttack)
            {
                playerEntityBehavior.StartAttck();
                _isAttacking = true;
                _canAttack = false;
            }

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

        protected  void VisualiseHp(float hp)
        {
            if (playerEntityBehavior.HpBar.maxValue < hp)
                playerEntityBehavior.HpBar.value = hp;

            playerEntityBehavior.HpBar.value = hp;
        }
    }
}