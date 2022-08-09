
public class ActionObjectLaser : ActionPathObject
{
    protected override void OnSetup()
    {
        base.OnSetup();

        SetEnabledElement();
    }

    public override void ExecuteSendSignal()
    {
        if (isOn)
        {
            SendSignal();
        }
    }

    public override void ExecuteGetSignal()
    {
        isOn = !isOn;

        SetEnabledElement();
    }

    private void SetEnabledElement()
    {
        actionObjectVisual.EnabledObjectElement(isOn);
    }
}
