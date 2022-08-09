using System.Collections.Generic;
using UnityEngine;

public class ActionObject : MonoBehaviour
{
    public Vector2Int LogicPos 
    {
        get;
        set;
    }
    public EDirection StartLookDirection
    { 
        get; 
        protected set; 
    }

    public ActionObject ActionObjectInterac 
    {
        get; 
        set; 
    }
    public List<ActionObjectSignalData> ActionObjectsSignalDatas 
    {
        get; 
        set; 
    } = new List<ActionObjectSignalData>();

    [field: SerializeField]
    public List<ActionObjectSignal> ActionObjectSignals = new List<ActionObjectSignal>();

    [SerializeField]
    protected ActionObjectVisual actionObjectVisual = null;

    protected Board board;
    protected bool isOn;

    public virtual void Setup(ObjectSpawner objectSpawner, Board board)
    {
        this.board = board;

        StartLookDirection = objectSpawner.StartLookDirection;
        LogicPos = objectSpawner.GridToSpawn.LogicPosition;
        isOn = objectSpawner.IsOn;

        actionObjectVisual.Setup(this);

        OnSetup();
    }

    public virtual void ExecuteSendSignal() { }
    public virtual void ExecuteGetSignal() { }
    public virtual void ExecuteInteraction() { }

    public virtual void SendSignal()
    {
        foreach (var data in ActionObjectsSignalDatas)
        {
            if (data.SignalType == EActionObjectSignalType.Send)
            {
                data.ActionObject.ExecuteGetSignal();
            }
        }
    }

    public virtual void ShowSignalsDiode(bool show)
    {
        actionObjectVisual.ShowDiode(show);
    }

    public virtual ActionObjectSignal GetSignalObject(ActionObject actionObject, EActionObjectSignalType objectSignalType)
    {
        foreach (var signalObject in ActionObjectSignals)
        {
            if (signalObject.SignalData.ActionObject == actionObject && ActionObjectSignal.GetOppositeSignal(signalObject.SignalData.SignalType) == objectSignalType)
            {
                return signalObject;
            }
        }

        return null;
    }

    public bool CheckCanBindSignal(ActionObjectSignalData dataSignal)
    {
        foreach (var signalObject in ActionObjectSignals)
        {
            if (signalObject.SignalData.ActionObject == dataSignal.ActionObject && signalObject.SignalData.SignalType == dataSignal.SignalType)
            {
                return false;
            }
        }
        return true;
    }

    protected virtual void OnSetup() { }

}
