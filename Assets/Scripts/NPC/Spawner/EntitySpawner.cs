using System;
using System.Collections.Generic;
using Drawing;
using NPC.Controller;
using NPC.Data;
using NPC.Enums;
using UnityEngine;

namespace NPC.Spawner
{
    public class EntitySpawner : IDisposable
    {
        private readonly LevelDrawer _levelDrawer;
        private readonly List<Entity> _entities;
        private readonly EntitiesFactory _entitiesFactory;
        
        public EntitySpawner(LevelDrawer levelDrawer)
        {
            _levelDrawer = levelDrawer;
            _entities = new List<Entity>();
            var entitiesSpawnerDataStorage = Resources.Load<EntitiesSpawnerDataStorage>($"{nameof(EntitySpawner)}/{nameof(EntitiesSpawnerDataStorage)}");
            _entitiesFactory = new EntitiesFactory(entitiesSpawnerDataStorage);
        }
        
        public void SpawnEntity(EntityId entityId, Vector2 position)
        {
            var entity = _entitiesFactory.GetEntityBrain(entityId, position);
            entity.ObjectDied += RemoveEntity;
            _levelDrawer.RegisterElement(entity);
            _entities.Add(entity);
        }

        public void Dispose()
        {
            foreach(var entity in _entities)
                DestroyEntity(entity);
            _entities.Clear();
        }

        private void RemoveEntity(Entity entity)
        {
            _entities.Remove(entity);
            DestroyEntity(entity);
        }

        private void DestroyEntity(Entity entity)
        {
            _levelDrawer.UnregisterElement(entity);
            entity.ObjectDied -= RemoveEntity;
        }
    }
}