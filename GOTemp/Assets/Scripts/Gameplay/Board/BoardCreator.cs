using System.Collections.Generic;
using UnityEngine;
using GambitUtils;

[ExecuteInEditMode]
public class BoardCreator : MonoBehaviour
{
    [SerializeField]
    private FieldGrid gridFieldPrefab = null;
    [SerializeField] 
    private Field fieldPrefab = null;
    [SerializeField] 
    private Transform gridParent = null;
    [SerializeField] 
    private Transform fieldsParent = null;
    [SerializeField]
    private Transform pathParent = null;
    [SerializeField]
    private Transform pathGridParent = null;
    [SerializeField]
    private Transform enemySpawnersParent = null;
    [SerializeField]
    private Transform objectSpawnersParent = null;
    [SerializeField] 
    private Transform objectPathSpawnersParent = null;
    [SerializeField]
    private FieldPath horizontalPath = null;
    [SerializeField] 
    private FieldPath verticalPath = null;
    [SerializeField] 
    private FieldPathGrid horizontalPathGrid = null;
    [SerializeField] 
    private FieldPathGrid verticalPathGrid = null;
    [SerializeField]
    private EnemySpawner enemySpawnerPrefab = null;
    [SerializeField]
    private ObjectSpawner objectSpawnerPrefab = null;
    [SerializeField]
    private ObjectPathSpawner objectPathSpawnerPrefab = null;

    [Header("--------------------------BOARD SETTINGS--------------------------")]
    [SerializeField] 
    private Vector2Int boardGridSize = new Vector2Int(3, 3);
    [SerializeField]
    private float spaceBetweenFields = 2;

    public int GetLenghtFromLongestSide()
    {
        if(boardGridSize.x > boardGridSize.y)
        {
            return boardGridSize.x;
        }
        else
        {
            return boardGridSize.y;
        }
    }

    public FieldBase GetFarthestFieldGrid()
    {
        return GetGridFields()[new Vector2Int(boardGridSize.x-1,boardGridSize.y-1)];
    }

    public float GetSpaceBetweenFields()
    {
        return spaceBetweenFields;
    }

    public void CreateEnemySpawner(FieldGrid grid)
    {
        EnemySpawner enemySpawner = Instantiate(enemySpawnerPrefab, enemySpawnersParent);
        Vector3 spawnerPos = new Vector3(grid.transform.position.x, grid.transform.position.y + 1, grid.transform.position.z);
        enemySpawner.transform.position = spawnerPos;
        enemySpawner.GridToSpawn = grid;

        grid.EnemySpawner = enemySpawner;
    }

    public void CreateObjectSpawner(FieldGrid grid)
    {
        ObjectSpawner objectSpawner = Instantiate(objectSpawnerPrefab, objectSpawnersParent);
        Vector3 spawnerPos = new Vector3(grid.transform.position.x, grid.transform.position.y + 1, grid.transform.position.z);
        objectSpawner.transform.position = spawnerPos;
        objectSpawner.GridToSpawn = grid;

        grid.ObjectSpawner = objectSpawner;
    }

    public void CreatePathObjectSpawner(FieldPathGrid pathGrid)
    {
        ObjectPathSpawner objectSpawner = Instantiate(objectPathSpawnerPrefab, objectPathSpawnersParent);
        Vector3 spawnerPos = new Vector3(pathGrid.transform.position.x, pathGrid.transform.position.y + 1, pathGrid.transform.position.z);
        objectSpawner.transform.position = spawnerPos;
        objectSpawner.PathGridToSpawn = pathGrid;

        pathGrid.ObjectPathSpawner = objectSpawner;
    }

    public void DestroyEnemySpawner(FieldGrid grid)
    {
        DestroyImmediate(grid.EnemySpawner.gameObject);
    }

    public void DestroyObjectSpawner(FieldGrid grid)
    {
        DestroyImmediate(grid.ObjectSpawner.gameObject);
    }

    public void DestroyPathObjectSpawner(FieldPathGrid path)
    {
        DestroyImmediate(path.ObjectPathSpawner.gameObject);
    }

