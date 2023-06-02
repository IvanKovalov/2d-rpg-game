using System.Collections.Generic;
using UnityEngine;

namespace Drawing.Data
{
    
    [CreateAssetMenu(fileName = nameof(LevelsDrawingDataStorage), 
        menuName = "Drawing/LevelsDrawingDataStorage")]
    public class LevelsDrawingDataStorage : ScriptableObject
    {
        [field: SerializeField] public List<LevelDrawingDataStorage> LevelsData { get; private set; }
    }
}