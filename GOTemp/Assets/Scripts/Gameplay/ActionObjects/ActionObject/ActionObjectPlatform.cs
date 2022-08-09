using System.Collections.Generic;

public class ActionObjectPlatform : ActionObject
{
    private List<Field> fieldsToMove = new List<Field>();
    private int currentFieldIndex;

    public override void Setup(ObjectSpawner objectSpawner, Board board)
    {
        base.Setup(objectSpawner, board);
        InitPlatforms(objectSpawner.Range);

        //Set start pos
        currentFieldIndex = objectSpawner.StartFieldIndex - 1;
        MovePlatform();
    }

    public override void ExecuteGetSignal()
    {
        MovePlatform();
    }

    private void MovePlatform()
    {
        Field currentStandField = board.Fields[LogicPos];
        Field nextFieldStand;

        currentStandField.ClosePathFromField(true);

        currentFieldIndex++;
        if (currentFieldIndex >= fieldsToMove.Count)
        {
            if (fieldsToMove.Count == 1)
            {
                currentFieldIndex = 0;
            }
            else
            {
                currentFieldIndex = 1;
                fieldsToMove.Reverse();
            }
        }

        nextFieldStand = fieldsToMove[currentFieldIndex];
        nextFieldStand.ClosePathFromField(false);

        board.TeleportActionObject(this, nextFieldStand);
        board.TeleportPlayerOnPlatform(nextFieldStand, currentStandField, false);
        board.TeleportEnemyFromField(currentStandField, nextFieldStand);

    }

    private void InitPlatforms(int range)
    {
        if(range < 0)
        {
            range = 0;
        }

        fieldsToMove.Clear();
        fieldsToMove.Add(board.Fields[LogicPos]);
        currentFieldIndex = 0;
        if (range > 0)
        {
            for (int i = 0; i < range; i++)
            {
                Field newField = board.GetFieldFromCurrentField(fieldsToMove[i].LogicPosition, StartLookDirection);

                if(newField == null)
                {
                    break;
                }

                fieldsToMove.Add(newField);
            }
        }

        foreach (var field in fieldsToMove)
        {
            field.SetActive(false);
        }
    }
}
