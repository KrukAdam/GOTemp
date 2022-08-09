using System.Collections.Generic;

public class ActionObjectStillCamera : ActionObject
{
    protected List<Field> lookFields = new List<Field>();

    protected int amountLookFields;

    public override void Setup(ObjectSpawner objectSpawner, Board board)
    {
        amountLookFields = objectSpawner.Range;

        base.Setup(objectSpawner, board);
    }

    public override void ExecuteGetSignal()
    {
        SetLookActive();
    }

    protected override void OnSetup()
    {
        actionObjectVisual.RotateBody(StartLookDirection);
        isOn = !isOn; //On set look is change opposite

        SetupLookFields();
        SetLookActive();
    }

    protected void SetLookActive()
    {
        isOn = !isOn;

        if (isOn)
        {
            CameraOn();
        }
        else
        {
            CameraOff();
        }
    }

    protected void CameraOn()
    {
        foreach (var field in lookFields)
        {
            EventObjectLookData eventObjectLook = new EventObjectLookData(field, this);
            field.Events.Add(eventObjectLook);
        }
    }

    protected void CameraOff()
    {
        foreach (var field in lookFields)
        {
            EventBaseObjectData eventLook = field.GetObjectEvent(this);
            field.Events.Remove(eventLook);
        }
    }

    protected void SetupLookFields()
    {
        lookFields.Clear();
        lookFields.Add(board.Fields[LogicPos]);

        for (int i = 0; i < amountLookFields; i++)
        {
            if(lookFields.Count > i)
            {
                Field fieldToAdd = board.GetFieldFromCurrentField(lookFields[i].LogicPosition, StartLookDirection);
                if(fieldToAdd != null)
                {
                    lookFields.Add(fieldToAdd);
                }
            }
        }
    }
}
