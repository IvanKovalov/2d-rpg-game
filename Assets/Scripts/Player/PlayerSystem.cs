using System;
using System.Collections.Generic;
using System.Linq;
using BattleSystem;
using Core.StatSystem;
using InputReader;
using Items;
using UnityEngine;

namespace Player
{
    public class PlayerSystem : IDisposable
    {
        private readonly PlayerEntityBehavior playerEntityBehavior;
        private readonly PlayerBrain _playerBrain;
        public StatsController StatsController { get;}
        private readonly List<IDisposable> _disposables;
        public Inventory Inventory { get; }

        public PlayerSystem(PlayerEntityBehavior playerEntityBehavior, List<IEntityInputSource> inputSources, WeaponsFactory weaponsFactory)
        {
            _disposables = new List<IDisposable>();
            
            var statStorage = Resources.Load<StatsStorage>($"Player/{nameof(StatsStorage)}");
            var stats = statStorage.Stats.Select(stat => stat.GetCopy()).ToList();
            StatsController = new StatsController(stats);
            _disposables.Add(StatsController);
            
            this.playerEntityBehavior = playerEntityBehavior;
            this.playerEntityBehavior.Initialize(StatsController);
            Inventory = new Inventory(null, null, this.playerEntityBehavior.transform);
            _playerBrain = new PlayerBrain(this.playerEntityBehavior, inputSources, StatsController, weaponsFactory , Inventory);
            _disposables.Add(_playerBrain);

            
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}