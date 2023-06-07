using System;
using System.Collections.Generic;
using Quests.Data;
using Quests.Enums;

namespace UI.QuestsUI.Model
{
    public interface IQuestsScreenModel
    {
        List<QuestId> ActiveQuests { get; }
        List<QuestId> CompletedQuests { get; }
        QuestViewData ActiveQuest { get; }
        event Action<QuestViewData> SelectedQuestChanged;
        void SelectQuest(QuestId questId);
    }
}