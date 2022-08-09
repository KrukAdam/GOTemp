using System;
using UnityEngine;

[Serializable]
public class EventBaseData
{
    public EEventType EventType = EEventType.None;

    [SerializeField][HideInInspector]
    protected Field field = null;

    public EventBaseData(Field field)
    {
        this.field = field;
    }

    public virtual void Execute() { }
}
