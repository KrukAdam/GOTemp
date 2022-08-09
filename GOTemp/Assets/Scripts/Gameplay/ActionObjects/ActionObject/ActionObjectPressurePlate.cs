
public class ActionObjectPressurePlate : ActionObject
{
    protected override void OnSetup()
    {
        base.OnSetup();

        Field standField = board.Fields[LogicPos];
        standField.Events.Add(new EventPressuerPlateData(standField, this));
    }

    public override void ExecuteSendSignal()
    {
        SendSignal();
    }
}
