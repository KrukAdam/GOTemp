using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Dictionary<Vector2Int, Field> Fields 
    {
        get; 
        private set; 
    } = new Dictionary<Vector2Int, Field>();
    public Field StartField 
    {
        get; 
        private set; 
    }

    [SerializeField] 
    private BoardCreator boardCreator = null;

    private LevelController levelController;

    public void Setup(LevelController levelController)
    {
        this.levelController = levelController;

        SetupFieldsDictionary();
        StartField = boardCreator.GetStartFieldGrid().Field;
    }

    public int GetLenghtFromLongestSide()
    {
        return boardCreator.GetLenghtFromLongestSide();
    }

    public float GetSpaceBetweenFields()
    {
        return boardCreator.GetSpaceBetweenFields();
    }

    public FieldBase GetFarthestField()
    {
        return boardCreator.GetFarthestFieldGrid();
    }

    public List<EnemySpawner> GetEnemiesSpawners()
    {
        return boardCreator.GetEnemySpawners();
    }

    public List<ObjectSpawner> GetObjectsSpawners()
    {
        return boardCreator.GetObjectSpawners();
    }

    public void TeleportPlayerOnPlatform(Field fieldToTeleport, Field currentPlatformField, bool nextTurn)
    {
        if(levelController.Player.PlayerMoveController.CurrentStandField == currentPlatformField)
        {
            levelController.Player.PlayerMoveController.TeleportToField(fieldToTeleport, nextTurn);
            levelController.CallEventsField(fieldToTeleport);
        }
    }

    public void TeleportEnemyFromField(Field startField, Field targetField)
    {
        List<Enemy> enemies = startField.Enemies;

        int listCount = enemies.Count;
        for (int i = 0; i < listCount; i++)
        {
            enemies[i].EnemyMovement.TeleportToField(targetField);
        }
    }

    public void TeleportPlayerOnConveyorBelt(Vector2Int fieldPos, bool nextTurn)
    {
        Field fieldToTeleport = Fields[fieldPos];
        Field playerStandField = levelController.Player.PlayerMoveController.CurrentStandField;

        if(fieldToTeleport == playerStandField)
        {
            return;
        }

        levelController.Player.PlayerMoveController.TeleportToField(fieldToTeleport, nextTurn);
        levelController.CallEventsField(fieldToTeleport);

        ActionObjectConveyorBelt nextBelt = fieldToTeleport.ActionObject as ActionObjectConveyorBelt;
        if(nextBelt != null)
        {
            nextBelt.ExecuteSendSignal();
        }
    }

    public void TeleportActionObject(ActionObject actionObject, Field fieldToTeleport)
    {
        if (fieldToTeleport.ActionObject != null)
        {
            return;
        }

        Fields[actionObject.LogicPos].ActionObject = null;
        fieldToTeleport.ActionObject = actionObject;

        actionObject.transform.position = fieldToTeleport.StandTransform.position;
        actionObject.LogicPos = fieldToTeleport.LogicPosition;

    }

    public Dictionary<Field, int> GetFieldsWithMoveCost()
    {
        Dictionary<Field, int> fieldsAndMoveCost = new Dictionary<Field, int>();
        foreach (var field in Fields)
        {
            fieldsAndMoveCost.Add(field.Value, field.Value.MoveCost);
        }

        return fieldsAndMoveCost;
    }

    public Field GetFieldFromCurrentField(Vector2Int logicPos, EDirection direction)
    {
        switch (direction)
        {
            case EDirection.None:
                break;
            case EDirection.Left:
                logicPos = new Vector2Int(logicPos.x - 1, logicPos.y);
                break;
            case EDirection.Right:
                logicPos = new Vector2Int(logicPos.x + 1, logicPos.y);
                break;
            case EDirection.Top:
                logicPos = new Vector2Int(logicPos.x, logicPos.y + 1);
                break;
            case EDirection.Down:
                logicPos = new Vector2Int(logicPos.x, logicPos.y - 1);
                break;
            case EDirection.Count:
                break;
            default:
                break;
        }

        if (Fields.TryGetValue(logicPos, out Field field))
        {
            return field;
        }

        return null;
    }

    private void SetupFieldsDictionary()
    {
        Fields.Clear();
        Fields = boardCreator.GetFields();
    }

}
