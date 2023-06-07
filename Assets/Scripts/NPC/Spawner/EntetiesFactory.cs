using System;
using System.Linq;
using Core.StatSystem;
using NPC.Behaviour;
using NPC.Controller;
using NPC.Data;
using NPC.Enums;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NPC.Spawner
{
    public class EntitiesFactory 
    {
        private readonly Transform _entitiesContainer;
        private readonly EntitiesSpawnerDataStorage _entitiesSpawnerDataStorage;
        
        public EntitiesFactory(EntitiesSpawnerDataStorage entitiesSpawnerDataStorage)
        {
            _entitiesSpawnerDataStorage = entitiesSpawnerDataStorage;
            var gameObject = new GameObject
            {
                name = nameof(EntitySpawner)
            };
            _entitiesContainer = gameObject.transform;
        }

        public Entity GetEntityBrain(EntityId entityId, Vector2 position)
        {
            var data = _entitiesSpawnerDataStorage.EntitiesSpawnData.Find(element => element.Id == entityId);
            var baseEntityBehaviour = Object.Instantiate(data.EntityBehaviourPrefab, position, Quaternion.identity);
            baseEntityBehaviour.transform.SetParent(_entitiesContainer);
            var stats = data.Stats.Select(stat => stat.GetCopy()).ToList();
            var statsController = new StatsController(stats);
            switch (entityId)
            {
                case EntityId.Knight:
                    return new KnightEntity(baseEntityBehaviour as KnightEntityBehaviour, statsController);
                case EntityId.None:
                default:
                    throw new NotImplementedException();
            }
        }
    }
}