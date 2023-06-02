using System;
using UnityEngine;

namespace Drawing
{
    public interface ILevelGraphicElement
    {
        event Action<ILevelGraphicElement> VerticalPositionChanged;
        float VerticalPosition { get; }
        void SetDrawingOrder(int order);
        void SetSize(Vector2 size);
        void SetVerticalPosition(float verticalPosition);
    }
}