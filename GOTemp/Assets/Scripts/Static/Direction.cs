using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Direction
{
    public static EDirection GetOppositeDirection(EDirection direction)
    {
        switch (direction)
        {
            case EDirection.None:
                return EDirection.None;
            case EDirection.Left:
                return EDirection.Right;
            case EDirection.Right:
                return EDirection.Left;
            case EDirection.Top:
                return EDirection.Down;
            case EDirection.Down:
                return EDirection.Top;
            case EDirection.Count:
                return EDirection.None;
            default:
                return EDirection.None;
        }
    }

    public static EDirection GetRightFromDirection(EDirection direction)
    {
        switch (direction)
        {
            case EDirection.None:
                return EDirection.None;
            case EDirection.Left:
                return EDirection.Top;
            case EDirection.Right:
                return EDirection.Down;
            case EDirection.Top:
                return EDirection.Right;
            case EDirection.Down:
                return EDirection.Left;
            case EDirection.Count:
                return EDirection.None;
            default:
                return EDirection.None;
        }
    }

    public static EDirection GetLeftFromDirection(EDirection direction)
    {
        switch (direction)
        {
            case EDirection.None:
                return EDirection.None;
            case EDirection.Left:
                return EDirection.Down;
            case EDirection.Right:
                return EDirection.Top;
            case EDirection.Top:
                return EDirection.Left;
            case EDirection.Down:
                return EDirection.Right;
            case EDirection.Count:
                return EDirection.None;
            default:
                return EDirection.None;
        }
    }
}
