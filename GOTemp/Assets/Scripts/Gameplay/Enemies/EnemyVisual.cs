using UnityEngine;

public class EnemyVisual : MonoBehaviour
{
    public void RotateBody(EDirection direction)
    {
        Vector3 bodyRotation = Vector3.zero;

        switch (direction)
        {
            case EDirection.None:
                break;
            case EDirection.Left:
                bodyRotation = new Vector3(0, -180, 0);
                break;
            case EDirection.Right:
                bodyRotation = new Vector3(0, 0, 0);
                break;
            case EDirection.Top:
                bodyRotation = new Vector3(0, -90, 0);
                break;
            case EDirection.Down:
                bodyRotation = new Vector3(0, 90, 0);
                break;
            case EDirection.Count:
                break;
            default:
                break;
        }

        transform.rotation = Quaternion.Euler(bodyRotation);
    }
}
