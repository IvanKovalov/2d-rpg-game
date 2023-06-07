using System;
using UnityEngine;

namespace Core.Movement.Data
{
    
    [Serializable]
    public class DirectionalMovementData
    {
        [field: SerializeField] public bool FaceRight { get; private set;  }
        [field: SerializeField] public float MinSize { get; private set;  }
        [field: SerializeField] public float MaxSize { get; private set;  }
        [field: SerializeField] public float MaxVerticalPosition { get; private set;  }
        [field: SerializeField] public float MinVerticalPosition { get; private set;  }
    }
}