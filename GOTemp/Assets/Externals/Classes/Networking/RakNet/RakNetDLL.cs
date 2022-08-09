﻿using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Diagnostics;
using UnityEngine;
using System.Text;
using System.IO;
using AOT;

public class RakNetDLL
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void DataReceived(IntPtr buffer, Int64 length, IntPtr port, IntPtr guid);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void UnityLog(StringBuilder buffer, char priority);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void ForwarderReady(StringBuilder addres, UInt16 port);

    public static event Action<string, UInt16> OnForwarderReady = delegate { };

    //TODO: Better name + use DLL x86 on x86 platform
#if UNITY_EDITOR

    public const string DLL_NAME = "RakNet_VS2008_DLL_Release_x64.dll";
#else
    public const string DLL_NAME = "RakNet_VS2008_DLL_Release_x64.dll";
#endif

    [DllImport(DLL_NAME, EntryPoint = "GetCurrentInstance")]
    private static extern IntPtr _GetCurrentInstance();

    public static IntPtr GetCurrentInstance()
    {
        IntPtr ptr = IntPtr.Zero;

        try
        {
            ptr = _GetCurrentInstance();
        }
        catch
        {
            UnityEngine.Debug.LogError("Error in " + DLL_NAME + " in " + new StackFrame().GetMethod().Name);
        }
        return ptr;
    }

    [DllImport(DLL_NAME, EntryPoint = "DestroyInstance")]
    private static extern void _DestroyInstance(IntPtr instance);

    public static void DestroyInstance(IntPtr instance)
    {
        try
        {
            _DestroyInstance(instance);
        }
        catch
        {
            UnityEngine.Debug.LogError("Error in " + DLL_NAME + " in " + new StackFrame().GetMethod().Name);
        }
    }

    [DllImport(DLL_NAME, EntryPoint = "StartLocalClient")]
    private static extern RakNetEnums.StartupResult _StartLocalClient(IntPtr instance, Int32 clientPort);

    public static RakNetEnums.StartupResult StartLocalClient(IntPtr instance, Int32 clientPort)
    {
        RakNetEnums.StartupResult result = RakNetEnums.StartupResult.STARTUP_OTHER_FAILURE;
        try
        {
            result = _StartLocalClient(instance, clientPort);
        }
        catch
        {
            UnityEngine.Debug.LogError("Error in " + DLL_NAME + " in " + new StackFrame().GetMethod().Name);
        }
        return result;
    }

    [DllImport(DLL_NAME, EntryPoint = "StartClient")]
    private static extern RakNetEnums.StartupResult _StartClient(IntPtr instance, Int32 clientPort);

    public static RakNetEnums.StartupResult StartClient(IntPtr instance, Int32 clientPort)
    {
        RakNetEnums.StartupResult result = RakNetEnums.StartupResult.STARTUP_OTHER_FAILURE;
        try
        {
            result = _StartClient(instance, clientPort);
        }
        catch
        {
            UnityEngine.Debug.LogError("Error in " + DLL_NAME + " in " + new StackFrame().GetMethod().Name);
        }
        return result;
    }

    [DllImport(DLL_NAME, EntryPoint = "PingAdress")]
    private static extern void _PingAdress(IntPtr instance, string adress, Int32 serverPort);

    public static void PingAdress(IntPtr instance, string adress, Int32 serverPort)
    {
        try
        {
            _PingAdress(instance, adress, serverPort);
        }
        catch
        {
            UnityEngine.Debug.LogError("Error in " + DLL_NAME + " in " + new StackFrame().GetMethod().Name);
        }
    }

    [DllImport(DLL_NAME, EntryPoint = "SetDataReceiver")]
    private static extern void _SetDataReceiver([MarshalAs(UnmanagedType.FunctionPtr)] DataReceived dataReceived);

    public static void SetDataReceiver([MarshalAs(UnmanagedType.FunctionPtr)] DataReceived dataReceived)
    {
        try
        {
            _SetDataReceiver(dataReceived);
        }
        catch
        {
            UnityEngine.Debug.LogError("Error in " + DLL_NAME + " in " + new StackFrame().GetMethod().Name);
        }
    }

    [DllImport(DLL_NAME, EntryPoint = "SetLogCallback")]
    private static extern void _SetLogCallback(UnityLog unityLog);

    public static void SetLogCallback(UnityLog unityLog)
    {
        try
        {
            _SetLogCallback(LogMethod);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Error in " + DLL_NAME + " in " + new StackFrame().GetMethod().Name + " " + e.Message + " " + e.StackTrace);
        }
    }

    [DllImport(DLL_NAME, EntryPoint = "SetForwarderCallback")]
    private static extern void _SetForwarderCallback(ForwarderReady callback);

    public static void SetForwarderCallback()
    {
        try
        {
            _SetForwarderCallback(ForwarderReadyMethod);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Error in " + DLL_NAME + " in " + new StackFrame().GetMethod().Name + " " + e.Message + " " + e.StackTrace);
        }
    }

    [MonoPInvokeCallback(typeof(UnityLog))]
    private static void LogMethod(StringBuilder buffer, char priority)
    {
        UnityEngine.Debug.Log(buffer);
    }

    [MonoPInvokeCallback(typeof(ForwarderReady))]
    private static void ForwarderReadyMethod(StringBuilder buffer, UInt16 port)
    {
        OnForwarderReady(buffer.ToString(), port);
    }

    [DllImport(DLL_NAME, EntryPoint = "StartLocalServer")]
    private static extern RakNetEnums.StartupResult _StartLocalServer(IntPtr instance, Int32 port, byte maxConnections);

    public static RakNetEnums.StartupResult StartLocalServer(IntPtr instance, Int32 port, byte maxConnections)
    {
        RakNetEnums.StartupResult result = RakNetEnums.StartupResult.STARTUP_OTHER_FAILURE;
        try
        {
            result = _StartLocalServer(instance, port, maxConnections);
        }
        catch
        {
            UnityEngine.Debug.LogError("Error in " + DLL_NAME + " in " + new StackFrame().GetMethod().Name);
        }
        return result;
    }
