using System;
using Core.StatSystem;
using Core.StatSystem.Enums;
using Drawing;
using NPC.Behaviour;
using UnityEngine;

namespace NPC.Controller
{
    public abstract class Entity : ILevelGraphicElement
    {
        private readonly BaseEntityBehaviour _entityBehaviour;
        protected readonly StatsController StatsController;

        private float _hp;
        public event Action<Entity> ObjectDied;

        public event Action<ILevelGraphicElement> VerticalPositionChanged;

        protected Entity(BaseEntityBehaviour entityBehaviour, StatsController statsController)
        {
            _entityBehaviour = entityBehaviour;
            _entityBehaviour.Initialize();
            StatsController = statsController;

            _hp = StatsController.GetStatValue(StatType.Health);
            _entityBehaviour.DamageTaken += OnDamageTaken;
        }

        private void OnDamageTaken(float damage)
        {
            damage -= StatsController.GetStatValue(StatType.Defence);
            if (damage < 0)
            {
                return;
            }

            _hp = Mathf.Clamp(_hp - damage, 0, _hp);

            if (_hp <= 0)
            {
                ObjectDied?.Invoke(this);
            }
        }

        protected abstract void VisualiseHp(float hp);

        public float VerticalPosition => _entityBehaviour.VerticalPosition;
        public void SetDrawingOrder(int order) => _entityBehaviour.SetDrawingOrder(order);
        public void SetSize(Vector2 size) => _entityBehaviour.SetSize(size);
        public void SetVerticalPosition(float verticalPostion) => _entityBehaviour.SetVerticalPosition(verticalPostion);
        public virtual void Dispose() => StatsController?.Dispose();
        protected void OnVerticalPositionChanged() => VerticalPositionChanged?.Invoke(this);
    }
}