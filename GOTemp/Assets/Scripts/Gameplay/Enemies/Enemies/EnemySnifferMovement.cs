using System.Collections.Generic;

public class EnemySnifferMovement : EnemyMovement
{
    public bool FallowPlayer
    {
        get;
        set;
    }

    private Player player;
    private Field firstSnifferLookField;
    private Field secondSnifferLookField;

    public override void Setup(EnemySpawner enemySpawner, Board board, Enemy enemy)
    {
        base.Setup(enemySpawner, board, enemy);

        SetLookSnifferEvent(GetSnifferField());
    }

    public override void Move()
    {
        if (IsDecoyField(currentStandField))
        {
            moveTargets.Clear();
            moveTargetsDecoy.Clear();
            targetIndex = 0;
        }

        if (moveTargets.Count > 0 || FallowPlayer)
        {
            MoveToTarget();
        }
        else
        {
            SpecyficMove();
        }

        SetLookSnifferEvent(GetSnifferField());
    }

    public override void MoveToTarget()
    {
        if (FallowPlayer)
        {
            //Refresh path to player
            currentMovePath.Clear();
            moveTargets.Clear();
            moveTargetsDecoy.Clear();
        }
        base.MoveToTarget();
    }

    public override void SpecyficMove()
    {
        SetLookEvent(board.GetFieldFromCurrentField(currentStandField.LogicPosition, lookDirection));
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    public override void SetDecoyTarget(Field targetField)
    {
        base.SetDecoyTarget(targetField);
        FallowPlayer = false;
        RemoveLookSnifferEvent();
    }

    public void SetLookSnifferEvent(Field lookField, bool clearCurrentLook = true)
    {
        if (clearCurrentLook)
        {
            RemoveLookSnifferEvent();
        }

        if (currentLookField != null)
        {
            EventSnifferLookData eventEnemyLook = new EventSnifferLookData(currentLookField, enemy);
            currentLookField.Events.Add(eventEnemyLook);
            firstSnifferLookField = currentLookField;
        }

        if (lookField != null)
        {
            EventSnifferLookData eventEnemyLookOnSniffer = new EventSnifferLookData(lookField, enemy);
            lookField.Events.Add(eventEnemyLookOnSniffer);
            secondSnifferLookField = lookField;

        }
    }

    public override List<Field> GetFieldsToCheckEvents()
    {
        List<Field> fields = new List<Field>();
        fields.Add(currentStandField);
        fields.Add(currentLookField);
        fields.Add(firstSnifferLookField);
        fields.Add(secondSnifferLookField);

        return fields;
    }

    protected override void SetLookEvent(Field lookField, bool clearCurrentLook = true)
    {
        //Can kill only on collision
    }

    protected override Field GetMoveTarget()
    {
        if (moveTargetsDecoy.Count > 0 && !FallowPlayer)
        {
            return base.GetMoveTarget();
        }

        if (FallowPlayer)
        {
            return board.Fields[player.PlayerMoveController.LogicPos];
        }
        else
        {
            return null;
        }
    }

    private void RemoveLookSnifferEvent()
    {
        if (secondSnifferLookField != null)
        {
            EventBaseEnemyData enemyLookData = secondSnifferLookField.GetEnemyEvent(enemy);
            if (enemyLookData != null)
            {
                secondSnifferLookField.Events.Remove(enemyLookData);
            }
        }
        if (firstSnifferLookField != null)
        {
            EventBaseEnemyData enemyLookData = firstSnifferLookField.GetEnemyEvent(enemy);
            if (enemyLookData != null)
            {
                firstSnifferLookField.Events.Remove(enemyLookData);
            }
        }
    }

    private Field GetSnifferField()
    {
        FieldPath path = currentLookField.GetPath(lookDirection) as FieldPath;
        if (path != null && !path.IsClosed())
        {
            return path.GetNextField(currentLookField) as Field;
        }

        return null;
    }
}