    public FieldGrid GetStartFieldGrid()
    {
        foreach (var field in GetGridFields())
        {
            if (field.Value.Field != null)
            {
                if (field.Value.Field.IsStartField)
                {
                    return field.Value;
                }
            }
        }

        return null;
    }

    public Dictionary<Vector2Int, Field> GetFields()
    {
        Dictionary<Vector2Int, Field> fields = new Dictionary<Vector2Int, Field>();
        fields.Clear();
        Field[] fieldsTab = fieldsParent.gameObject.GetComponentsInChildren<Field>();
        foreach (var field in fieldsTab)
        {
            fields.Add(field.LogicPosition, field);
        }

        return fields;
    }

    public Dictionary<Vector2Int, FieldGrid> GetGridFields()
    {
        Dictionary<Vector2Int, FieldGrid> fieldsGrid = new Dictionary<Vector2Int, FieldGrid>();
        fieldsGrid.Clear();
        FieldGrid[] fieldsGridTab = gridParent.gameObject.GetComponentsInChildren<FieldGrid>();
        foreach (var grid in fieldsGridTab)
        {
            fieldsGrid.Add(grid.LogicPosition, grid);
        }

        return fieldsGrid;
    }

    public void SetGridVisible()
    {
        gridParent.SetGameObjectActive(!gridParent.gameObject.activeSelf);
    }

    public void UpdateSettings()
    {
        FieldPathGrid[] pathGrid = pathGridParent.gameObject.GetComponentsInChildren<FieldPathGrid>();
        foreach (var path in pathGrid)
        {
            path.gameObject.layer = 6;
            path.PathObject.layer = 6;
        }

        fieldsParent.localPosition = Vector3.zero;
        pathParent.localPosition = Vector3.zero;
        pathGridParent.localPosition = Vector3.zero;
        enemySpawnersParent.localPosition = Vector3.zero;
        objectSpawnersParent.localPosition = Vector3.zero;
        objectPathSpawnersParent.localPosition = Vector3.zero;

        fieldsParent.rotation = Quaternion.identity;
        pathParent.rotation = Quaternion.identity;
        pathGridParent.rotation = Quaternion.identity;
        enemySpawnersParent.rotation = Quaternion.identity;
        objectSpawnersParent.rotation = Quaternion.identity;
        objectPathSpawnersParent.rotation = Quaternion.identity;
    }

    public void SetSpaceBetweenFields()
    {
        Vector3 space;
        foreach (var fieldGrid in GetGridFields())
        {
            Vector2Int logicPos = fieldGrid.Key;
            FieldGrid fieldGridValue = fieldGrid.Value;
            space = new Vector3((logicPos.x * spaceBetweenFields), 0, (logicPos.y * spaceBetweenFields));
            fieldGridValue.gameObject.transform.localPosition = space;

            ObjectSpawner oSpawner = fieldGridValue.ObjectSpawner;
            EnemySpawner eSpawner = fieldGridValue.EnemySpawner;
            Vector3 spawnerPos = new Vector3(fieldGridValue.transform.position.x, fieldGridValue.transform.position.y + 1, fieldGridValue.transform.position.z);
            if (oSpawner != null)
            {
                oSpawner.transform.position = spawnerPos;
            }
            if(eSpawner != null)
            {
                eSpawner.transform.position = spawnerPos;
            }
        }

        foreach (var field in GetFields())
        {
            Vector2Int logicPos = field.Key;
            space = new Vector3((logicPos.x * spaceBetweenFields), 0, (logicPos.y * spaceBetweenFields));
            field.Value.gameObject.transform.localPosition = space;

            if(field.Value.ActionObject != null)
            {
                field.Value.ActionObject.transform.position = field.Value.StandTransform.position;
            }
        }

        FieldPath[] fieldPaths = pathParent.gameObject.GetComponentsInChildren<FieldPath>();
        foreach (var path in fieldPaths)
        {
            Vector3 pathPos = path.GetPositionFields() / 2;

            switch (path.GetDirectionFromFirstData())
            {
                case EDirection.None:
                    break;
                case EDirection.Left:
                case EDirection.Right:
                    path.PathObject.transform.localScale = new Vector3(spaceBetweenFields, path.PathObject.transform.localScale.y, path.PathObject.transform.localScale.z);
                    break;
                case EDirection.Top:
                case EDirection.Down:
                    path.PathObject.transform.localScale = new Vector3(path.PathObject.transform.localScale.x, path.PathObject.transform.localScale.y, spaceBetweenFields);
                    break;
                case EDirection.Count:
                    break;
                default:
                    break;
            }

            path.transform.position = pathPos;
        }


        FieldPathGrid[] pathGrid = pathGridParent.gameObject.GetComponentsInChildren<FieldPathGrid>();
        foreach (var path in pathGrid)
        {
            Vector3 pathPos = path.GetPositionFields() / 2;

            switch (path.GetDirectionFromFirstData())
            {
                case EDirection.None:
                    break;
                case EDirection.Left:
                case EDirection.Right:
                    path.PathObject.transform.localScale = new Vector3(spaceBetweenFields, path.PathObject.transform.localScale.y, path.PathObject.transform.localScale.z);
                    break;
                case EDirection.Top:
                case EDirection.Down:
                    path.PathObject.transform.localScale = new Vector3(path.PathObject.transform.localScale.x, path.PathObject.transform.localScale.y, spaceBetweenFields);
                    break;
                case EDirection.Count:
                    break;
                default:
                    break;
            }

            path.transform.position = pathPos;

            Vector3 spawnerPos = new Vector3(pathPos.x, pathPos.y + 1, pathPos.z);
            ObjectSpawner objectSpawner = path.ObjectPathSpawner;
            if(objectSpawner != null)
            {
                objectSpawner.transform.position = spawnerPos;
            }
        }
    }

