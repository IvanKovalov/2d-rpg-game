using TMPro;
using UI.Core;
using UI.QuestsUI.Element;
using UnityEngine;

namespace UI.QuestsUI
{
    public class QuestsScreenView : ScreenView
    {
        [SerializeField] private TMP_Text _questText;
        [field: SerializeField] public Transform QuestsContainer { get; private set; }
        [field: SerializeField] public Transform StepsLinesContainer { get; private set; }
        [field: SerializeField] public QuestLine QuestLinePrefab { get; private set; }
        [field: SerializeField] public TextLine StepLinePrefab { get; private set; }

        public void SetQuestText(string text) => _questText.text = text;
    }
}