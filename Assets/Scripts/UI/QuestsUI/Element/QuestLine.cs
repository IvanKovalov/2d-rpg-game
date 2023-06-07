using System;
using Core.ObjectPool;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.QuestsUI.Element
{
    public class QuestLine : MonoBehaviour, IPoolable
    {
        [SerializeField] private Button _button;
        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _notSelectedColor;
        [SerializeField] private Image _backImage;
        [SerializeField] private TMP_Text _questName;
        
        public GameObject GameObject => gameObject;
        
        public event Action<IPoolable> Destroyed;
        public event Action<QuestLine> Selected;
        private void Awake() => _button.onClick.AddListener(() => Selected?.Invoke(this));
        private void OnDestroy() => _button.onClick.RemoveAllListeners();
        public void SetQuest(string questName) => _questName.text = questName;
        public void SetSelected(bool selected) => _backImage.color = selected ? _selectedColor : _notSelectedColor;
        
        public void Reset()
        {
            _backImage.color = _notSelectedColor;
            Destroyed?.Invoke(this);
        }
    }
}