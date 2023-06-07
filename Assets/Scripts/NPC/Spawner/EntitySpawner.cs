using System;
using System.Collections.Generic;
using Drawing;
using InputReader;
using Items;
using NPC.Controller;
using NPC.Data;
using NPC.Enums;
using UnityEngine;

namespace NPC.Spawner
{
    public class EntitySpawner : IDisposable
    {
        private readonly LevelDrawer _levelDrawer;
        private readonly Dictionary<Entity, EntityId> _entities;
        private readonly EntitiesFactory _entitiesFactory;
        private DropGenerator _dropGenerator;
        private GameUIInputView _gameUIInputView;
        private Transform _spawner;

        public event Action<EntityId> EntityDied; 

        public EntitySpawner(LevelDrawer levelDrawer, DropGenerator dropGenerator, GameUIInputView gameUIInputView, Transform spawner)
        {
            _levelDrawer = levelDrawer;
            _entities = new Dictionary<Entity, EntityId>();
            var entitiesSpawnerDataStorage = Resources.Load<EntitiesSpawnerDataStorage>($"{nameof(EntitySpawner)}/{nameof(EntitiesSpawnerDataStorage)}");
            _entitiesFactory = new EntitiesFactory(entitiesSpawnerDataStorage);
            _dropGenerator = dropGenerator;
            _gameUIInputView = gameUIInputView;
            _spawner = spawner;
            _gameUIInputView.SpawnRequested += SpawnEntity;
        }

        private void SpawnEntity()
        {
            var entity = _entitiesFactory.GetEntityBrain(EntityId.Knight, _spawner.position);
            entity.ObjectDied += RemoveEntity;
            _levelDrawer.RegisterElement(entity);
            _entities.Add(entity, EntityId.Knight);
        }

        public void SpawnEntity(EntityId entityId, Vector2 position)
        {
            var entity = _entitiesFactory.GetEntityBrain(entityId, position);
            entity.ObjectDied += RemoveEntity;
            _levelDrawer.RegisterElement(entity);
            _entities.Add(entity, entityId);
        }

        public void Dispose()
        {
            foreach(var entity in _entities.Keys)
                DestroyEntity(entity);
            _entities.Clear();
        }

        private void RemoveEntity(Entity entity)
        {
            EntityDied?.Invoke(_entities[entity]);
            _entities.Remove(entity);
            DestroyEntity(entity);
            _dropGenerator.DropRandomItem(_dropGenerator.GetDropRarity());
        }

        private void DestroyEntity(Entity entity)
        {
            _levelDrawer.UnregisterElement(entity);
            entity.ObjectDied -= RemoveEntity;
            entity.Dispose();
        }
    }
}