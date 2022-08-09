using UnityEngine;

public class FieldPathBase : MonoBehaviour
{
    public bool IsClosedPath
    {
        get;
        set;
    } = false;

    public ActionObject ActionObject
    {
        get;
        set;
    }

    [field: SerializeField]
    public GameObject PathObject
    {
        get;
        private set;
    }

    [SerializeField] 
    private FieldPathData fieldDataFirst;
    [SerializeField] 
    private FieldPathData fieldDataSecond;


    public void Setup(FieldPathData fieldFirst, FieldPathData fieldSecond)
    {
        this.fieldDataFirst = fieldFirst;
        this.fieldDataSecond = fieldSecond;
    }

    public Vector3 GetPositionFields()
    {
        return fieldDataFirst.Field.transform.position + fieldDataSecond.Field.transform.position;
    }

    public FieldBase GetNextField(FieldBase currentField)
    {
        if (currentField == fieldDataFirst.Field)
        {
            return fieldDataSecond.Field;
        }
        else
        {
            return fieldDataFirst.Field;
        }
    }

    public bool IsClosed()
    {
        ActionObjectDoor door = ActionObject as ActionObjectDoor;

        if (IsClosedPath)
        {
            return true;
        }

        if (door != null)
        {
            return door.IsClosed;
        }

        return false;
    }

    public void OnRemovePath()
    {
        fieldDataFirst.Field.RemovePath(fieldDataFirst.FieldPathDirection);
        fieldDataSecond.Field.RemovePath(fieldDataSecond.FieldPathDirection);
    }

    public EDirection GetDirectionFromFirstData()
    {
        return fieldDataFirst.FieldPathDirection;
    }
}
