﻿using System;
using System.Text;
using System.Collections;
using System.Net;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;
using RakNetEnums;
using static RakNetDLL;
using System.IO;

public class RoomListData
{
    public UInt32 RoomID;
    public bool Passwd;
    public string Name;
    public uint Slots;
    public byte AdditionalFlags;
}

public class NetworkManager
{
    public event Action<RakNetGUID> ConnectionEstablished = delegate { };
    public event Action<IntPtr> UserAccepted = delegate { };
    public event Action<List<RoomListData>> RoomsListReceived = delegate { };
    public event Action<List<string>,List<bool>> RoomMembersReceived = delegate { };
    public event Action StartHost = delegate { };
    public event Action IncorrectPassword = delegate { };
    public event Action<string, bool> PlayerLeftRoom = delegate { };
    public event Action<UInt64> PlayerChangedStatus = delegate { };
    public event Action RoomFull = delegate { };
    public event Action<byte> OnRoomJoined = delegate { };
    public event Action<ERoomsErrorCodes> ErrorRoomConnection = delegate { };
    public event Action<RakNetGUID> ConnectionLost = delegate { };
    public event Action<RakNetGUID> DisconnectNotification = delegate { };
    public event Action UserRegistered = delegate{ };
    public event Action<IntPtr, UInt64> InLobbyMessage = delegate{ };

    private static NetworkManager instance;
    public static NetworkManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new NetworkManager();
            }
            return instance;
        }
    }

    public UInt32 ConnectedRoomID { get; private set; } = UInt32.MaxValue;
    public int AwaitedPlayers { get; private set; } = int.MaxValue;
    public RakNetGUID ServerGUID => serverGUID;

    public bool IsConnectedToServer { get; set; }
    
    private MonoBehaviour coroutineSource;

    private bool startHost = false;

    private IntPtr rakNetInstance = IntPtr.Zero;
    //private RakNetGUID connectedGuid = new RakNetGUID();

    private Coroutine receiveData = null;

    //private string NATPunchAdress = "";
    
    private IGameNetworkManager gameNetworkManager = null;

    private string userName = null;

    private RakNetGUID serverGUID;

    private RakNetGUID forwardGUID;
    
    
    private RakNetGUID forwarderGUID;
    private RakNetGUID natGUID;

    public NetworkManager()
    {
#if NETWORK_PROJECT
        if (GameManager.IsApplicationQuitting)
            return;
#endif

        coroutineSource = GameObject.Instantiate(new GameObject("NetworkManagerHelper")).AddComponent<NetworkManagerHelper>();
        GameObject.DontDestroyOnLoad(coroutineSource);

        rakNetInstance = GetCurrentInstance();
        //SetDataReceiver(DataReceived);
        SetLogCallback((StringBuilder value, char piority) => { /*value.Length = (int)length;*/ Debug.Log(value.ToString()); });
        SetForwarderCallback();
        OnForwarderReady += (adress, port) =>
        {
            if (startHost)
            {
                Debug.Log(adress + " " + port);
                ConnectToServer(adress, port);
            }
        };
    }

    ~NetworkManager()
    {
        DestroyInstance(rakNetInstance);
    }

    public void SetGameNetworkManager(IGameNetworkManager manager)
    {
        gameNetworkManager = manager;
    }

    public void StartLocalHost(byte maxConnections)
    {
#if NETWORK_PROJECT
        Debug.Log(StartLocalServer(rakNetInstance, Consts.ServerPort, maxConnections));
        receiveData = coroutineSource.StartCoroutine(ReceiveData());
#endif
    }

    public void StartLocalClient(int port)
    {
        Debug.Log(RakNetDLL.StartLocalClient(rakNetInstance, port));
        receiveData = coroutineSource.StartCoroutine(ReceiveData());
    }

    public void StartClient(int port)
    {
        Debug.Log(RakNetDLL.StartClient(rakNetInstance, port));
        receiveData = coroutineSource.StartCoroutine(ReceiveData());
    }

    public void RefreshLocalConnection()
    {
#if NETWORK_PROJECT
        PingAdress(rakNetInstance, "255.255.255.255", Consts.ServerPort);
#endif
    }

    public void ConnectToGameServer(string address)
    {
#if NETWORK_PROJECT
        Connect(rakNetInstance, address, Consts.ServerPort);
#endif
    }

    public void ConnectToServer(string address, int port)
    {
        Debug.Log(Connect(rakNetInstance, address, port));
    }

    public void Disconnect(RakNetGUID guid)
    {
#if NETWORK_PROJECT
        RakNetDLL.Disconnect(rakNetInstance, guid);
#endif
    }
    public void SendData(RakNetGUID guid, byte[] array)
    {
        //TODO find better way
        Assert.IsTrue(array.Length > 0, "Cannot send empty data");
        byte[] newArray = new byte[array.Length + 1];
        Array.Copy(array, 0, newArray, 1, array.Length);
        newArray[0] = (byte)RakNetEnums.MasterConnectionMessage.GAME_CUSTOM_MSG;
#if NETWORK_PROJECT
        RakNetDLL.SendData(rakNetInstance, guid, newArray);
#endif
    }
    