    public List<EnemySpawner> GetEnemySpawners()
    {
        EnemySpawner[] enemySpawners = enemySpawnersParent.gameObject.GetComponentsInChildren<EnemySpawner>();
        List<EnemySpawner> spawners = new List<EnemySpawner>();
        foreach (var spawner in enemySpawners)
        {
            spawners.Add(spawner);
        }

        return spawners;
    }

    public List<ObjectSpawner> GetObjectSpawners()
    {
        ObjectSpawner[] objectSpawners = objectSpawnersParent.gameObject.GetComponentsInChildren<ObjectSpawner>();
        ObjectSpawner[] objectSpawnersPath = objectPathSpawnersParent.gameObject.GetComponentsInChildren<ObjectSpawner>();
        List<ObjectSpawner> spawners = new List<ObjectSpawner>();
        foreach (var spawner in objectSpawners)
        {
            spawners.Add(spawner);
        }
        foreach (var spawner in objectSpawnersPath)
        {
            spawners.Add(spawner);
        }

        return spawners;
    }

    public void SetupGrid()
    {
        Vector3 space;

        //Clear old grid
        FieldGrid[] oldGrid = gridParent.gameObject.GetComponentsInChildren<FieldGrid>();
        FieldPath[] oldPath = pathParent.gameObject.GetComponentsInChildren<FieldPath>();
        FieldPathBase[] oldPathGrid = pathGridParent.gameObject.GetComponentsInChildren<FieldPathBase>();
        EnemySpawner[] enemySpawners = enemySpawnersParent.gameObject.GetComponentsInChildren<EnemySpawner>();
        ObjectSpawner[] objectSpawners = objectSpawnersParent.gameObject.GetComponentsInChildren<ObjectSpawner>();
        ObjectSpawner[] objectSpawnersPath = objectPathSpawnersParent.gameObject.GetComponentsInChildren<ObjectSpawner>();

        foreach (var oldField in oldGrid)
        {
            oldField.DestroyField();
            DestroyImmediate(oldField.gameObject);
        }

        foreach (var oldP in oldPath)
        {
            RemovePath(oldP);
        }

        foreach (var oldP in oldPathGrid)
        {
            RemovePath(oldP);
        }

        foreach (var spawner in enemySpawners)
        {
            if (spawner != null)
            {
                DestroyImmediate(spawner.gameObject);
            }
        }

        foreach (var spawner in objectSpawners)
        {
            if (spawner != null)
            {
                DestroyImmediate(spawner.gameObject);
            }
        }


        foreach (var spawner in objectSpawnersPath)
        {
            if (spawner != null)
            {
                DestroyImmediate(spawner.gameObject);
            }
        }


        //Create new grid
        for (int x = 0; x < boardGridSize.x; x++)
        {
            for (int y = 0; y < boardGridSize.y; y++)
            {
                space = new Vector3((x * spaceBetweenFields), 0, (y * spaceBetweenFields));

                FieldGrid newField = Instantiate(gridFieldPrefab, gridParent);
                newField.gameObject.transform.localPosition = space;
                newField.Setup(new Vector2Int(x, y));

                if (x == 0 && y == 0)
                {
                    CreateFirstStartField(newField);

                }
            }
        }
    }

