using GambitUtils.UI;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public EActionObjectType ObjectType = EActionObjectType.None;

    [ConditionalHide("showLookDirection", true)]
    public EDirection StartLookDirection = EDirection.Top;
    [ConditionalHide("showSecondLookDirection", true)]
    public EDirection SecondLookDirection = EDirection.Down;
    [ConditionalHide("showAmountLookField", true)]
    public int Range = 3;
    [ConditionalHide("showStartIndexField", true)]
    public int StartFieldIndex = 0;
    [ConditionalHide("showIsOn", true)]
    public bool IsOn = true;
    [ConditionalHide("showFields", true)]
    public List<Field> Fields = new List<Field>();
    [ConditionalHide("showActionObjectInteraction", true)]
    public ObjectSpawner ActionObjectSpawnerInteraction = null;
    [ConditionalHide("showSignalsSpawners", true)]
    public ObjectSpawner ActionObjectSpawnerSignal1 = null;
    [ConditionalHide("showSignalsSpawners", true)]
    public ObjectSpawner ActionObjectSpawnerSignal2 = null;

    [HideInInspector]
    public FieldGrid GridToSpawn = null;

    //use to ConditionalHide
    [SerializeField, HideInInspector]
    protected bool showLookDirection = false;
    [SerializeField, HideInInspector]
    protected bool showSecondLookDirection = false;
    [SerializeField, HideInInspector]
    protected bool showAmountLookField = false;
    [SerializeField, HideInInspector]
    protected bool showIsOn = false;
    [SerializeField, HideInInspector]
    protected bool showActionObjectInteraction = false;
    [SerializeField, HideInInspector]
    protected bool showFields = false;
    [SerializeField, HideInInspector]
    protected bool showSignalsSpawners = false;
    [SerializeField, HideInInspector]
    protected bool showStartIndexField = false;

    public virtual void SetupObjectData()
    {
        Fields.Clear();
        showLookDirection = false;
        showSecondLookDirection = false;
        showAmountLookField = false;
        showIsOn = false;
        showActionObjectInteraction = false;
        showFields = false;
        showSignalsSpawners = false;
        showStartIndexField = false;

        switch (ObjectType)
        {
            case EActionObjectType.None:
                break;
            case EActionObjectType.Switch:
                showIsOn = true;
                showSignalsSpawners = true;
                break;
            case EActionObjectType.PressurePlate:
                showSignalsSpawners = true;
                break;
            case EActionObjectType.StillCamera:
                showLookDirection = true;
                showAmountLookField = true;
                showIsOn = true;
                showSignalsSpawners = true;
                break;
            case EActionObjectType.RotatingCamera:
                showLookDirection = true;
                showSecondLookDirection = true;
                showAmountLookField = true;
                showSignalsSpawners = true;
                break;
            case EActionObjectType.Laser:
                showSignalsSpawners = true;
                showIsOn = true;
                break;
            case EActionObjectType.Alarm:
                break;
            case EActionObjectType.Doors:
                showLookDirection = true;
                showIsOn = true;
                break;
            case EActionObjectType.FanPassage:
                showActionObjectInteraction = true;
                showLookDirection = true;
                break;
            case EActionObjectType.ConveyorBelt:
                showActionObjectInteraction = true;
                showSignalsSpawners = true;
                showLookDirection = true;
                showSecondLookDirection = true;
                break;
            case EActionObjectType.Platform:
                showLookDirection = true;
                showAmountLookField = true;
                showStartIndexField = true;
                break;
            case EActionObjectType.Terminal:
                showLookDirection = true;
                break;
            case EActionObjectType.Decoy:
               // showAmountLookField = true;
                break;
            case EActionObjectType.Count:
                break;
            default:
                break;
        }
    }

}
