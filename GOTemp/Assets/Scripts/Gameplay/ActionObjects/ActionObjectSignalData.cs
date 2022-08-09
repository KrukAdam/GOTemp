using System;

[Serializable]
public class ActionObjectSignalData
{
    public ActionObject ActionObject;
    public EActionObjectSignalType SignalType = EActionObjectSignalType.None;

    public ActionObjectSignalData(ActionObject actionObject, EActionObjectSignalType signalType)
    {
        ActionObject = actionObject;
        SignalType = signalType;
    }
}
