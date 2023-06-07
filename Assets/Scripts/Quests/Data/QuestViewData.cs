using System.Collections.Generic;

namespace Quests.Data
{
    public class QuestViewData
    {
        public string QuestText { get; }
        public Dictionary<string, bool> Steps { get; }

        public QuestViewData(string questText, Dictionary<string, bool> steps)
        {
            QuestText = questText;
            Steps = steps;
        }
    }
}