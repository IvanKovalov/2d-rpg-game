using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Services.Updater
{
    public class ProjectUpdater : MonoBehaviour, IProjectUpdater
    {
        public static IProjectUpdater Instance;
        
        
        public event Action UpdateCalled;
        public event Action FixedUpdateCalled;
        public event Action LateUpdateCalled;
        
        Coroutine IProjectUpdater.StartCoroutine(IEnumerator coroutine)
        {
            return StartCoroutine(coroutine);
        }


        private bool _isPaused;
        private readonly List<Coroutine> _activeCoroutines = new List<Coroutine>();

        public bool IsPaused
        {
            get => _isPaused;
            set
            {
                
                if(_isPaused == value)
                    return;
                
                Time.timeScale = value ? 0 : 1;
                _isPaused = value;
            }
        } 

        public void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
            }
        }

        public void Invoke(Action action, float time)
        {
            _activeCoroutines.Add(StartCoroutine(InvokeCoroutine(action, time)));
        }

        private IEnumerator InvokeCoroutine(Action action, float time)
        {
            yield return new WaitForSeconds(time);
            yield return new WaitUntil((() => !_isPaused));
            action?.Invoke();
            _activeCoroutines.RemoveAll(element => element == null);
        }
        
        void IProjectUpdater.StopCoroutine(Coroutine coroutine)
        {
            if(coroutine != null)
                StopCoroutine(coroutine);
        }

        private void Update()
        {
            if(IsPaused)
                return;
            
            UpdateCalled?.Invoke();
        }

        private void FixedUpdate()
        {
            if(IsPaused)
                return;
            
            FixedUpdateCalled?.Invoke();
        }

        private void LateUpdate()
        {
            if(IsPaused)
                return;
            
            LateUpdateCalled?.Invoke();
        }
    }
}