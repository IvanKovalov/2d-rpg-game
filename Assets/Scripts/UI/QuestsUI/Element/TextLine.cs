using System;
using Core.ObjectPool;
using TMPro;
using UnityEngine;
namespace UI.QuestsUI.Element
{
    public class TextLine : MonoBehaviour, IPoolable
    {
        [SerializeField] private TMP_Text _line;
        [SerializeField] private Color _activeColor;
        [SerializeField] private Color _passiveColor;

        public GameObject GameObject => gameObject;
        
        public event Action<IPoolable> Destroyed;

        public void SetText(string text, bool active)
        {
            _line.text = text;
            _line.color = active ? _activeColor : _passiveColor;
        }
        
        public void Reset()
        {
            _line.color =  _passiveColor;
            Destroyed?.Invoke(this);
        }
    }
}