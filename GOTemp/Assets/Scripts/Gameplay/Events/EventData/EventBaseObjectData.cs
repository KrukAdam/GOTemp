public class EventBaseObjectData : EventBaseData
{
    public ActionObject ActionObject 
    { 
        get; 
        private set; 
    }

    public EventBaseObjectData(Field field, ActionObject actionObject) : base(field)
    {
        ActionObject = actionObject;
    }
}
