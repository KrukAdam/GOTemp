using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveController : MonoBehaviour
{
    public event Action<Field> Moved = delegate { };

    public Vector2Int LogicPos 
    {
        get;
        private set;
    }
    public Field CurrentStandField 
    {
        get;
        private set;
    }

    private Board board;
    private EDirection lookDirection;

    public void Setup(Board board, Vector2Int logicPos)
    {
        this.board = board;
        this.LogicPos = logicPos;

        CurrentStandField = board.StartField;

        SetCanMove(true);
    }

    public void SetCanMove(bool canMove)
    {
        if (canMove)
        {
            GameManager.Instance.InputManager.InputActions.Player.Movement.performed += MoveClick;
        }
        else
        {
            GameManager.Instance.InputManager.InputActions.Player.Movement.performed -= MoveClick;
        }
    }

    public void TeleportToField(Field field, bool nextTurn)
    {
        transform.position = field.StandTransform.position;

        LogicPos = field.LogicPosition;

        CurrentStandField = field;

        if (nextTurn)
        {
            Moved(field);
        }
    }

    public void SetMoveAtLookDirection(EDirection directionLook)
    {
        lookDirection = directionLook;
    }

    private void MoveClick(InputAction.CallbackContext ctx)
    {
        Vector2 inputDirection = ctx.ReadValue<Vector2>();
        EDirection moveDirection = EDirection.None;

        if (inputDirection.y > 0)
        {
            moveDirection = EDirection.Top;
        }
        else if (inputDirection.y < 0)
        {
            moveDirection = EDirection.Down;
        }
        else if (inputDirection.x < 0)
        {
            moveDirection = EDirection.Left;
        }
        else if (inputDirection.x > 0)
        {
            moveDirection = EDirection.Right;
        }

        switch (lookDirection)
        {
            case EDirection.None:
                break;
            case EDirection.Left:
                moveDirection = Direction.GetRightFromDirection(moveDirection);
                break;
            case EDirection.Right:
                moveDirection = Direction.GetLeftFromDirection(moveDirection);
                break;
            case EDirection.Top:
                moveDirection = Direction.GetOppositeDirection(moveDirection);
                break;
            case EDirection.Down:
                break;
            case EDirection.Count:
                break;
            default:
                break;
        }

        Move(moveDirection);
    }

    private void Move(EDirection direction)
    {
        if (HaveFanPassage(direction))
        {
            return;
        }

        Field currentStandField = board.Fields[LogicPos];
        FieldPath fieldPath = currentStandField.GetPath(direction) as FieldPath;

        if (fieldPath != null && !fieldPath.IsClosed())
        {
            //Check path object
            if (fieldPath.ActionObject != null)
            {
                fieldPath.ActionObject.ExecuteSendSignal();
            }

            Field nextField = fieldPath.GetNextField(currentStandField) as Field;
            if (nextField != null)
            {
                transform.position = nextField.StandTransform.position;

                LogicPos = nextField.LogicPosition;

                CurrentStandField = nextField;
                Moved(nextField);
            }
        }
    }

    private bool HaveFanPassage(EDirection moveDirection)
    {
        ActionObjectFanPassage fan = CurrentStandField.ActionObject as ActionObjectFanPassage;
        if (fan != null)
        {
           if(fan.MoveOnFan(moveDirection, out ActionObject childFan))
            {
                TeleportToField(board.Fields[childFan.LogicPos], true);
                return true;
            }
        }

        return false;
    }
}
