using System.Collections.Generic;
using Quests.Enums;
using UnityEngine;

namespace Quests.Data
{
    [CreateAssetMenu(fileName = "Quest", menuName= "Quests/Quest")]
    public class QuestData : ScriptableObject
    {
        [TextArea(10, 100)]
        [SerializeField] private string _textDescription;
        
        [field: SerializeField] public QuestId QuestId { get; private set; }
        [field: SerializeField] public List<StepData> Steps { get; private set; }
        [field: SerializeField] public Reward Reward { get; private set; }

        public string TextDescription => _textDescription;
    }
}