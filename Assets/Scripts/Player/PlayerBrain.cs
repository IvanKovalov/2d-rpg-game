using System;
using System.Collections.Generic;
using System.Linq;
using BattleSystem;
using BattleSystem.Weapon;
using Core.Services.Updater;
using Core.StatSystem;
using Core.StatSystem.Enums;
using InputReader;
using Items;
using Items.Core;
using UnityEngine;

namespace Player
{
  public sealed class PlayerBrain : IDisposable
  {
    private readonly PlayerEntityBehavior playerEntityBehavior;
    private readonly List<IEntityInputSource> _inputSources;
    private StatsController _statsController;

    private float _hp;
    private bool _isAttack;
    private bool _canAttack = true;
    private BaseWeapon _currWeapon;
    private WeaponsFactory _weaponsFactory;
    private Inventory _inventory;
    public event Action<PlayerBrain> ObjectDied;

    public PlayerBrain(PlayerEntityBehavior playerEntityBehavior, List<IEntityInputSource> inputSources, StatsController statsController, WeaponsFactory weaponsFactory, Inventory inventory)
    {
      this.playerEntityBehavior = playerEntityBehavior;
      playerEntityBehavior.ActionRequested += OnAttackStarted;
      playerEntityBehavior.AnimationEnded += OnAttackEnded;
      _inputSources = inputSources;
      _statsController = statsController;
      _hp = statsController.GetStatValue(StatType.Health);
      this.playerEntityBehavior.DamageTaken += OnDamageTaken;
      VisualiseHp(statsController.GetStatValue(StatType.Health));
      _weaponsFactory = weaponsFactory;
      _inventory = inventory;
      ProjectUpdater.Instance.FixedUpdateCalled += OnFixedUpdate;
      ObjectDied += OnDead;
    }

    private void OnAttackEnded()
    {
      _canAttack = false;
      _currWeapon.EndAttack();
      ProjectUpdater.Instance.Invoke(() => _canAttack = true, _statsController.GetStatValue(StatType.AfterAttackDelay));
    }

    private void OnAttackStarted()
    {

      _currWeapon?.Attack(_statsController.GetStatValue(StatType.Damage));
      Debug.Log("Attack");
    }
    private void OnDead(PlayerBrain pb)
    {
      playerEntityBehavior.PlayAnimation(AnimationType.Death, true);
      playerEntityBehavior.TriggerDeathCor();
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
      playerEntityBehavior.MoveHorizontally(GetHorizontalDirection());
      playerEntityBehavior.MoveVertically(GetVerticalDirection());

      if (IsJump)
        playerEntityBehavior.Jump();

      if (IsAttack && _canAttack)
      {
        Equipment weapon;
        weapon = _inventory.Equipment.Find(element => element.IsWeapon());
        if (weapon != null)
        {
          _currWeapon = _weaponsFactory.GetWeapon(weapon.Descriptor.ItemId);
          playerEntityBehavior.StartAttck();
        }
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
        if (inputSource.HorizontalDirection == 0)
          continue;

        return inputSource.HorizontalDirection;
      }

      return 0;
    }

    private float GetVerticalDirection()
    {
      foreach (var inputSource in _inputSources)
      {
        if (inputSource.VerticalDirection == 0)
          continue;

        return inputSource.VerticalDirection;
      }

      return 0;
    }

    private bool IsJump => _inputSources.Any(source => source.Jump);
    private bool IsAttack => _inputSources.Any(source => source.Attack);

    protected void VisualiseHp(float hp)
    {
      if (playerEntityBehavior.HpBar.maxValue < hp)
        playerEntityBehavior.HpBar.value = hp;

      playerEntityBehavior.HpBar.value = hp;
    }
  }
}