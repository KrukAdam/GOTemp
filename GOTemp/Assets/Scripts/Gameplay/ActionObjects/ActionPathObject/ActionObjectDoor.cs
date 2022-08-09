
public class ActionObjectDoor : ActionPathObject
{
    public bool IsClosed { get => isOn; }

    protected override void OnSetup()
    {
        base.OnSetup();

        SetRotate();
    }

    public override void ExecuteGetSignal()
    {
        isOn = !isOn;

        SetRotate();
    }

    private void SetRotate()
    {
        if (isOn)
        {
            actionObjectVisual.RotateBody(StartLookDirection);
        }
        else
        {
            actionObjectVisual.RotateBody(Direction.GetRightFromDirection(StartLookDirection));
        }
    }
}
