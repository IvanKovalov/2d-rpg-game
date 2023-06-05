using System;
using UnityEngine;

namespace Core.ObjectPool
{
    public interface IPoolable
    {
        GameObject GameObject { get; }
        event Action<IPoolable> Destroyed;
        void Reset();
    }
}