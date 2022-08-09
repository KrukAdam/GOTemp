
public class ActionObjectSwitch : ActionObject
{

    public override void ExecuteSendSignal()
    {
        SendSignal();
    }

    public override void ExecuteInteraction()
    {
        ExecuteSendSignal();
    }
}
