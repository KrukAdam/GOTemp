using UnityEngine;

[ExecuteInEditMode]
public class FieldGrid : FieldBase
{
    public EnemySpawner EnemySpawner = null;
    public ObjectSpawner ObjectSpawner = null;

    [field:SerializeField]
    public bool IsBusy 
    {
        get;
        private set;
    } = false;
    [field:SerializeField]
    public Field Field
    {
        get; 
        private set; 
    } = null;

    [SerializeField]
    private MeshRenderer body = null;
    [SerializeField]
    private Material freeMaterial = null;
    [SerializeField]
    private Material busyMaterial = null;
    [SerializeField]
    private Material startFieldMaterial = null;
    [SerializeField]
    private Material wonMaterial = null;
    [SerializeField]
    private Material enemyMaterial = null;
    [SerializeField]
    private Material objectMaterial = null;

    public void SetStartField()
    {
        Field.IsStartField = !Field.IsStartField;

        if (Field.IsStartField)
        {
            body.material = startFieldMaterial;
        }
        else
        {
            body.material = busyMaterial;
        }
    }

    public bool HasEnemySpawner()
    {
        if(EnemySpawner != null)
        {
            return true;
        }

        return false;
    }

    public bool HasObjectSpawner()
    {
        if (ObjectSpawner != null)
        {
            return true;
        }

        return false;
    }

    public void SetEnemyOnField()
    {
        if (EnemySpawner != null)
        {
            body.material = enemyMaterial;
        }
        else
        {
            body.material = busyMaterial;
        }
    }

    public void SetObjectOnField()
    {
        if (ObjectSpawner != null)
        {
            body.material = objectMaterial;
        }
        else
        {
            body.material = busyMaterial;
        }
    }

    public void SetWonField()
    {
        Field.IsWonField = !Field.IsWonField;

        if (Field.IsWonField)
        {
            body.material = wonMaterial;
        }
        else
        {
            body.material = busyMaterial;
        }
    }

    public EDirection IsANeighborGrid(Vector2Int neighborLogicPos)
    {
        if ((LogicPosition.x == neighborLogicPos.x))
        {
            if (((LogicPosition.y - 1) == neighborLogicPos.y))
            {
                return EDirection.Down;
            }

            if (((LogicPosition.y + 1) == neighborLogicPos.y))
            {
                return EDirection.Top;
            }
        }

        if ((LogicPosition.y == neighborLogicPos.y))
        {
            if (((LogicPosition.x - 1) == neighborLogicPos.x))
            {
                return EDirection.Left;
            }

            if (((LogicPosition.x + 1) == neighborLogicPos.x))
            {
                return EDirection.Right;
            }
        }

        return EDirection.None;
    }

    public void SetField(Field field)
    {
        this.Field = field;
    }

    public void DestroyField()
    {
        if(Field != null)
        {
            DestroyImmediate(Field.gameObject);
        }
    }

    public void SetBusy()
    {
        IsBusy = !IsBusy;

        if (IsBusy)
        {
            body.material = busyMaterial;
        }
        else
        {
            body.material = freeMaterial;
        }
    }
}
