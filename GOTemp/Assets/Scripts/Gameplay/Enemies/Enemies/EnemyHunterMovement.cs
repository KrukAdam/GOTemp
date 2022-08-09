public class EnemyHunterMovement : EnemyRunnerMovement
{

    private Field currentFlashlightLookField;

    public override void Setup(EnemySpawner enemySpawner, Board board, Enemy enemy)
    {
        base.Setup(enemySpawner, board, enemy);

        AddFlashlightLook();
    }

    public override void SpecyficMove()
    {
        AddFlashlightLook();
    }

    private void AddFlashlightLook()
    {
        if(currentFlashlightLookField != null)
        {
            RemoveLookEvent(currentFlashlightLookField);
        }

        EDirection lightDirection = Direction.GetRightFromDirection(lookDirection);
        currentFlashlightLookField = board.GetFieldFromCurrentField(logicPos, lightDirection);

        if (currentStandField.GetPath(lightDirection) != null)
        {
            SetLookEvent(currentFlashlightLookField, false);
        }
    }
}
