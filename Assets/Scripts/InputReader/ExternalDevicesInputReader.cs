using System;
using Core.Services.Updater;
using InputReader;
using UnityEngine;

public class ExternalDevicesInputReader : IEntityInputSource, IWindowsInputSource,  IDisposable
{
    
    public float HorizontalDirection => Input.GetAxisRaw("Horizontal");
    public float VerticalDirection => Input.GetAxisRaw("Vertical");
    public bool Jump { get; private set; }
    public bool Attack { get; private set; }
    
    public event Action InventoryRequested;
    public event Action SkillWindowRequested;
    public event Action SettingsWindowRequested;
    public event Action QuestWindowRequested;

    public ExternalDevicesInputReader()
    {
        ProjectUpdater.Instance.UpdateCalled += OnUpdate;
        
    }

    public void Dispose() => ProjectUpdater.Instance.UpdateCalled -= OnUpdate;
    
    
    public void ResetOneTimeAction()
    {
        Jump = false;
        Attack = false;
    }
    
    private void OnUpdate () 
    {

        if(Input.GetButtonDown("Jump"))
        {
            Jump = true;
        }

        if (Input.GetKeyDown("left ctrl"))
            Attack = true;

        if (Input.GetKeyDown(KeyCode.I))
        {
            InventoryRequested?.Invoke();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            QuestWindowRequested?.Invoke();
        }
        
    }

    /*private void FixedUpdate () {
        _playerEntity.MoveHorizontally(_horizontalDirection);
        _playerEntity.MoveVertically(_verticalDirection);
    }*/



} 