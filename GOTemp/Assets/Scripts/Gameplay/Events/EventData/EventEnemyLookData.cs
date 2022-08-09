using System;

[Serializable]
public class EventEnemyLookData : EventBaseEnemyData
{
    public EventEnemyLookData(Field field, Enemy enemy) : base(field, enemy)
    {
    }

    public override void Execute()
    {
        Enemy.EnemyMovement.MoveToFieldLookDirection(field);
    }
}
