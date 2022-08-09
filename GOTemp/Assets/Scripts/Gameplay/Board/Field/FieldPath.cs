using UnityEngine;

public class FieldPath : FieldPathBase
{
    [field: SerializeField]
    public Transform StandTransform 
    {
        get;
        private set;
    } = null;
    
}
