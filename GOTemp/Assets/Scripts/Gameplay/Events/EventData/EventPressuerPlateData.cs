
public class EventPressuerPlateData : EventBaseObjectData
{
    public EventPressuerPlateData(Field field, ActionObject actionObject) : base(field, actionObject)
    {
    }

    public override void Execute()
    {
        ActionObject.ExecuteSendSignal();
    }
}
