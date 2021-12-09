using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;


namespace Server
{
    class ServerTCP
    {
        private static TcpListener socket;
        private static int port = 9423;
        public static void InitNetwork()
        {
            Logger.Log(LogType.info1, "Starting server on port " + port + "..");
            ServerHandle.InitPackets();
            socket = new TcpListener(IPAddress.Any, port);
            socket.Start();
            socket.BeginAcceptTcpClient(new AsyncCallback(ClientConnected), null);

        }
        private static void ClientConnected(IAsyncResult _result)
        {
            TcpClient _client = socket.EndAcceptTcpClient(_result);
            _client.NoDelay = false;
            socket.BeginAcceptTcpClient(new AsyncCallback(ClientConnected), null);
            Logger.Log(LogType.info1, "Incoming connection from " + _client.Client.RemoteEndPoint.ToString());



            for (int i = 1; i <= Constants.MAX_PLAYERS; i++)
            {
                if (Globals.clients[i].socket == null)
                {
                    Globals.clients[i].socket = _client;
                    Globals.clients[i].playerID = i;
                    Globals.clients[i].StartClient();
                    return;
                }
            }
            Logger.Log(LogType.warning, "Server is full");


        }

    }
}
