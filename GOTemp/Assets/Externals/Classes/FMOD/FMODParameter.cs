﻿using System.Collections.Generic;

#if !FMOD_DISABLED
public class FMODParameter
{
    public FMOD.Studio.PARAMETER_ID Id { get; private set; }
    public string Name { get; private set; }

    private readonly List<string> labelNames = null;

    public static FMODParameter Create(FMOD.Studio.EventInstance eventInstance, string paramName, List<string> labels = null)
    {
        return new FMODParameter(eventInstance, paramName, labels);
    }

    public FMODParameter(FMOD.Studio.EventInstance eventInstance, string paramName, List<string> labels = null)
    {
        labelNames = labels != null ? new List<string>(labels) : null;
        Name = paramName;
        Id = FMODStudio.GetParameterId(eventInstance, paramName);
    }

    public int GetLabelIndex(string labelName)
    {
        return labelNames != null ? labelNames.FindIndex(label => label.Equals(labelName)) : -1;
    }

    public void SetValue(FMODEvent eventInstance, float paramValue)
    {
        FMODStudio.SetEventParameterValue(eventInstance.Instance, Id, paramValue);
    }
}
#endif