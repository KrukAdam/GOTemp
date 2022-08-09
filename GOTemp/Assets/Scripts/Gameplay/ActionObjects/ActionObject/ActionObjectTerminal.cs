using System;

public class ActionObjectTerminal : ActionObject
{
    public Action<bool> Interaction = delegate { }; //bool - Show terminal

    public override void ExecuteInteraction()
    {
        Interaction(true);
    }

    protected override void OnSetup()
    {
        base.OnSetup();

        actionObjectVisual.RotateBody(StartLookDirection);
    }
}
