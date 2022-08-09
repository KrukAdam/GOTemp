
public class EnemyPairMovement : EnemyMovement
{
    public override void Setup(EnemySpawner enemySpawner, Board board, Enemy enemy)
    {
        base.Setup(enemySpawner, board, enemy);

        EDirection pairLookDirection = Direction.GetOppositeDirection(lookDirection);
        Field lookField = board.GetFieldFromCurrentField(logicPos, pairLookDirection);

        if(currentStandField.GetPath(pairLookDirection) != null)
        {
            SetLookEvent(lookField, false);
        }
    }
}
