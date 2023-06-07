using System;
using System.Collections.Generic;
using UI.Enum;

namespace UI.Core
{
    public interface IScreenController
    {
        public event Action CloseRequested;
        public event Action<ScreenType> OpenScreenRequested;

        void Initialize(List<object> data);
        void Complete();
    }
}