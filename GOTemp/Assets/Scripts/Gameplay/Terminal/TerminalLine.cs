using UnityEngine;

public class TerminalLine : MonoBehaviour
{
    [HideInInspector]
    public bool Drawn = false;

    [field: SerializeField] 
    public LineRenderer LineRenderer
    {
        get;
        private set;
    }

    public void SetPosition(Vector3 startPos, Vector3 endPos)
    {
        LineRenderer.SetPosition(0, startPos);
        LineRenderer.SetPosition(1, endPos);
    }
}
