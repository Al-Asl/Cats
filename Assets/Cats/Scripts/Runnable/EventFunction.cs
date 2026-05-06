using UnityEngine;
using UnityEngine.Events;

public class EventFunction : Runnable
{
    public UnityEvent _Event;

    protected override void RunInternal()
    {
        _Event.Invoke();
    }  
}