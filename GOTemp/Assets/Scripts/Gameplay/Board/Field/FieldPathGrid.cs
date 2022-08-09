using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldPathGrid : FieldPathBase
{
    public ObjectPathSpawner ObjectPathSpawner
    {
        get;
        set; 
    }

    [field: SerializeField]
    public FieldPath FieldPath
    {
        get;
        private set;
    } = null;

    public void SetFieldPath(FieldPath fieldPath)
    {
        this.FieldPath = fieldPath;
    }
}
