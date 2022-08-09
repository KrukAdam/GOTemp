using UnityEngine;

public class Enemy : MonoBehaviour
{
    [field: SerializeField] 
    public EnemyMovement EnemyMovement 
    {
        get;
        private set;
    }
    [field: SerializeField] 
    public EnemyVisual EnemyVisual 
    {
        get;
        private set; 
    }

    private EEnemyType enemyType = EEnemyType.None;
    private Board board;

    public virtual void Setup(EnemySpawner enemySpawner, Board board)
    {
        this.board = board;
        enemyType = enemySpawner.EnemyType;

        EnemyMovement.Setup(enemySpawner, board, this);
    }

    public virtual void CheckEvents(Field currentPlayerStandField)
    {
        foreach (var field in EnemyMovement.GetFieldsToCheckEvents())
        {
            if(field != null)
            {
                foreach (var singleEvent in field.Events)
                {
                    EventEnemyLookData eventEnemyLookData = singleEvent as EventEnemyLookData;
                    if (eventEnemyLookData != null && field == currentPlayerStandField)
                    {
                        singleEvent.Execute();
                    }

                    if (eventEnemyLookData == null)
                    {
                        singleEvent.Execute();
                    }
                }
            }
        }
    }
}
