using UnityEngine;

public class ActionPathObject : ActionObject
{
    protected FieldPath fieldPath;

    public override void Setup(ObjectSpawner objectSpawner, Board board)
    {
        this.board = board;

        ObjectPathSpawner objectPathSpawner = objectSpawner as ObjectPathSpawner;

        actionObjectVisual.Setup(this);

        StartLookDirection = objectPathSpawner.StartLookDirection;
        isOn = objectPathSpawner.IsOn;
        fieldPath = objectPathSpawner.PathGridToSpawn.FieldPath;
        LogicPos = Vector2Int.zero;

        OnSetup();
    }
}
