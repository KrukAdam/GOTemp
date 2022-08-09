public class EnemyTowerMovement : EnemyMovement
{

    public override void SpecyficMove()
    {
        EDirection newLookDirection = Direction.GetOppositeDirection(lookDirection);
        Field newLookField = board.GetFieldFromCurrentField(logicPos, newLookDirection);

        if (currentStandField.GetPath(newLookDirection) != null)
        {
            SetLookEvent(newLookField);
        }
        else
        {
            RemoveLookEvent(currentLookField);
        }

        currentLookField = newLookField;
        SetLookDirection(newLookDirection);
    }
}
