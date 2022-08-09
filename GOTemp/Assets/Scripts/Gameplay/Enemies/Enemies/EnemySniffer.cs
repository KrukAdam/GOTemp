
public class EnemySniffer : Enemy
{

    public void SetPlayerToFallow(Player player)
    {
        EnemySnifferMovement enemySnifferMovement = EnemyMovement as EnemySnifferMovement;
        enemySnifferMovement.SetPlayer(player);
    }

    public override void CheckEvents(Field currentPlayerStandField)
    {
        foreach (var field in EnemyMovement.GetFieldsToCheckEvents())
        {
            foreach (var singleEvent in field.Events)
            {
                EventEnemyLookData eventEnemyLookData = singleEvent as EventEnemyLookData;
                if(eventEnemyLookData != null && field == currentPlayerStandField)
                {
                    singleEvent.Execute();
                }

                EventSnifferLookData eventEnemySnifferData = singleEvent as EventSnifferLookData;
                if (eventEnemySnifferData != null && field == currentPlayerStandField)
                {
                    singleEvent.Execute();
                }

                if (eventEnemyLookData == null && eventEnemySnifferData == null)
                {
                    singleEvent.Execute();
                }
            }
        }
    }
}