    public void CreateFirstStartField(FieldGrid grid)
    {
        grid.SetBusy();
        if (grid.IsBusy)
        {
            grid.SetField(CreateField(grid.transform, grid.LogicPosition));
        }

        grid.SetStartField();
    }

    public Field CreateField(Transform spawnPos, Vector2Int logicPos)
    {
        Vector3 spawnPosition = new Vector3(spawnPos.position.x, 0, spawnPos.position.z);
        Field newField = Instantiate(fieldPrefab, fieldsParent);
        newField.transform.position = spawnPosition;
        newField.Setup(logicPos);

        return newField;
    }

    public void RemovePath(FieldBase field, EDirection fieldPathDirection)
    {
        FieldPathBase path = field.GetPath(fieldPathDirection);

        if (path == null)
        {
            return;
        }

        path.OnRemovePath();
        DestroyImmediate(path.gameObject);
    }

    public void RemovePath(FieldPathBase path)
    {
        if (path == null)
        {
            return;
        }

        path.OnRemovePath();
        DestroyImmediate(path.gameObject);
    }

    public void RemoveAllPathFromField(FieldBase field)
    {
        if (field == null)
        {
            return;
        }

        for (EDirection i = EDirection.None + 1; i < EDirection.Count; i++)
        {
            FieldPathBase pathToDestroy = field.GetPath(i);
            if (pathToDestroy != null)
            {
                pathToDestroy.OnRemovePath();
                DestroyImmediate(pathToDestroy.gameObject);
            }
        }
    }

    public FieldPath SetPathField(Vector2Int field1Pos, Vector2Int field2Pos, EDirection fieldPathDirection)
    {
        Dictionary<Vector2Int, Field> fields = GetFields();

        if (fields.TryGetValue(field1Pos, out Field fieldFirst))
        {
            if (fields.TryGetValue(field2Pos, out Field fieldSecond))
            {
                if (fieldFirst.GetPath(fieldPathDirection) != null)
                {
                    return null;
                }

                Vector3 pathPos = (fieldFirst.transform.position + fieldSecond.transform.position) / 2;
                FieldPath path = null;
                FieldPathData dataPathFirst = new FieldPathData();
                FieldPathData dataPathSecond = new FieldPathData();

                dataPathFirst.Field = fieldFirst;
                dataPathFirst.FieldPathDirection = fieldPathDirection;
                dataPathSecond.Field = fieldSecond;
                dataPathSecond.FieldPathDirection = Direction.GetOppositeDirection(fieldPathDirection);

                switch (fieldPathDirection)
                {
                    case EDirection.None:
                        break;
                    case EDirection.Left:
                        path = Instantiate(horizontalPath, pathParent);
                        path.PathObject.transform.localScale = new Vector3(spaceBetweenFields, path.PathObject.transform.localScale.y, path.PathObject.transform.localScale.z);
                        break;
                    case EDirection.Right:
                        path = Instantiate(horizontalPath, pathParent);
                        path.PathObject.transform.localScale = new Vector3(spaceBetweenFields, path.PathObject.transform.localScale.y, path.PathObject.transform.localScale.z);
                        break;
                    case EDirection.Top:
                        path = Instantiate(verticalPath, pathParent);
                        path.PathObject.transform.localScale = new Vector3(path.PathObject.transform.localScale.x, path.PathObject.transform.localScale.y, spaceBetweenFields);
                        break;
                    case EDirection.Down:
                        path = Instantiate(verticalPath, pathParent);
                        path.PathObject.transform.localScale = new Vector3(path.PathObject.transform.localScale.x, path.PathObject.transform.localScale.y, spaceBetweenFields);
                        break;
                    case EDirection.Count:
                        break;
                    default:
                        break;
                }

                path.transform.position = pathPos;

                path.Setup(dataPathFirst, dataPathSecond);

                fieldFirst.SetPath(path, fieldPathDirection);
                fieldSecond.SetPath(path, dataPathSecond.FieldPathDirection);

                return path;
            }
        }

        return null;
    }

