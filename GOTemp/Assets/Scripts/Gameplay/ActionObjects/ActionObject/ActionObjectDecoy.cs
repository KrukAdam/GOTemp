using System.Collections.Generic;
using UnityEngine;

public class ActionObjectDecoy : ActionObject
{
    private List<Vector2Int> targetsLogicPos = new List<Vector2Int>()
        {
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(1, -1),
            new Vector2Int(1, 1),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
        };

    private List<Field> targetFields = new List<Field>();
    private Field standField;

    public override void Setup(ObjectSpawner objectSpawner, Board board)
    {
        base.Setup(objectSpawner, board);

        SetFieldsToUse();
    }

    public override void ExecuteGetSignal()
    {
        UseDecoy();
    }

    private void UseDecoy()
    {
        foreach (var field in targetFields)
        {
            if(field.Enemies.Count > 0)
            {
                foreach (var enemy in field.Enemies)
                {
                    enemy.EnemyMovement.SetDecoyTarget(standField);
                }
            }
        }
    }

    private void SetFieldsToUse()
    {
        standField = board.Fields[LogicPos];

        targetFields.Clear();
        foreach (var targetLogicPos in targetsLogicPos)
        {
            if(board.Fields.TryGetValue((targetLogicPos + LogicPos), out Field newTargetField))
            {
                targetFields.Add(newTargetField);
            }
        }
    }
}
