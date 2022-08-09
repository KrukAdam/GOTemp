public class ActionObjectFanPassage : ActionObject
{
    private ActionObjectFanPassage childFan;

    protected override void OnSetup()
    {
        base.OnSetup();

        childFan = ActionObjectInterac as ActionObjectFanPassage;
    }

    public bool MoveOnFan(EDirection moveDirection, out ActionObject actionObject)
    {
        actionObject = childFan; 
        if (moveDirection == StartLookDirection)
        {
            return true;
        }

        return false;
    }

}
