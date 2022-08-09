public class EnemyRunnerMovement : EnemyMovement
{
    public override void SpecyficMove()
    {
        Field nextStandField = board.GetFieldFromCurrentField(logicPos, lookDirection);
        Field nextLookField;
        EDirection nextLookDirection = lookDirection;
        FieldPathBase path = currentStandField.GetPath(lookDirection);

        if (nextStandField == null || path == null)
        {
            SetLookDirection(Direction.GetOppositeDirection(lookDirection));
            nextStandField = board.GetFieldFromCurrentField(logicPos, lookDirection);
            path = currentStandField.GetPath(lookDirection);

            if (nextStandField == null || path == null) return;
        }

        //Check door
        if (path != null && path.IsClosed())
        {
            SetLookDirection(Direction.GetOppositeDirection(lookDirection));
            nextStandField = board.GetFieldFromCurrentField(logicPos, lookDirection);

            if (nextStandField == null) return;
        }

        //Move
        MoveToField(nextStandField);
        nextLookField = board.GetFieldFromCurrentField(logicPos, lookDirection);
        path = currentStandField.GetPath(lookDirection);

        //Set Look Field
        if (nextLookField == null || path == null)
        {
            SetLookDirection(Direction.GetOppositeDirection(lookDirection));
            nextLookField = board.GetFieldFromCurrentField(logicPos, lookDirection);

            if (nextLookField == null) return;
        }
        else
        {
            SetLookDirection(nextLookDirection);
        }

        ////Check door on next look field
        if (path != null)
        {
            ActionObjectDoor door = path.ActionObject as ActionObjectDoor;
            if (door != null && door.IsClosed)
            {
                SetLookDirection(Direction.GetOppositeDirection(lookDirection));
                nextStandField = board.GetFieldFromCurrentField(logicPos, lookDirection);

                if (nextStandField == null) return;
            }
        }

        //Set look event
        SetLookEvent(nextLookField);
    }
}
