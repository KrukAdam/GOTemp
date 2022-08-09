
public class ActionObjectConveyorBelt : ActionObject
{
    private ActionObjectConveyorBelt nextBelt;
    private ActionObjectConveyorBelt backBelt;
    private EDirection secondLookDirection;
    private bool moveToMainSide;

    public override void Setup(ObjectSpawner objectSpawner, Board board)
    {
        secondLookDirection = objectSpawner.SecondLookDirection;

        base.Setup(objectSpawner, board);
    }

    protected override void OnSetup()
    {
        base.OnSetup();

        nextBelt = ActionObjectInterac as ActionObjectConveyorBelt;

        if (nextBelt != null)
        {
            nextBelt.SetBackBelt(this);
        }

        actionObjectVisual.RotateBody(StartLookDirection);
        moveToMainSide = isOn;
    }

    public void SetBackBelt(ActionObjectConveyorBelt backBelt)
    {
        this.backBelt = backBelt;
    }

    public override void ExecuteGetSignal()
    {
        moveToMainSide = !moveToMainSide; 

        if (nextBelt != null)
        {
            nextBelt.ExecuteGetSignal();
        }

        Rotate();
    }

    public override void ExecuteInteraction()
    {
        MoveToNextBelt();
    }

    private void MoveToNextBelt()
    {
        if (moveToMainSide && nextBelt != null)
        {
            board.TeleportPlayerOnConveyorBelt(nextBelt.LogicPos, false);
        }
        else if (!moveToMainSide && backBelt != null)
        {
            board.TeleportPlayerOnConveyorBelt(backBelt.LogicPos, false);
        }
    }

    private void Rotate()
    {
        if (moveToMainSide)
        {
            actionObjectVisual.RotateBody(StartLookDirection);
        }
        else
        {
            actionObjectVisual.RotateBody(secondLookDirection);
        }
    }
}
