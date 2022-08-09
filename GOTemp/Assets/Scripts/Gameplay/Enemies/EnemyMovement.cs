using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    protected EDirection lookDirection
    {
        get;
        set;
    } = EDirection.None;

    [SerializeField]
    protected Vector2Int logicPos = new Vector2Int();

    protected Board board = null;
    protected Enemy enemy = null;
    protected Field currentStandField = null;
    protected Field currentLookField = null;
    protected List<Field> moveTargets = new List<Field>();
    protected List<Field> moveTargetsDecoy = new List<Field>();
    protected Queue<Field> currentMovePath = new Queue<Field>();
    protected int targetIndex = 0;

    public virtual void Setup(EnemySpawner enemySpawner, Board board, Enemy enemy)
    {
        this.board = board;
        this.enemy = enemy;

        logicPos = enemySpawner.GridToSpawn.LogicPosition;
        //moveTargets = enemySpawner.MoveTarget;

        SetLookDirection(enemySpawner.StartLookDirection);

        currentStandField = board.Fields[logicPos];
        currentStandField.Enemies.Add(enemy);
        currentLookField = board.GetFieldFromCurrentField(logicPos, lookDirection);
        SetLookEvent();
    }

    public virtual void Move()
    {
        if (IsDecoyField(currentStandField))
        {
            moveTargets.RemoveAt(targetIndex);
            moveTargetsDecoy.Remove(currentStandField);
        }

        if (moveTargets.Count > 0)
        {
            MoveToTarget();
        }
        else
        {
            SpecyficMove();
        }
    }

    public virtual void SpecyficMove()
    {

    }

    public void TeleportToField(Field field)
    {
        currentStandField.Enemies.Remove(enemy);

        logicPos = field.LogicPosition;

        currentStandField = board.Fields[logicPos];
        currentStandField.Enemies.Add(enemy);

        transform.position = field.StandTransform.position;
        SetLookEvent(board.GetFieldFromCurrentField(logicPos, lookDirection));
    }

    public virtual bool MoveToFieldLookDirection(Field fieldToMove)
    {
        if (currentStandField.GetPath(lookDirection) != null)
        {
            currentStandField.Enemies.Remove(enemy);

            logicPos = fieldToMove.LogicPosition;

            currentStandField = board.Fields[logicPos];
            currentStandField.Enemies.Add(enemy);

            transform.position = fieldToMove.StandTransform.position;

            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual void MoveToField(Field fieldToMove)
    {
        currentStandField.Enemies.Remove(enemy);

        logicPos = fieldToMove.LogicPosition;

        currentStandField = fieldToMove;
        currentStandField.Enemies.Add(enemy);

        transform.position = fieldToMove.StandTransform.position;
    }

    public virtual void MoveToTarget()
    {
        Queue<Field> path;
        if (currentMovePath.Count <= 0)
        {
            Field targetField = GetMoveTarget();
            if (targetField == null) return;

            path = FindPath(targetField);
            if (path == null) return;

            currentMovePath = path;
        }
        else
        {
            path = currentMovePath;
        }

        Field nextStandField = path.Dequeue();
        Field nextLookField = null;
        EDirection nextLookDirection = lookDirection;
        if (path.Count > 0)
        {
            nextLookField = path.Peek();
            nextLookDirection = nextStandField.GetDirectionToField(nextLookField);
        }
        else
        {
            //Check next target and look at there
            nextLookField = GetNextMoveTarget();
            if (nextLookField == null || nextLookField == nextStandField)
            {
                nextLookDirection = currentStandField.GetDirectionToField(nextStandField);
                nextLookField = board.GetFieldFromCurrentField(nextStandField.LogicPosition, nextLookDirection);
            }
            else
            {
                Queue<Field> pathNextTarget = FindPath(nextLookField, nextStandField);

                if (pathNextTarget != null)
                {
                    nextLookField = pathNextTarget.Peek();
                    nextLookDirection = nextStandField.GetDirectionToField(nextLookField);
                }
            }
        }

        //Check door
        ActionObjectDoor door = currentStandField.GetPath(currentStandField.GetDirectionToField(nextStandField)).ActionObject as ActionObjectDoor;
        if (door != null && door.IsClosed)
        {

            currentMovePath.Clear();
            MoveToTarget();
            return;

            ////Change target if cant go to goal
            //targetIndex = GetNextTargetIndex();
            //if(moveTargets.Count > 1)
            //{
            //    MoveToTarget();
            //}
            //return;
        }

        MoveToField(nextStandField);

        if (nextLookField == null)
        {
            SetLookDirection(Direction.GetOppositeDirection(lookDirection));
            nextLookField = board.GetFieldFromCurrentField(logicPos, lookDirection);

            if (nextLookField == null) return;
        }
        else
        {
            SetLookDirection(nextLookDirection);
        }

        SetLookEvent(nextLookField);
    }

    public virtual void SetDecoyTarget(Field targetField)
    {
        moveTargets.Add(targetField);
        moveTargetsDecoy.Add(targetField);
    }

    public virtual List<Field> GetFieldsToCheckEvents()
    {
        List<Field> fields = new List<Field>();
        fields.Add(currentStandField);
        fields.Add(currentLookField);

        return fields;
    }

    protected virtual Queue<Field> FindPath(Field goal, Field start = null)
    {
        Dictionary<Field, Field> nextFieldGoal = new Dictionary<Field, Field>();
        Dictionary<Field, int> costToRechField = new Dictionary<Field, int>();
        PriorityQueue<Field> frontier = new PriorityQueue<Field>();

        if (start == null)
        {
            start = currentStandField;
        }

        frontier.Enqueue(goal, 0);
        costToRechField.Add(goal, 0);

        while (frontier.Count > 0)
        {
            Field currentField = frontier.Dequeue();

            if (currentField == start)
            {
                break;
            }

            foreach (var neighborField in currentField.GetNeighborsFields())
            {
                EDirection directionToField = currentField.GetDirectionToField(neighborField);
                FieldPathBase fieldPath = currentField.GetPath(directionToField);
                if(fieldPath != null)
                {
                    ActionObjectDoor door = fieldPath.ActionObject as ActionObjectDoor;
                    if (door == null || !door.IsClosed)
                    {
                        int newMoveCost = costToRechField[currentField] + neighborField.MoveCost;
                        if (!costToRechField.ContainsKey(neighborField) || newMoveCost < costToRechField[neighborField])
                        {
                            int priority = newMoveCost + Distance(neighborField, start);
                            costToRechField.Add(neighborField, newMoveCost);
                            frontier.Enqueue(neighborField, priority);
                            nextFieldGoal.Add(neighborField, currentField);
                        }
                    }
                }
            }
        }

        if (!nextFieldGoal.ContainsKey(start))
        {
            return null;
        }

        Queue<Field> path = new Queue<Field>();
        Field currentPathField = start;
        while (currentPathField != goal)
        {
            currentPathField = nextFieldGoal[currentPathField];
            path.Enqueue(currentPathField);
        }

        return path;
    }

    protected virtual void SetLookEvent(Field lookField, bool clearCurrentLook = true)
    {
        if (clearCurrentLook)
        {
            RemoveLookEvent(currentLookField);
        }

        if (lookField != null)
        {
            FieldPath path = currentStandField.GetPath(lookDirection) as FieldPath;
            if(path != null && !path.IsClosed())
            {
                EventEnemyLookData eventEnemyLook = new EventEnemyLookData(lookField, enemy);
                lookField.Events.Add(eventEnemyLook);

                if (clearCurrentLook)
                {
                    currentLookField = lookField;
                }
            }
        }
    }

    protected void SetLookEvent()
    {
        if (currentLookField != null)
        {
            EventEnemyLookData eventEnemyLook = new EventEnemyLookData(currentLookField, enemy);
            currentLookField.Events.Add(eventEnemyLook);
        }
    }

    protected void RemoveLookEvent(Field field)
    {
        if (field != null)
        {
            EventBaseEnemyData enemyLookData = field.GetEnemyEvent(enemy);
            if (enemyLookData != null)
            {
                field.Events.Remove(enemyLookData);
            }
        }
    }

    protected void SetLookDirection(EDirection direction)
    {
        lookDirection = direction;
        enemy.EnemyVisual.RotateBody(direction);
    }

    protected virtual Field GetMoveTarget()
    {
        if (moveTargets.Count <= 0)
        {
            return null;
        }

        if (currentStandField == moveTargets[targetIndex])
        {
            targetIndex++;
        }

        if (moveTargets.Count <= targetIndex)
        {
            targetIndex = 0;
        }

        return moveTargets[targetIndex];
    }

    protected virtual bool IsDecoyField(Field fieldToCheck)
    {
        foreach (var targetDecoy in moveTargetsDecoy)
        {
            if (targetDecoy.LogicPosition == fieldToCheck.LogicPosition)
            {
                return true;
            }
        }

        return false;
    }

    protected virtual Field GetNextMoveTarget()
    {

        if (moveTargets.Count <= 0)
        {
            return null;
        }

        int nextIndex;
        nextIndex = targetIndex;
        nextIndex++;

        if (moveTargets.Count <= nextIndex)
        {
            nextIndex = 0;
        }

        return moveTargets[nextIndex];
    }

    protected virtual int GetNextTargetIndex()
    {
        int nextIndex;
        nextIndex = targetIndex;
        nextIndex++;

        if (moveTargets.Count <= nextIndex)
        {
            nextIndex = 0;
        }

        return nextIndex;
    }

    private int Distance(Field field1, Field field2)
    {
        return Mathf.Abs(field1.LogicPosition.x - field2.LogicPosition.x) + Mathf.Abs(field1.LogicPosition.y - field2.LogicPosition.y);
    }
}
