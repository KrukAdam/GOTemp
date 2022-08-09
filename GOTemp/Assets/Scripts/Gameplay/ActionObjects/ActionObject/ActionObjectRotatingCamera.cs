using System.Collections.Generic;

public class ActionObjectRotatingCamera : ActionObjectStillCamera
{
    private List<Field> secondLookFields = new List<Field>();
    private bool isRotate = false;
    private EDirection secondLookDirection;

    public override void Setup(ObjectSpawner objectSpawner, Board board)
    {
        secondLookDirection = objectSpawner.SecondLookDirection;

        base.Setup(objectSpawner, board);
    }

    public override void ExecuteGetSignal()
    {
        RotateCameraLook();
    }

    protected override void OnSetup()
    {
        actionObjectVisual.RotateBody(StartLookDirection);
        isOn = !isOn; //On set look is change opposite

        SetupLookFields();
        SetupSecondLookFields();
        EnableSecondLookFields(false);
        EnableLookFields(true);
    }

    private void RotateCameraLook()
    {
        isRotate = !isRotate;

        if (isRotate)
        {
            EnableLookFields(false);
            EnableSecondLookFields(true);

            actionObjectVisual.RotateBody(secondLookDirection);
        }
        else
        {
            EnableSecondLookFields(false);
            EnableLookFields(true);

            actionObjectVisual.RotateBody(StartLookDirection);
        }
    }

    private void EnableLookFields(bool enable)
    {
        if (enable)
        {
            foreach (var field in lookFields)
            {
                EventObjectLookData eventObjectLook = new EventObjectLookData(field, this);
                field.Events.Add(eventObjectLook);
            }
        }
        else
        {
            foreach (var field in lookFields)
            {
                EventBaseObjectData eventLook = field.GetObjectEvent(this);
                field.Events.Remove(eventLook);
            }
        }
    }

    private void EnableSecondLookFields(bool enable)
    {
        if (enable)
        {
            foreach (var field in secondLookFields)
            {
                EventObjectLookData eventObjectLook = new EventObjectLookData(field, this);
                field.Events.Add(eventObjectLook);
            }
        }
        else
        {
            foreach (var field in secondLookFields)
            {
                EventBaseObjectData eventLook = field.GetObjectEvent(this);
                field.Events.Remove(eventLook);
            }
        }
    }

    protected void SetupSecondLookFields()
    {
        secondLookFields.Clear();
        secondLookFields.Add(board.Fields[LogicPos]);

        for (int i = 0; i < amountLookFields; i++)
        {
            if (secondLookFields.Count > i)
            {
                Field fieldToAdd = board.GetFieldFromCurrentField(secondLookFields[i].LogicPosition, secondLookDirection);
                if (fieldToAdd != null)
                {
                    secondLookFields.Add(fieldToAdd);
                }
            }
        }
    }
}