#region ConnectingToGameServer
    public void StartOnlineConnection()
    {
        if (IsConnectedToServer)
            return;
#if NETWORK_PROJECT
        StartClient(GameNetworkManager.ClientPort + UnityEngine.Random.Range(0, 100));
        ConnectToServer(Consts.ServerAdress, 8100);
        UserAccepted += OnConnectedToCoordinator;
#endif
    }

    private void OnConnectedToCoordinator(IntPtr data)
    {
#if NETWORK_PROJECT
        Debug.Log("Connencted to coordinator");
        UserAccepted -= OnConnectedToCoordinator;
        forwarderGUID.g = (UInt64)IPAddress.NetworkToHostOrder(Marshal.ReadInt64(data, 1));
        Debug.Log(forwarderGUID.g);
        ConnectToServer(Consts.ServerAdress, 8105);
        UserAccepted += OnConnectedToNATPunch;
#endif
    }

    private void OnConnectedToNATPunch(IntPtr data)
    {
#if NETWORK_PROJECT
        Debug.Log("Connencted to NAT punch");
        UserAccepted -= OnConnectedToNATPunch;
        natGUID.g = (UInt64)IPAddress.NetworkToHostOrder(Marshal.ReadInt64(data, 1));
        Debug.Log(natGUID.g);

        ConnectToServer(Consts.ServerAdress, Consts.ServerPort);
        ConnectionEstablished += OnConnectedToGameServer;
#endif
    }

    private void OnConnectedToGameServer(RakNetGUID guid)
    {
        serverGUID = guid;
        ConnectionEstablished -= OnConnectedToGameServer;
        LogUser();
    }

    private void LogUser()
    {    
        var memStream = new MemoryStream();

        memStream.WriteByte((byte)RakNetEnums.MasterConnectionMessage.LOG_ME_IN);    
        memStream.Write(BitConverter.GetBytes(natGUID.g), 0, sizeof(ulong));
        memStream.Write(BitConverter.GetBytes(forwarderGUID.g), 0, sizeof(ulong));
#if NETWORK_PROJECT
        SetGameNetworkManager(GameNetworkManager.Instance);
#endif
        gameNetworkManager?.GetUserData(memStream);
        byte[] array = memStream.ToArray();
        SendDataToServer(serverGUID, array);

        IsConnectedToServer = true;
        UserAccepted += OnUserRegistered;
    }

    private void OnUserRegistered(IntPtr ptr)
    {
        UserAccepted -= OnUserRegistered;
        UserRegistered();
    }
#endregion

