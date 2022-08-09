
public class ActionObjectAlarm : ActionObject
{

    public override void ExecuteGetSignal()
    {
        LevelController.LostLevel();
    }
}
