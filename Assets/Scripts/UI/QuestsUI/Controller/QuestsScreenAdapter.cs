using System.Collections.Generic;
using Core.ObjectPool;
using Quests.Data;
using Quests.Enums;
using Quests.Enums;
using UI.Core;
using UI.QuestsUI;
using UI.QuestsUI.Element;
using UI.QuestsUI.Model;
using UnityEngine;
using UnityEngine.UI;

namespace UI.QuestsUI.Controller
{
    public class QuestsScreenAdapter : ScreenController<QuestsScreenView>
    {
        private readonly IQuestsScreenModel _questsScreenModel;
        private readonly Dictionary<QuestLine, QuestId> _activeQuests;
        private readonly List<QuestLine> _completedQuests;
        private readonly List<TextLine> _stepLines;
        
        public QuestsScreenAdapter(QuestsScreenView view, IQuestsScreenModel questsScreenModel) : base(view)
        {
            _questsScreenModel = questsScreenModel;
            _activeQuests = new Dictionary<QuestLine, QuestId>();
            _stepLines = new List<TextLine>();
            _completedQuests = new List<QuestLine>();
        }

        public override void Initialize(List<object> data)
        {
            base.Initialize(data);

            if (_questsScreenModel.ActiveQuest != null)
            {
                SetSelectedQuest(_questsScreenModel.ActiveQuest);
            
                _questsScreenModel.SelectedQuestChanged += SetSelectedQuest;
            
                foreach (var quest in _questsScreenModel.ActiveQuests)
                {
                    var questLine = CreateQuestLine();
                    questLine.Selected += OnQuestSelected;
                    _activeQuests.Add(questLine, quest);
                }
            }

            foreach (var quest in _questsScreenModel.CompletedQuests)
            {
                var questLine = CreateQuestLine();
                questLine.SetQuest(quest.ToString());
                _completedQuests.Add(questLine);
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)View.transform);
        }

        public override void Complete()
        {
            base.Complete();
            ClearStepLines();
            foreach (var questLine in _activeQuests.Keys)
            {
                questLine.Selected -= OnQuestSelected;
                questLine.Reset();
            }
            _activeQuests.Clear();

            foreach(var questLine in _completedQuests)
                questLine.Reset();
            _completedQuests.Clear();
        }
        
        private QuestLine CreateQuestLine()
        {
            var questLine = ObjectPool.Instance.GetObject(View.QuestLinePrefab);
            questLine.transform.SetParent(View.QuestsContainer);
            questLine.transform.localScale = Vector3.one;
            return questLine;
        }

        private void SetSelectedQuest(QuestViewData questViewData)
        {
            ClearStepLines();
            View.SetQuestText(questViewData.QuestText);
            foreach (var step in questViewData.Steps)
            {
                var stepLine = ObjectPool.Instance.GetObject(View.StepLinePrefab);
                stepLine.transform.SetParent(View.StepsLinesContainer);
                stepLine.transform.localScale = Vector3.one;
                stepLine.SetText(step.Key, step.Value);
                stepLine.gameObject.SetActive(true);
                _stepLines.Add(stepLine);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)View.transform);
        }

        private void OnQuestSelected(QuestLine selectedLine)
        {
            foreach (var quest in _activeQuests.Keys)
                quest.SetSelected(quest == selectedLine);
            
            _questsScreenModel.SelectQuest(_activeQuests[selectedLine]);
        }
        
        private void ClearStepLines()
        {
            foreach(var line in _stepLines)
                line.Reset();
            _stepLines.Clear();
        }
    }
}