    public FieldPathGrid SetPathGrid(Vector2Int field1Pos, Vector2Int field2Pos, EDirection fieldPathDirection)
    {
        Dictionary<Vector2Int, FieldGrid> grid = GetGridFields();
        Dictionary<Vector2Int, Field> fields = GetFields();

        if (!fields.TryGetValue(field1Pos, out Field _) || !fields.TryGetValue(field2Pos, out Field _))
        {
            return null;
        }

        if (grid.TryGetValue(field1Pos, out FieldGrid fieldFirst))
        {
            if (grid.TryGetValue(field2Pos, out FieldGrid fieldSecond))
            {
                if (fieldFirst.GetPath(fieldPathDirection) != null || fieldSecond.GetPath(Direction.GetOppositeDirection(fieldPathDirection)) != null)
                {
                    Debug.Log("Have Path Here");
                    return null;
                }

                Vector3 pathPos = (fieldFirst.transform.position + fieldSecond.transform.position) / 2;
                FieldPathGrid pathGrid = null;
                FieldPathData dataPathFirst = new FieldPathData();
                FieldPathData dataPathSecond = new FieldPathData();

                dataPathFirst.Field = fieldFirst;
                dataPathFirst.FieldPathDirection = fieldPathDirection;
                dataPathSecond.Field = fieldSecond;
                dataPathSecond.FieldPathDirection = Direction.GetOppositeDirection(fieldPathDirection);

                switch (fieldPathDirection)
                {
                    case EDirection.None:
                        break;
                    case EDirection.Left:
                        pathGrid = Instantiate(horizontalPathGrid, pathGridParent);
                        pathGrid.PathObject.transform.localScale = new Vector3(spaceBetweenFields, pathGrid.PathObject.transform.localScale.y, pathGrid.PathObject.transform.localScale.z);
                        break;
                    case EDirection.Right:
                        pathGrid = Instantiate(horizontalPathGrid, pathGridParent);
                        pathGrid.PathObject.transform.localScale = new Vector3(spaceBetweenFields, pathGrid.PathObject.transform.localScale.y, pathGrid.PathObject.transform.localScale.z);
                        break;
                    case EDirection.Top:
                        pathGrid = Instantiate(verticalPathGrid, pathGridParent);
                        pathGrid.PathObject.transform.localScale = new Vector3(pathGrid.PathObject.transform.localScale.x, pathGrid.PathObject.transform.localScale.y, spaceBetweenFields);
                        break;
                    case EDirection.Down:
                        pathGrid = Instantiate(verticalPathGrid, pathGridParent);
                        pathGrid.PathObject.transform.localScale = new Vector3(pathGrid.PathObject.transform.localScale.x, pathGrid.PathObject.transform.localScale.y, spaceBetweenFields);
                        break;
                    case EDirection.Count:
                        break;
                    default:
                        break;
                }

                pathGrid.transform.position = pathPos;

                pathGrid.Setup(dataPathFirst, dataPathSecond);

                fieldFirst.SetPath(pathGrid, fieldPathDirection);
                fieldSecond.SetPath(pathGrid, dataPathSecond.FieldPathDirection);

                return pathGrid;
            }
        }

        return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        foreach (var spawner in GetObjectSpawners())
        {
            if(spawner != null)
            {
                if (spawner.ActionObjectSpawnerSignal1 != null)
                {
                    Gizmos.DrawLine(spawner.transform.position, spawner.ActionObjectSpawnerSignal1.transform.position);
                }
                if (spawner.ActionObjectSpawnerSignal2 != null)
                {
                    Gizmos.DrawLine(spawner.transform.position, spawner.ActionObjectSpawnerSignal2.transform.position);
                }
            }
        }
    }
}
