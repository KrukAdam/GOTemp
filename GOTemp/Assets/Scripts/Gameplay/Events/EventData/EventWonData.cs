using System;

[Serializable]
public class EventWonData : EventBaseData
{
    public EventWonData(Field field) : base(field)
    {

    }

    public bool CheckConditions()
    {
        return true;
    }
}
