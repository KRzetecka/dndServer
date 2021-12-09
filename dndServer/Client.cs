using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using Polenter.Serialization;
using System.IO;
using System.Xml.Serialization;

namespace Server
{
    class Client
    {
        public int playerID;
        public bool isPlaying = false;
        public bool isLogged = false;
        public string name {get; set;}
        public string currentRoom;

        //public string password { get; set; }
        public TcpClient socket;
        public NetworkStream stream;

        public ByteBuffer buffer;
        //public Player player;
        private byte[] receiveBuffer;

        public void StartClient()
        {
            socket.ReceiveBufferSize = 8192;
            socket.SendBufferSize = 8192;

            stream = socket.GetStream();
            receiveBuffer = new byte[socket.ReceiveBufferSize];
            stream.BeginRead(receiveBuffer, 0, socket.ReceiveBufferSize, ReceivedData, null);
            name = "Guest";
            currentRoom = null;
            ServerSend.Welcome(playerID, "Connected");
        }
        private void ReceivedData(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    CloseConnection();
                    return;
                }
                byte[] _tempBuffer = new byte[_byteLength];
                Array.Copy(receiveBuffer, _tempBuffer, _byteLength);
                ServerHandle.HandleData(playerID, _tempBuffer); 
                stream.BeginRead(receiveBuffer, 0, socket.ReceiveBufferSize, ReceivedData, null);
            }
            catch (Exception _ex)
            {
                Logger.Log(LogType.error, "Error while receiving data: " + _ex);
                CloseConnection();
                return;
            }
        }
        private void CloseConnection()
        {
            Logger.Log(LogType.info1, "Connection from " + socket.Client.RemoteEndPoint.ToString() + " has been terminated");
            LogOutAccount();
            socket.Close();
            socket = null;
        }

        public void NewAccount(string _name, string _password, int _playerID)
        {
            if (Directory.Exists(Globals.root + @"players\" + _name))
            {
                var rand = new Random();
                int i = rand.Next(4);
                switch (i)
                {
                    case 0:
                        ServerSend.RegisterError(_playerID, "Name taken unfortunately. But it is a nice day huh?");
                        break;
                    case 1:
                        ServerSend.RegisterError(_playerID, "Name already taken :c");
                        break;
                    case 2:
                        ServerSend.RegisterError(_playerID, "Nah that will not work. Username already taken.");
                        break;
                    case 3:
                        ServerSend.RegisterError(_playerID, "I have a bad news for you. Username already taken.");
                        break;
                    case 4:
                        ServerSend.RegisterError(_playerID, "Username already taken, oh no.");
                        break;
                }
                return;
            }
            var acc = new Account(_name, _password);
            Directory.CreateDirectory(Globals.root + @"players\" + _name);
            XmlTool.SaveToFile<Account>(acc, Globals.root + @"players\" + _name + @"\" + _name + ".sav");      
            Logger.Log(LogType.info2, "New account created: " + _name);
            ServerSend.RegisterError(_playerID, "Registered");
        }
        public void LogToAccount(string _name, string _password, int _playerID)
        {
            foreach (Client client in Globals.clients.Values)
            {
                if (client.name == _name)
                {
                    ServerSend.LoginMessage(_playerID, "Someone is currently using this account.", "");
                    return;
                }
            }
            if (Directory.Exists(Globals.root + @"players\" + _name))
            {
                Account tmp;
                XmlTool.LoadFrom(Globals.root + @"players\" + _name + @"\" + _name + ".sav", out tmp);
                string password = tmp.getPassword();
                if(password == _password)
                {
                    Globals.clients[_playerID].isLogged = true;
                    Globals.clients[_playerID].name = _name;
                    Logger.Log(LogType.info2, "Connection from " + Globals.clients[_playerID].socket.Client.RemoteEndPoint + " logged as: " + _name);
                    ServerSend.LoginMessage(_playerID, "Logged", _name);
                }
                else
                {
                    ServerSend.LoginMessage(_playerID, "Wrong password", "");
                }
            }
            else
            {
                ServerSend.LoginMessage(_playerID, "Account with this name does not exist", "");
            }
        }
        public void LogOutAccount()
        {
            
            if (currentRoom != null)
            {
                Globals.GameRooms.Find(e => e.Name == currentRoom).LogOffPlayer(name);
                currentRoom = null;
            }
            isPlaying = false;
            isLogged = false;
            name = "Guest";
        }


    }

    public class Account
    {
        public string Name;
        public string Password;
        public Account()
        {

        }
        public Account(string _name, string _password)
        {
            Name = _name;
            Password = _password;
        }
        public string getPassword() { return Password; }
    }
}
