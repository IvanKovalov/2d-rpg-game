using System;
using Core.StatSystem;
using Drawing;
using NPC.Behaviour;
using UnityEngine;

namespace NPC.Controller
{
    public class Entity : ILevelGraphicElement
    {
        private readonly BaseEntityBehaviour _entityBehaviour;
        protected readonly StatsController StatsController;

        public event Action<Entity> ObjectDied;

        public event Action<ILevelGraphicElement> VerticalPositionChanged;

        protected Entity(BaseEntityBehaviour entityBehaviour, StatsController statsController)
        {
            _entityBehaviour = entityBehaviour;
            _entityBehaviour.Initialize();
            StatsController = statsController;
        }

        public float VerticalPosition => _entityBehaviour.VerticalPosition;
        public void SetDrawingOrder(int order) => _entityBehaviour.SetDrawingOrder(order);
        public void SetSize(Vector2 size) => _entityBehaviour.SetSize(size);
        public void SetVerticalPosition(float verticalPostion) => _entityBehaviour.SetVerticalPosition(verticalPostion);
        public virtual void Dispose() => StatsController?.Dispose();
        protected void OnVerticalPositionChanged() => VerticalPositionChanged?.Invoke(this);
    }
}