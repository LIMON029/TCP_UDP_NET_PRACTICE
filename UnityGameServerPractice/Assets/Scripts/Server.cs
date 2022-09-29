using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Server
{
    public static int MaxPlayers { get; private set; }
    public static int Port { get; private set; }
    public static Dictionary<int, Client> clients = new Dictionary<int, Client>();

    public delegate void PacketHandler(int _fromClient, Packet _packet);
    public static Dictionary<int, PacketHandler> packetHandlers;

    private static TcpListener tcpListener;
    private static UdpClient udpListener;

    public static void Start(int _maxPlayers, int _port)
    {
        Debug.Log("Server is starting...");

        MaxPlayers = _maxPlayers;
        Port = _port;

        InitiallizeServerData();

        tcpListener = new TcpListener(IPAddress.Any, Port);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

        udpListener = new UdpClient(Port);
        udpListener.BeginReceive(UDPReceiveCallback, null);

        Debug.Log($"Server is started on {Port}.");
    }

    private static void TCPConnectCallback(IAsyncResult _result)
    {
        // tcpListener -> Accecpt -> Get Client
        TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

        Debug.Log($"Incoming connection from {_client.Client.RemoteEndPoint}...");

        // Client
        for (int i = 1; i <= MaxPlayers; i++)
        {
            if (clients[i].tcp.socket == null)
            {
                clients[i].tcp.Connect(_client);
                // 하나만 연결하도록
                return;
            }
        }

        Debug.Log($"{_client.Client.RemoteEndPoint} failed to connect: Server full!");
    }

    private static void UDPReceiveCallback(IAsyncResult _result)
    {
        try
        {
            IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            if (_data.Length < 4)
            {
                return;
            }

            using (Packet _packet = new Packet(_data))
            {
                int _clientId = _packet.ReadInt();

                if (_clientId == 0)
                {
                    return;
                }

                if (clients[_clientId].udp.endPoint == null)
                {
                    clients[_clientId].udp.Connect(_clientEndPoint);
                    return;
                }

                if (clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
                {
                    clients[_clientId].udp.HandleData(_packet);
                }
            }
        }
        catch (Exception _e)
        {
            Debug.Log($"Error receiving UDP data: {_e}");
        }
    }

    public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
    {
        try
        {
            if (_clientEndPoint != null)
            {
                udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
            }
        }
        catch (Exception _e)
        {
            Debug.Log($"Error sending data to {_clientEndPoint} via UDP: {_e}");
        }
    }

    private static void InitiallizeServerData()
    {
        for (int i = 1; i <= MaxPlayers; i++)
        {
            clients.Add(i, new Client(i));
        }

        packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
                { (int)ClientPackets.playerMovement, ServerHandle.PlayerMovement }
            };
        Debug.Log("Initialized packets.");
    }
}
