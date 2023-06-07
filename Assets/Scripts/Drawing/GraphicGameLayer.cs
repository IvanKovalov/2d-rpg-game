using System.Collections.Generic;

namespace Drawing
{
    public class GraphicGameLayer
    {
        private readonly List<ILevelGraphicElement> _elementsOnLayer;
        private readonly int _layerStartingOrder;

        public GraphicGameLayer(int layerStartingOrder)
        {
            _elementsOnLayer = new List<ILevelGraphicElement>();
            _layerStartingOrder = layerStartingOrder;
        }

        public void AddElement(ILevelGraphicElement element)
        {
            int index;
            if (!_elementsOnLayer.Contains(null))
            {
                _elementsOnLayer.Add(element);
                index = _elementsOnLayer.Count - 1;
            }
            else
            {
                index = _elementsOnLayer.IndexOf(null);
                _elementsOnLayer[index] = element;
            }
            
            element.SetDrawingOrder(_layerStartingOrder + index);
        }

        public void RemoveElement(ILevelGraphicElement element)
        {
            if (!_elementsOnLayer.Contains(element))
            {
                return;
            }

            var indexOf = _elementsOnLayer.IndexOf(element);
            _elementsOnLayer[indexOf] = null;
        }

        public bool ContainsElement(ILevelGraphicElement element) => _elementsOnLayer.Contains(element);

        public void Clear() => _elementsOnLayer.Clear();
    }
}