﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

#if !FMOD_DISABLED
public class FMODEvent
{
    public bool IsInstanceValid => instance.isValid();
    public FMOD.Studio.EventInstance Instance => instance;
    public bool IsPlaying => FMODStudio.IsPlaying(instance);
    public bool IsPaused => FMODStudio.IsPaused(instance);

    public string EventName
    {
        get;
        private set;
    }

    protected FMOD.Studio.EventInstance instance = default;

    protected Transform transform = null;

    private readonly List<FMODParameter> parameters = new List<FMODParameter>();

#if LOG_SOUNDS && UNITY_EDITOR
    private readonly List<int> paramValues = new List<int>();
#endif

    public static bool IsValid(string eventName)
    {
        if (!string.IsNullOrEmpty(eventName))
        {
            FMOD.RESULT eventFoundResult = FMODUnity.RuntimeManager.StudioSystem.getEvent(eventName, out FMOD.Studio.EventDescription eventDesc);
            return eventFoundResult == FMOD.RESULT.OK && eventDesc.isValid();
        }
        return false;
    }

    public static FMODEvent CreateWithParameter<T>(string eventName, string paramName, Transform transform = null) where T : Enum
    {
        return CreateWithParameter(eventName, paramName, null, transform);
    }

    public static FMODEvent CreateWithParameter(string eventName, string paramName, List<string> parameterLabels = null, Transform transform = null)
    {
        if (IsValid(eventName))
        {
            FMODEvent eventInstance = new FMODEvent(eventName, transform);
            eventInstance.AddParameter(paramName, parameterLabels);
            return eventInstance;
        }
        return null;
    }

    public FMODEvent(string eventName, Transform transform = null)
    {
        Release();
        Init(eventName, transform);
    }

    ~FMODEvent()
    {
        Assert.IsFalse(instance.isValid(), EventName);
    }

    public void ReplaceEvent(string eventName, Transform transform = null)
    {
        Init(eventName, transform);
    }

    public void AttachToGameObject(Transform transform)
    {
        this.transform = transform;
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance, transform, null as Rigidbody);
    }

    public FMODParameter AddParameter(string paramName, List<string> labels = null)
    {
        var parameter = FMODParameter.Create(instance, paramName, labels);
        parameters.Add(parameter);
        return parameter;
    }

    public void Play()
    {
#if LOG_SOUNDS && UNITY_EDITOR
        LogEvent();
        paramValues.Clear();
#endif
        FMODStudio.Play(instance, transform);
    }

    public void Play(Enum parameterValue, float volume)
    {
        SetVolume(volume);
        Play(parameterValue);
    }

    public bool PlayWithOneEnum(Enum param)
    {
        if (!FMODStudio.SetEventParameterValue(instance, parameters[0].Id, Convert.ToInt32(param)))
        {
            return false;
        }
#if LOG_SOUNDS && UNITY_EDITOR
        paramValues.Clear();
        paramValues.Add(Convert.ToInt32(param));
#endif
        Play();
        return true;
    }

    public bool Play(params Enum[] parametersValue)
    {
        if (parameters.Count == parametersValue.Length)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                if (!FMODStudio.SetEventParameterValue(instance, parameters[i].Id, Convert.ToInt32(parametersValue[i])))
                {
                    return false;
                }
            }
#if LOG_SOUNDS && UNITY_EDITOR
            paramValues.Clear();
            foreach (var param in parametersValue)
            {
                paramValues.Add(Convert.ToInt32(param));
            }
#endif
            Play();
            return true;
        }
        else
        {
            Debug.LogError("Incorrect parameters length");
            return false;
        }
    }

    public bool Continue(params Enum[] parametersValue)
    {
        if (parameters.Count == parametersValue.Length)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                if (!FMODStudio.SetEventParameterValue(instance, parameters[i].Id, Convert.ToInt32(parametersValue[i])))
                {
                    return false;
                }
            }
#if LOG_SOUNDS && UNITY_EDITOR
            paramValues.Clear();
            foreach (var param in parametersValue)
            {
                paramValues.Add(Convert.ToInt32(param));
            }
