using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Core
{
    public abstract class ScreenView: MonoBehaviour
    {
        [SerializeField] private Canvas _root;
        [SerializeField] private Button _closeButton;

        public event Action CloseClicked;
        private void Awake() => Subscribe();
        private void OnDestroy() => Unsubscribe();

        public virtual void Show() => _root.enabled = true;
        public virtual void Hide() => _root.enabled = false;
        protected virtual void Subscribe() => _closeButton.onClick.AddListener(() => CloseClicked?.Invoke());
        protected virtual void Unsubscribe() => _closeButton.onClick.RemoveAllListeners();
    }
}