#if NETWORK_PROJECT
    [DllImport(DLL_NAME, EntryPoint = "SendData", CallingConvention = CallingConvention.StdCall)]
    private static unsafe extern void _SendData(IntPtr instance, RakNetGUID* adress, byte* data, int length);

    public unsafe static void SendData(IntPtr instance, RakNetGUID adress, byte[] data)
    {
        try
        {
            fixed (byte* outData = &((data != null && data.Length != 0) ? ref data[0] : ref *(byte*)null))
            {
                _SendData(instance, &adress, outData, data.Length);
            }
        }
        catch
        {
            UnityEngine.Debug.LogError("Error in " + DLL_NAME + " in " + new StackFrame().GetMethod().Name);
        }
    }
#endif
    [DllImport(DLL_NAME, EntryPoint = "Connect")]
    private static extern RakNetEnums.ConnectionAttemptResult _Connect(IntPtr instance, string adress, Int32 port);

    public static RakNetEnums.ConnectionAttemptResult Connect(IntPtr instance, string adress, Int32 port)
    {
        RakNetEnums.ConnectionAttemptResult result = RakNetEnums.ConnectionAttemptResult.INVALID_PARAMETER;
        try
        {
            result = _Connect(instance, adress, port);
        }
        catch
        {
            UnityEngine.Debug.LogError("Error in " + DLL_NAME + " in " + new StackFrame().GetMethod().Name);
        }
        return result;
    }

#if NETWORK_PROJECT
    [DllImport(DLL_NAME, EntryPoint = "Disconnect")]
    private unsafe static extern void _Disconnect(IntPtr instance, RakNetGUID* guid);

    public unsafe static void Disconnect(IntPtr instance, RakNetGUID guid)
    {       
        try
        {
            _Disconnect(instance, &guid);
        }
        catch
        {
            UnityEngine.Debug.LogError("Error in " + DLL_NAME + " in " + new StackFrame().GetMethod().Name);
        }
    }

#endif

    [DllImport(DLL_NAME, EntryPoint = "GetPacket")]
    private static extern IntPtr _GetPacket(IntPtr instance);

    public static IntPtr GetPacket(IntPtr instance)
    {
        IntPtr result = IntPtr.Zero;
        try
        {
            result = _GetPacket(instance);
        }
        catch
        {
            UnityEngine.Debug.LogError("Error in " + DLL_NAME + " in " + new StackFrame().GetMethod().Name);
        }

        return result;
    }

    [DllImport(DLL_NAME, EntryPoint = "DealocatePacket")]
    private static extern void _DealocatePacket(IntPtr instance, IntPtr packet);

    public static void DealocatePacket(IntPtr instance, IntPtr packet)
    {
        try
        {
            _DealocatePacket(instance, packet);
        }
        catch
        {
            UnityEngine.Debug.LogError("Error in " + DLL_NAME + " in " + new StackFrame().GetMethod().Name);
        }
    }

#if NETWORK_PROJECT
    [DllImport(DLL_NAME, EntryPoint = "PunchNAT")]
    private unsafe static extern bool _PunchNAT(IntPtr instance, RakNetGUID* guid, string adress);

    public unsafe static bool PunchNAT(IntPtr instance, RakNetGUID guid, string adress)
    {
        bool result = false;
        try
        {
            result = _PunchNAT(instance, &guid, adress);
        }
        catch
        {
            UnityEngine.Debug.LogError("Error in " + DLL_NAME + " in " + new StackFrame().GetMethod().Name);
        }
        return result;
    }

#endif

#if NETWORK_PROJECT
    [DllImport(DLL_NAME, EntryPoint = "RequestForwarding")]
    private unsafe static extern bool _RequestForwarding(IntPtr instance, string adress, RakNetGUID* guid);

    public unsafe static bool RequestForwarding(IntPtr instance, RakNetGUID guid, string adress)
    {
        bool result = false;
        try
        {
            result = _RequestForwarding(instance, adress, &guid);
        }
        catch
        {
            UnityEngine.Debug.LogError("Error in " + DLL_NAME + " in " + new StackFrame().GetMethod().Name);
        }
        return result;
    }

#endif
}
