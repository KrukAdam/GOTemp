using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FieldBase : MonoBehaviour
{
    public int MoveCost 
    {
        get;
        private set;
    } = 1;

    public Vector2Int LogicPosition
    {
        get => logicPos;
    }

    [SerializeField] 
    protected Dictionary<EDirection, FieldPathBase> paths = new Dictionary<EDirection, FieldPathBase>();

    [SerializeField]
    private Vector2Int logicPos;
    [SerializeField, HideInInspector]
    private List<FieldPathBase> pathList = new List<FieldPathBase>();
    [SerializeField, HideInInspector] 
    private List<EDirection> directionList = new List<EDirection>();
    [SerializeField]
    private GameObject bodyObject = null;

    private void Awake()
    {
        SetupDictionary();
    }

    public void Setup(Vector2Int logicPostion)
    {
        logicPos = logicPostion;
    }

    public void SetActive(bool show)
    {
        bodyObject.SetActive(show);

        foreach (var path in paths)
        {
            path.Value.PathObject.SetActive(show);
            path.Value.IsClosedPath = !show;
        }
    }

    public void ClosePathFromField(bool close)
    {
        foreach (var path in paths)
        {
            path.Value.IsClosedPath = close;
        }
    }

    public void SetPath(FieldPathBase path, EDirection fieldPathDirection)
    {
        if (!directionList.Contains(fieldPathDirection))
        {
            pathList.Add(path);
            directionList.Add(fieldPathDirection);
        }
    }

    public void RemovePath(EDirection fieldPathDirection)
    {
        for (int i = 0; i < directionList.Count; i++)
        {
            if (directionList[i] == fieldPathDirection)
            {
                pathList.RemoveAt(i);
                directionList.RemoveAt(i);
                return;
            }
        }
        //if (directionList.Contains(fieldPathDirection))
        //{
        //    pathList.Remove(paths[fieldPathDirection]);
        //    paths.Remove(fieldPathDirection);
        //    directionList.Remove(fieldPathDirection);
        //}
    }

    public FieldPathBase GetPath(EDirection fieldPathDirection)
    {
        //if (paths.TryGetValue(fieldPathDirection, out FieldPathBase path))
        //{
        //    return path;
        //}

        for (int i = 0; i < directionList.Count; i++)
        {
            if (directionList[i] == fieldPathDirection)
            {
                return pathList[i];
            }
        }

        return null;
    } 

    public EDirection GetDirectionToField(Field targetField)
    {
        foreach (var singlePath in paths)
        {
            if(singlePath.Value.GetNextField(this) == targetField)
            {
                return singlePath.Key;
            }
        }

        return EDirection.None;
    }

    private void SetupDictionary()
    {
        paths.Clear();
        for (int i = 0; i < directionList.Count; i++)
        {
            paths.Add(directionList[i], pathList[i]);
        }
    }

}
