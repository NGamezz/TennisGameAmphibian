using System.Collections.Generic;

public enum EventType
{
    StartGame = 0,
    Restart = 1,
    Victory = 2,
    Pause = 3,
    UnPause = 4,
}

public static class EventManager
{
    public static Dictionary<EventType, System.Action> events = new();

    public static void AddListener(EventType type, System.Action action)
    {
        if (!events.ContainsKey(type))
        {
            events.Add(type, action);
        }
        else
        {
            events[type] += action;
        }
    }

    public static void RemoveListener(EventType type, System.Action action)
    {
        if (!events.ContainsKey(type)) { return; }
        events[type] -= action;
    }

    public static void ClearEvents(bool AreYouSure = false)
    {
        if (AreYouSure)
        {
            events.Clear();
        }
    }

    public static void InvokeEvent(EventType type)
    {
        if (!events.ContainsKey(type)) { return; }
        events[type]?.Invoke();
    }
}
