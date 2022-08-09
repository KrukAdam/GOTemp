
public class EnemyPatrolMovement : EnemyMovement
{
    private bool turnLeft;

    public override void Setup(EnemySpawner enemySpawner, Board board, Enemy enemy)
    {
        turnLeft = enemySpawner.TurnLeft;
        base.Setup(enemySpawner, board, enemy);
    }

    public override void SpecyficMove()
    {
        Field nextStandField = board.GetFieldFromCurrentField(logicPos, lookDirection);
        Field nextLookField;
        FieldPathBase path = currentStandField.GetPath(lookDirection);
        bool doorIsClosed = false;

        if (path != null)
        {
            doorIsClosed = path.IsClosed();
        }

        if (nextStandField == null || path == null || doorIsClosed)
        {
            EDirection startLookDirection = lookDirection;
            SetLookPatrolDirection();

            while (startLookDirection != lookDirection)
            {
                nextStandField = board.GetFieldFromCurrentField(logicPos, lookDirection);

                if (nextStandField != null)
                {
                    FieldPathBase currentPath = currentStandField.GetPath(lookDirection);
                    if (currentPath != null && !currentPath.IsClosed())
                    {
                        break;
                    }
                }

                SetLookPatrolDirection();
            }

            if (nextStandField == null)
            {
                return;
            }
        }

        //Move
        MoveToField(nextStandField);
        nextLookField = board.GetFieldFromCurrentField(logicPos, lookDirection);

        //Set next look field
        if (nextLookField == null)
        {
            EDirection startLookDirection = lookDirection;
            SetLookPatrolDirection();

            while (startLookDirection != lookDirection)
            {
                nextStandField = board.GetFieldFromCurrentField(logicPos, lookDirection);

                if (nextStandField != null)
                {
                    FieldPathBase currentPath = currentStandField.GetPath(lookDirection);
                    if (currentPath != null && !currentPath.IsClosed())
                    {
                        break;
                    }
                }

                SetLookPatrolDirection();
            }

            if (nextLookField == null) return;
        }
        else
        {
            SetLookDirection(lookDirection);
        }

        //Set look event
        SetLookEvent(nextLookField);
    }

    protected override void SetLookEvent(Field lookField, bool clearCurrentLook = true)
    {
        //Can kill only on collision
    }

    private void SetLookPatrolDirection()
    {
        if (turnLeft)
        {
            SetLookDirection(Direction.GetLeftFromDirection(lookDirection));
        }
        else
        {
            SetLookDirection(Direction.GetRightFromDirection(lookDirection));
        }
    }

}
