using UnityEngine;

public class ActionObjectVisual : MonoBehaviour
{
    [SerializeField]
    private GameObject elementToRotate = null;
    [SerializeField]
    private GameObject elementToEnable = null; //Can be null

    private ActionObject actionObject;

    public void Setup(ActionObject actionObject)
    {
        this.actionObject = actionObject;

        if(actionObject.ActionObjectSignals.Count > 0)
        {
            for (int i = 0; i < actionObject.ActionObjectSignals.Count; i++)
            {
                actionObject.ActionObjectSignals[i].Setup(actionObject.ActionObjectsSignalDatas[i], actionObject);
            }
        }

        ShowDiode(false);
    }

    public void ShowDiode(bool show)
    {
        foreach (var signal in actionObject.ActionObjectSignals)
        {
            signal.Show(show);
        }
    }

    public void EnabledObjectElement(bool isEnabled)
    {
        if (elementToEnable != null)
        {
            elementToEnable.SetActive(isEnabled);
        }
    }

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

        elementToRotate.transform.rotation = Quaternion.Euler(bodyRotation);
    }
}
