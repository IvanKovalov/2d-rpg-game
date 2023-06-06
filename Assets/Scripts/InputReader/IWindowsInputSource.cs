using System;

namespace InputReader
{
    public interface IWindowsInputSource
    {
        event Action InventoryRequested;
        event Action SkillWindowRequested;
        event Action SettingsWindowRequested;
        event Action QuestWindowRequested;
    }
}