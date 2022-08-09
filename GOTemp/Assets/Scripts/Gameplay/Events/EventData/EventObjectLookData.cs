public class EventObjectLookData : EventBaseObjectData
{
    public EventObjectLookData(Field field, ActionObject actionObject) : base(field, actionObject)
    {
    }

    public override void Execute()
    {
        ActionObject.SendSignal();
    }
}