#region RequestsToServer
    public void GetRooms()
    {
        byte[] array = new byte[1];
        array[0] = (byte)RakNetEnums.MasterConnectionMessage.GET_ROOMS_LIST;
        SendDataToServer(serverGUID, array);
    }

    public void CreateRoom(byte playersSlots, byte additionalParams, string roomName, string roomPasswd)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            stream.WriteByte((byte)RakNetEnums.MasterConnectionMessage.GIB_ME_ROOM);

            stream.WriteByte(playersSlots);
            stream.WriteByte(additionalParams);

            stream.Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)roomName.Length)), 0, 2);
            stream.Write(Encoding.GetEncoding("UTF-8").GetBytes(roomName), 0, roomName.Length);

            stream.Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)roomPasswd.Length)), 0, 2);

            stream.Write(Encoding.GetEncoding("UTF-8").GetBytes(roomPasswd), 0, roomPasswd.Length);
            //Debug.Log(stream.ToArray().Length);
            SendDataToServer(serverGUID, stream.ToArray());
        }
    }

    public void ConnectToRoom(uint ID, string password = "")
    {
        /*using (MemoryStream memStream = new MemoryStream())
        {
            memStream.WriteByte((byte)RakNetEnums.MasterConnectionMessage.JOIN_ROOM);
            memStream.Write(BitConverter.GetBytes((uint)IPAddress.HostToNetworkOrder(ID)), 0, sizeof(uint));
            memStream.Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)password.Length)), 0, 2);
            memStream.Write(Encoding.GetEncoding("UTF-8").GetBytes(password), 0, password.Length);
            byte[] newArray = memStream.ToArray();
            SendDataToServer(serverGUID, newArray);
        }*/
        var memStream = new MemoryStream();
        memStream.WriteByte((byte)RakNetEnums.MasterConnectionMessage.JOIN_ROOM);
        byte[] a = BitConverter.GetBytes(ID);
        Array.Reverse(a);
        memStream.Write(a, 0, sizeof(uint));
        memStream.Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)password.Length)), 0, 2);
        memStream.Write(Encoding.GetEncoding("UTF-8").GetBytes(password), 0, password.Length);
        byte[] newArray = memStream.ToArray();
        SendDataToServer(ServerGUID, newArray);
    }

    public void LeaveRoom(uint ID)
    {
        var memStream = new MemoryStream();
        memStream.WriteByte((byte)RakNetEnums.MasterConnectionMessage.LEAVE_ROOM);
        byte[] a = BitConverter.GetBytes(ID);
        Array.Reverse(a);
        memStream.Write(a, 0, sizeof(uint));
        byte[] newArray = memStream.ToArray();
        SendDataToServer(serverGUID, newArray);
    }

    public void SetReadyStatus(string userName, bool ready)
    {
        var memStream = new MemoryStream();
        memStream.WriteByte((byte)RakNetEnums.MasterConnectionMessage.SET_READY_STATUS);
        byte[] a = BitConverter.GetBytes(ready?1:0);
        Array.Reverse(a);
        memStream.Write(a, 0, sizeof(uint));
        memStream.Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)userName.Length)), 0, 2);
        memStream.Write(Encoding.GetEncoding("UTF-8").GetBytes(userName), 0, userName.Length);
        byte[] newArray = memStream.ToArray();
        Instance.SendDataToServer(serverGUID, newArray);
    }

    public void KickPlayer(string userName)
    {
        var memStream = new MemoryStream();
        memStream.WriteByte((byte)RakNetEnums.MasterConnectionMessage.KICK_USER);
        byte[] a = BitConverter.GetBytes(ConnectedRoomID);
        Array.Reverse(a);
        memStream.Write(a, 0, sizeof(uint));
        memStream.Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)userName.Length)), 0, 2);
        memStream.Write(Encoding.GetEncoding("UTF-8").GetBytes(userName), 0, userName.Length);
        byte[] newArray = memStream.ToArray();
        Instance.SendDataToServer(serverGUID, newArray);
    }

    public void SendStartGame()
    {
        byte[] array = new byte[5];
        array[0] = (byte)RakNetEnums.MasterConnectionMessage.FORCE_START_ROOM_GAME;
        byte[] a = BitConverter.GetBytes(ConnectedRoomID);
        Array.Reverse(a);
        array[1] = a[0];
        array[2] = a[1];
        array[3] = a[2];
        array[4] = a[3];
        SendDataToServer(ServerGUID, array);
    }
