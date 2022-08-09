using UnityEngine;

public class EventSnifferLookData : EventBaseEnemyData
{
    public EventSnifferLookData(Field field, Enemy enemy) : base(field, enemy)
    {
    }

    public override void Execute()
    {
        EnemySnifferMovement snifferMovement = Enemy.EnemyMovement as EnemySnifferMovement;
        snifferMovement.FallowPlayer = true;
    }
}
