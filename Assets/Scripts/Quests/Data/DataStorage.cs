using System.Collections.Generic;
using UnityEngine;

namespace Quests.Data
{
    [CreateAssetMenu(fileName = nameof(QuestsStorage), menuName = "Quests/QuestsStorage")]
    public class QuestsStorage : ScriptableObject
    {
        [field: SerializeField] public List<QuestData> Quests { get; private set; }
    }
}