#endregion

    public void SendDataToServer(RakNetGUID guid, byte[] array)
    {
        Assert.IsTrue(array.Length > 0, "Cannot send empty data");
#if NETWORK_PROJECT
        RakNetDLL.SendData(rakNetInstance, guid, array);
#endif
    }

    private IEnumerator ReceiveData()
    {
        int offset = 0;
        while(true)
        {
            IntPtr ptr = GetPacket(rakNetInstance);
            if(ptr != null && ptr != IntPtr.Zero)
            {
                //TODO Dealocate?
                RakNetPacket packet = Marshal.PtrToStructure<RakNetPacket>(ptr);
                Debug.Log(packet.port);
                byte id = Marshal.ReadByte(packet.data);
                if (id < (byte)RakNetEnums.DefaultMessageIDTypes.ID_USER_PACKET_ENUM)
                {
                    Debug.Log((RakNetEnums.DefaultMessageIDTypes)id);
                    switch ((RakNetEnums.DefaultMessageIDTypes)id)
                    {
                        case RakNetEnums.DefaultMessageIDTypes.ID_UNCONNECTED_PONG:
                            {
                                gameNetworkManager?.PongReceived(packet.adress);
                            }
                            break;
                        case RakNetEnums.DefaultMessageIDTypes.ID_NEW_INCOMING_CONNECTION:
                            {
                                gameNetworkManager?.ConnectionIncoming(packet.guid);
                            }
                            break;
                        case RakNetEnums.DefaultMessageIDTypes.ID_ALREADY_CONNECTED:
                        case RakNetEnums.DefaultMessageIDTypes.ID_CONNECTION_REQUEST_ACCEPTED:
                            {
                                gameNetworkManager?.ConnectionEstablished(packet.guid);
                                ConnectionEstablished(Marshal.PtrToStructure<RakNetGUID>(packet.guid));
                            }
                            break;
                        case DefaultMessageIDTypes.ID_DISCONNECTION_NOTIFICATION:
                            DisconnectNotification(Marshal.PtrToStructure<RakNetGUID>(packet.guid));
                            break;
                        case RakNetEnums.DefaultMessageIDTypes.ID_USER_PACKET_ENUM:
                            {
                                gameNetworkManager?.ParseIncommingData(packet.data, packet.length);
                            }
                            break;
                        case RakNetEnums.DefaultMessageIDTypes.ID_NAT_PUNCHTHROUGH_SUCCEEDED:
                            {
                                if (startHost)
                                {
                                    ConnectToServer(packet.adress, (int)packet.port);
                                }                              
                            }
                            break;
                        case RakNetEnums.DefaultMessageIDTypes.ID_NAT_PUNCHTHROUGH_FAILED:
                            {
                                if (startHost)
                                {
#if NETWORK_PROJECT
                                    RequestForwarding(rakNetInstance, forwardGUID, Consts.ForwarderServerAdress);
#endif
                                }
                            }
                            break;
                        case RakNetEnums.DefaultMessageIDTypes.ID_CONNECTION_LOST:
                            {
                                ConnectionLost(Marshal.PtrToStructure<RakNetGUID>(packet.guid));
                            }
                            break;
                    }
                }
                else
                {
                    Debug.Log((RakNetEnums.MasterConnectionMessage)id);
                    switch ((RakNetEnums.MasterConnectionMessage)id)
                    {
                        case RakNetEnums.MasterConnectionMessage.LOG_ME_IN:
                            //ShowPacket(packet.data, packet.length);

                            break;
                        case RakNetEnums.MasterConnectionMessage.USER_REGISTERED:
                            {
                                //ShowPacket(packet.data, packet.length);
                                if (gameNetworkManager != null)
                                {
                                    offset = 1;
                                    var nLength = IPAddress.NetworkToHostOrder(Marshal.ReadInt16(packet.data, offset));
                                    offset += 2;
                                    var nam = "";
                                    for (int n = 0; n < nLength; n++)
                                    {
                                        nam += (char)Marshal.ReadByte(packet.data, offset++);
                                    }

                                    
                                    userName = nam;
                                    gameNetworkManager?.SetUserName(userName);
                                    serverGUID = Marshal.PtrToStructure<RakNetGUID>(packet.guid);
                                }
                                //ShowPacket(packet.data, packet.length);
                                UserAccepted(packet.data);
                            }
                            break;
                        case RakNetEnums.MasterConnectionMessage.JOIN_ANY:
                            Debug.Log(Marshal.ReadByte(packet.data, 1));
                            Debug.Log(Marshal.ReadByte(packet.data, 2));
                            Debug.Log(Marshal.ReadByte(packet.data, 3));
                            Debug.Log(Marshal.ReadByte(packet.data, 4));
                            Debug.Log(Marshal.ReadByte(packet.data, 5));

                            break;
                        case RakNetEnums.MasterConnectionMessage.GET_ROOMS_LIST:
                            //ShowPacket(packet.data, packet.length);
                            offset = 1;
                            uint counter = (uint)IPAddress.NetworkToHostOrder(Marshal.ReadInt16(packet.data, offset));
                            offset += 2;
                            List<RoomListData> list = new List<RoomListData>((int)counter);
                            for (int i = 0; i < counter; i++)
                            {
                                RoomListData data = new RoomListData();
                                Debug.Log(Marshal.ReadInt32(packet.data, offset));
                                data.RoomID = (uint)IPAddress.NetworkToHostOrder(Marshal.ReadInt32(packet.data, offset));
                                offset += 4;
                                data.Passwd = Marshal.ReadByte(packet.data, offset++) != 0;                          
                                var nameLength = IPAddress.NetworkToHostOrder(Marshal.ReadInt16(packet.data, offset));
                                offset += 2;
                                var name = "";
                                for (int j = 0; j < nameLength; j++)
                                {
                                    name += (char)Marshal.ReadByte(packet.data, offset++);
                                }
                                data.Name = name;
                                data.Slots = (uint)IPAddress.NetworkToHostOrder(Marshal.ReadInt32(packet.data, offset));
                                offset += 4;
                                data.AdditionalFlags = Marshal.ReadByte(packet.data, offset++);
                                list.Add(data);
                            }
                            RoomsListReceived(list);
                            break;
                        case RakNetEnums.MasterConnectionMessage.YOU_HOST:
                            //ShowPacket(packet.data, packet.length);
                            Int32 numberOfPlayers = IPAddress.NetworkToHostOrder(Marshal.ReadInt32(packet.data, 1));
                            Debug.Log(numberOfPlayers);
                            AwaitedPlayers = numberOfPlayers;
                            StartHost();
                            startHost = false;
                            break;
                        case RakNetEnums.MasterConnectionMessage.YOU_DONT_HOST:

                            UInt64 guid = (UInt64)(Marshal.ReadInt64(packet.data, 1));
                            RakNetGUID natPunchGUID = new RakNetGUID();
                            natPunchGUID.g = (guid);
                            forwardGUID = new RakNetGUID();
                            forwardGUID.g = (UInt64)(Marshal.ReadInt64(packet.data, 9));
#if NETWORK_PROJECT
                            Debug.Log(PunchNAT(rakNetInstance, natPunchGUID, Consts.NatServerAdress));
                            //Debug.Log(RequestForwarding(rakNetInstance, forwardGUID, Consts.ForwarderServerAdress));
#endif
                            //Debug.Log(RequestForwarding(rakNetInstance, remoteGUID, "95.50.217.69:8100"));
                            startHost = true;

                            break;
                        case RakNetEnums.MasterConnectionMessage.GAME_CUSTOM_MSG:
                            gameNetworkManager?.ParseIncommingData(packet.data, packet.length);
                            break;
                        case RakNetEnums.MasterConnectionMessage.GIB_ME_ROOM:
                            {
                                //ShowPacket(packet.data, packet.length);
                                byte errorCode = Marshal.ReadByte(packet.data, 1);
                                if (errorCode == 0)
                                {
                                    ConnectedRoomID = (uint)IPAddress.NetworkToHostOrder(Marshal.ReadInt32(packet.data, 2));
                                    OnRoomJoined(Marshal.ReadByte(packet.data, 6));
                                    RoomMembersReceived(new List<string>() { userName }, new List<bool>() { false });
                                }
                                else
                                {
                                    Debug.LogWarning("Error in room creation");
                                    ErrorRoomConnection((ERoomsErrorCodes)errorCode);
                                }
                            }
                            break;
                        case MasterConnectionMessage.JOIN_ROOM:
                            {
                                //ShowPacket(packet.data, packet.length);
                                byte errorCode = Marshal.ReadByte(packet.data, 1);
                                if (errorCode == 0)
                                {

                                    OnRoomJoined(Marshal.ReadByte(packet.data, 2));
                                    byte size = Marshal.ReadByte(packet.data, 3);
                                    List<string> names = new List<string>(size);
                                    List<bool> readys = new List<bool>(size);
                                    offset = 4;
                                    for (int i = 0; i < size; i++)
                                    {
                                        string s = "";
                                        short length = IPAddress.NetworkToHostOrder(Marshal.ReadInt16(packet.data, offset));
                                        offset += 2;
                                        for (int j = 0; j < length; j++)
                                        {
                                            s += (char)Marshal.ReadByte(packet.data, offset++);
                                        }
                                        names.Add(s);
                                        offset += 3;
                                        var ready = Marshal.ReadByte(packet.data, offset++);
                                        readys.Add(ready == 1);
                                    }
                                    ConnectedRoomID = (uint)IPAddress.NetworkToHostOrder(Marshal.ReadInt32(packet.data, offset));
                                    RoomMembersReceived(names, readys);
                                }
                                else
                                {
                                    Debug.LogWarning("Error in room joining");
                                }
                            }
                            break;
                        case RakNetEnums.MasterConnectionMessage.UPDATE_ROOM_USERS:
                            {
                                //ShowPacket(packet.data, packet.length);
                                offset = 1;
                                byte size = Marshal.ReadByte(packet.data, offset++);
                                List<string> names = new List<string>(size);
                                List<bool> readys = new List<bool>(size);
                                for (int i = 0; i < size; i++)
                                {
                                    string s = "";
                                    short length = IPAddress.NetworkToHostOrder(Marshal.ReadInt16(packet.data, offset));
                                    offset += 2;
                                    for (int j = 0; j < length; j++)
                                    {
                                        s += (char)Marshal.ReadByte(packet.data, offset++);
                                    }
                                    names.Add(s);
                                    var ready = Marshal.ReadByte(packet.data, offset++);
                                    readys.Add(ready == 1);
                                }
                                RoomMembersReceived(names, readys);
                            }
                            break;
                        case MasterConnectionMessage.LEAVE_ROOM:
                            {
                                var nameLength = Marshal.ReadByte(packet.data, 10);
                                var name = "";
                                var i = 11;
                                for (; i < nameLength + 11; i++)
                                {
                                    name += (char)Marshal.ReadByte(packet.data, i);
                                }
                                var kicked = Marshal.ReadByte(packet.data, i)!= 0;
                                Debug.Log(kicked);
                                PlayerLeftRoom(name, kicked);
                                break;
                            }
                        case MasterConnectionMessage.SET_READY_STATUS:
                            {
                                //ShowPacket(packet.data, packet.length);
                                var ready = (UInt64)IPAddress.NetworkToHostOrder(Marshal.ReadInt64(packet.data, 1));                               
                                PlayerChangedStatus(ready);
                                break;
                            }
                        case MasterConnectionMessage.SET_TEAMS:
                        {
                            InLobbyMessage(packet.data, packet.length);
                            break;
                            }
                        case MasterConnectionMessage.ROOM_FULL:
                            {
                                RoomFull();
                                break;
                            }
                    }
                }
                DealocatePacket(rakNetInstance, packet.packetRef);
            }
            yield return null;
        }
    }

    private void ShowPacket(IntPtr data, ulong size)
    {
        for (ulong i = 0; i < size; i++)
        {
            Debug.Log(Marshal.ReadByte(data, (int)i));
        }
    }
}