#endif
            if (!IsPlaying)
            {
                Play();
            }
            return true;
        }
        else
        {
            Debug.LogError("Incorrect parameters length");
            return false;
        }
    }

    public void Play(int parameterValue)
    {
        if (parameters.Count == 1)
        {
            FMODParameter parameter = parameters[0];
            if (FMODStudio.SetEventParameterValue(instance, parameter.Id, parameterValue))
            {
#if LOG_SOUNDS && UNITY_EDITOR
                paramValues.Clear();
                paramValues.Add(parameterValue);
#endif
                Play();
            }
        }
    }

    public void Play(string labelName)
    {
        if (parameters.Count == 1)
        {
            FMODParameter parameter = parameters[0];
            if (FMODStudio.SetEventParameterValue(instance, parameter.Id, parameter.GetLabelIndex(labelName)))
            {
#if LOG_SOUNDS && UNITY_EDITOR
                paramValues.Clear();
                paramValues.Add(parameter.GetLabelIndex(labelName));
#endif
                Play();
            };
        }
    }

    public void Play(string parameterName, Enum parameterValue)
    {
        Play(parameterName, Convert.ToInt32(parameterValue));
    }

    public void Play(string parameterName, string labelName)
    {
        //Debug.Log(GetType() + ".Play: " + parameterName + " " + labelName);
        Play(parameterName, parameter => parameter.GetLabelIndex(labelName));
    }

    public void Play(string parameterName, float paramValue)
    {
        Play(parameterName, parameter => paramValue);
    }

    public void SetPause(bool pause)
    {
        FMODStudio.SetPause(instance, pause);
    }

    public void Stop(bool allowFadeout = false)
    {
        FMODStudio.Stop(instance, allowFadeout);
    }

    public void SetParameter(string parameterName, float paramValue)
    {
        SetParameter(parameterName, parameter => paramValue, out _);
    }

    public void SetParameter(int index, Enum value)
    {
        FMODStudio.SetEventParameterValue(instance, parameters[index].Id, Convert.ToInt32(value));
    }

    public void SetTimelinePosition(int milliseconds)
    {
        FMODStudio.SetTimelinePosition(instance, milliseconds);
    }

    public void Release()
    {
        Stop();
        if (transform != null)
        {
            FMODUnity.RuntimeManager.DetachInstanceFromGameObject(instance);
        }
        instance.release();
        instance.clearHandle();
    }

    public void SetVolume(float volume)
    {
        if (instance.isValid())
        {
            instance.setVolume(volume);
        }
    }

    public float GetVolume()
    {
        if (instance.isValid())
        {
            return instance.getVolume(out float volume) == FMOD.RESULT.OK ? volume : 0f;
        }
        return 0f;
    }

    protected void Play(string parameterName, Func<FMODParameter, float> getParamValue)
    {
        if (SetParameter(parameterName, getParamValue, out var param))
        {
#if LOG_SOUNDS && UNITY_EDITOR
            paramValues.Clear();
            paramValues.Add((int)getParamValue(param));
#endif
            Play();
        }
    }

    private void Init(string eventName, Transform transform = null)
    {
        EventName = eventName;
        instance = FMODStudio.CreateEventInstance(eventName, transform);
        this.transform = transform;
    }

    private bool SetParameter(string parameterName, Func<FMODParameter, float> getParamValue, out FMODParameter param)
    {
        param = null;
        if (instance.isValid() && parameters.Count > 0)
        {
            param = parameters.Count > 1 ? parameters.Find(p => p.Name.Equals(parameterName)) : parameters[0];
            if (param != null)
            {
                return FMODStudio.SetEventParameterValue(instance, param.Id, getParamValue(param));
            }
        }
        return false;
    }

#if LOG_SOUNDS && UNITY_EDITOR
    private void LogEvent()
    {
        if (DisabledLogSounds.DisabledEvents.Contains(EventName))
        {
            return;
        }
        string text = "";
        foreach (var param in paramValues)
        {
            text += ", ";
            text += param;
        }
        Debug.Log(Time.realtimeSinceStartup.ToString("F3") + "-" + Time.time.ToString("F3") + " " + EventName + text);
    }
#endif
}
#endif
