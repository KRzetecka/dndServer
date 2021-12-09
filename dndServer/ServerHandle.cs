using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


namespace Server
{
    class ServerHandle
    {
        public delegate void Packet(int _playerID, byte[] _data);
        public static Dictionary<int, Packet> packets;

        public static void InitPackets()
        {
            Logger.Log(LogType.info1, "Initializing packets..");
            packets = new Dictionary<int, Packet>()
            {
                { (int)ClientPackets.welcomeReceived, WelcomeReceived },
                { (int)ClientPackets.getRoomList, GetRoomList },
                { (int)ClientPackets.getRoomDesc, GetRoomDesc },
                { (int)ClientPackets.isPasswordCorrect, RoomPasswordCheck },
                { (int)ClientPackets.userRegister, UserRegister },
                { (int)ClientPackets.userLogin, UserLogin},
                { (int)ClientPackets.leaveRoom, LeaveRoom },
                { (int)ClientPackets.userLogout, UserLogout },
                { (int)ClientPackets.refreshRoomData, RefreshRoomData },
                { (int)ClientPackets.newCharacter, NewCharacter },
                { (int)ClientPackets.changeSettings, ChangeSettings },
                { (int)ClientPackets.editRace, EditRace },
            };
        }

        public static void HandleData(int _playerID, byte[] _data)
        {
            byte[] _tempBuffer = (byte[])_data.Clone();
            int _packetLength = 0;

            if (Globals.clients[_playerID].buffer == null)
            {
                Globals.clients[_playerID].buffer = new ByteBuffer();
            }

            Globals.clients[_playerID].buffer.WriteBytes(_tempBuffer);

            if (Globals.clients[_playerID].buffer.Count() == 0)
            {
                Globals.clients[_playerID].buffer.Clear();
                return;
            }

            if (Globals.clients[_playerID].buffer.Length() >= 4)
            {
                _packetLength = Globals.clients[_playerID].buffer.ReadInt(false);
                if (_packetLength <= 0)
                {
                    Globals.clients[_playerID].buffer.Clear();
                    return;
                }
            }

            while (_packetLength > 0 && _packetLength <= Globals.clients[_playerID].buffer.Length() - 4)
            {
                Globals.clients[_playerID].buffer.ReadInt();
                _data = Globals.clients[_playerID].buffer.ReadBytes(_packetLength);
                HandlePackets(_playerID, _data);

                _packetLength = 0;
                if (Globals.clients[_playerID].buffer.Length() >= 4)
                {
                    _packetLength = Globals.clients[_playerID].buffer.ReadInt(false);
                    if (_packetLength <= 0)
                    {
                        Globals.clients[_playerID].buffer.Clear();
                        return;
                    }
                }
            }
            if (_packetLength <= 1)
            {
                Globals.clients[_playerID].buffer.Clear();
            }


        }
        private static void HandlePackets(int _PlayerID, byte[] _data)
        {
            ByteBuffer _buffer = new ByteBuffer();
            _buffer.WriteBytes(_data);
            int _packetID = _buffer.ReadInt();
            _buffer.Dispose();

            if(packets.TryGetValue(_packetID, out Packet _packet))
            {
                _packet.Invoke(_PlayerID, _data);
            }
        }

        private static void WelcomeReceived(int _playerID, byte[] _data)
        {
            ByteBuffer _buffer = new ByteBuffer();
            _buffer.WriteBytes(_data);
            _buffer.ReadInt();
            string _username = _buffer.ReadString();
            _buffer.Dispose();
            Logger.Log(LogType.info2, "Connection from " + Globals.clients[_playerID].socket.Client.RemoteEndPoint + " was successful. Username: " + _username);
        }

        private static void GetRoomList(int _playerID, byte[] _data)
        {
            string msg;
            List<string> RoomNames = new List<string>();
            foreach (string name in Directory.EnumerateDirectories(Globals.root + @"rooms", "*", SearchOption.AllDirectories).Select(f => Path.GetFileName(f)))
            {
                RoomNames.Add(name);
            }
            msg = XmlTool.SerializeObjectToXML(RoomNames);
            ServerSend.RoomList(_playerID, msg);
        }

        private static void GetRoomDesc(int _playerID, byte[] _data)
        {
            string msg;
            ByteBuffer _buffer = new ByteBuffer();
            _buffer.WriteBytes(_data);
            _buffer.ReadInt();
            msg = _buffer.ReadString();
            _buffer.Dispose();

            string desc = RoomsHandle.GetDesc(msg);
            ServerSend.RoomDesc(_playerID, desc);
        }
        
        //Room Password
        private static void RoomPasswordCheck(int _playerID, byte[] _data)
        {
            string passw, room, result;
            ByteBuffer _buffer = new ByteBuffer();
            _buffer.WriteBytes(_data);
            _buffer.ReadInt();
            passw = _buffer.ReadString();
            room = _buffer.ReadString();
            _buffer.Dispose();
            var gameRoom = Globals.GameRooms.Find(e => e.Name == room);

            if (gameRoom.PasswordCheck(passw))
            {
                ServerSend.PasswordCheckResult(_playerID, "Success");
                if (Globals.clients[_playerID].isLogged)
                {
                    if (Globals.clients[_playerID].isPlaying == false)
                    {
                        if (gameRoom.IsPlayerDM(_playerID))
                        {
                            gameRoom.LogInPlayer(Globals.clients[_playerID].name);
                            Globals.clients[_playerID].currentRoom = gameRoom.Name;
                            ServerSend.GameInit(_playerID, gameRoom, true);
                            Logger.Log(LogType.info1, "Player " + Globals.clients[_playerID].name + " entered the -" + gameRoom.Name + "- room as DM");
                            Globals.clients[_playerID].isPlaying = true;
                        }
                        else
                        {
                            gameRoom.LogInPlayer(Globals.clients[_playerID].name);
                            Globals.clients[_playerID].currentRoom = gameRoom.Name;
                            ServerSend.GameInit(_playerID, gameRoom, false);
                            Logger.Log(LogType.info1, "Player " + Globals.clients[_playerID].name + " entered the -" + gameRoom.Name + "- room");
                            Globals.clients[_playerID].isPlaying = true;
                        }
                    }
                    else
                    {
                        string desc = "Looks like u re already in one of the rooms. Please contact admin if you are worried for your account.";
                        ServerSend.RoomDesc(_playerID, desc);
                    }
                }
                else
                {
                    string desc = "You need to log in first :D \n If you don't have account, please, try to register a new one.";
                    ServerSend.RoomDesc(_playerID, desc);
                }
            }
            else
            {
                ServerSend.PasswordCheckResult(_playerID, "Wrong Password");
            }
        }
        private static void UserRegister(int _playerID, byte[] _data)
        {  
            ByteBuffer _buffer = new ByteBuffer();
            _buffer.WriteBytes(_data);
            _buffer.ReadInt();
            string name = _buffer.ReadString();
            string passw = _buffer.ReadString();
            _buffer.Dispose();

            Globals.clients[_playerID].NewAccount(name, passw, _playerID);
        }
        private static void UserLogin(int _playerID, byte[] _data)
        {
            string passw, name, result;
            ByteBuffer _buffer = new ByteBuffer();
            _buffer.WriteBytes(_data);
            _buffer.ReadInt();
            name = _buffer.ReadString();
            passw = _buffer.ReadString();
            _buffer.Dispose();
            Globals.clients[_playerID].LogToAccount(name, passw, _playerID);
        }
        private static void LeaveRoom(int _playerID, byte[] _data)
        {
            var currentRoom = Globals.clients[_playerID].currentRoom;
            Globals.GameRooms.Find(e => e.Name == currentRoom).LogOffPlayer(Globals.clients[_playerID].name);
            Globals.clients[_playerID].currentRoom = null;
            Logger.Log(LogType.info1, "Player " + Globals.clients[_playerID].name + " left the room.");
            Globals.clients[_playerID].isPlaying = false;
        }
        private static void UserLogout(int _playerID, byte[] _data)
        {
            Logger.Log(LogType.info1, "Player " + Globals.clients[_playerID].name + " has logged out.");

            Globals.clients[_playerID].LogOutAccount();           
        }
        private static void NewCharacter(int _playerID, byte[] _data)
        {
            ByteBuffer _buffer = new ByteBuffer();
            _buffer.WriteBytes(_data);
            _buffer.ReadInt();

            string characterData = _buffer.ReadString();
            Character tmp = XmlTool.SharpXMLStringToObject<Character>(characterData);
            Logger.Log(LogType.info1, "New Char attempt: \n" + characterData);
            _buffer.Dispose();
        }
        //refreshing room's info
        private static void RefreshRoomData(int _playerID, byte[] _data)
        {
            ByteBuffer _buffer = new ByteBuffer();
            _buffer.WriteBytes(_data);
            _buffer.ReadInt();
            string option = _buffer.ReadString();
            GameRoom gr = Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom);
            switch (option)
            {
                case "Characters":
                    ServerSend.getCharacterInfo(_playerID, gr);
                    break;
                case "Classes":
                    ServerSend.getRacesInfo(_playerID, gr);
                    break;
                case "Races":
                    ServerSend.getClassesInfo(_playerID, gr);
                    break;
                case "Stats":
                    break;
            }
        }

        //DM TOOLS
        private static void ChangeSettings(int _playerID, byte[] _data)
        {
            if(Security.securityDMCheck(_playerID) == false)
            {
                ServerSend.sendMessage(_playerID, "Don't cheat");
                return;
            }

            ByteBuffer _buffer = new ByteBuffer();
            _buffer.WriteBytes(_data);
            _buffer.ReadInt();
            string option = _buffer.ReadString();
            int settingtype = _buffer.ReadInt(); //0-string, 1-int
            string Ssetting = "";
            int Isetting = 0;
            if (settingtype == 0) Ssetting = _buffer.ReadString();
            if (settingtype == 1) Isetting = _buffer.ReadInt();
            string[] options = option.Split('.');
            GameRoom gameRoom;

            if(options[0] == "AttributeName")
            {
                if (StringValider.instance.isTextFine(Ssetting) == false)
                {
                    ServerSend.sendMessage(_playerID, "Name is wrong.");
                    return;
                }
                switch (options[1])
                {
                    case "1":
                        Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom).Stats.Stat1.Name = Ssetting;
                        Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom).SaveRoom();
                        ServerSend.sendMessage(_playerID, "Name updated.");
                        return;
                    case "2":
                        Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom).Stats.Stat2.Name = Ssetting;
                        Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom).SaveRoom();
                        ServerSend.sendMessage(_playerID, "Name updated.");

                        return;
                    case "3":
                        Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom).Stats.Stat3.Name = Ssetting;
                        Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom).SaveRoom();
                        ServerSend.sendMessage(_playerID, "Name updated.");
                        return;
                    case "4":
                        Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom).Stats.Stat4.Name = Ssetting;
                        Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom).SaveRoom();
                        ServerSend.sendMessage(_playerID, "Name updated.");
                        return;
                    case "5":
                        Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom).Stats.Stat5.Name = Ssetting;
                        Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom).SaveRoom();
                        ServerSend.sendMessage(_playerID, "Name updated.");
                        return;
                    case "6":
                        Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom).Stats.Stat6.Name = Ssetting;
                        Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom).SaveRoom();
                        ServerSend.sendMessage(_playerID, "Name updated.");
                        return;
                    case "7":
                        Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom).Stats.Stat7.Name = Ssetting;
                        Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom).SaveRoom();
                        ServerSend.sendMessage(_playerID, "Name updated.");
                        return;
                    case "8":
                        Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom).Stats.Stat8.Name = Ssetting;
                        Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom).SaveRoom();
                        ServerSend.sendMessage(_playerID, "Name updated.");
                        return;
                }

                return;
            }
            if(option == "MaxLevel")
            {
                if (Isetting>=1 && Isetting<=99999)
                {                   
                    Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom).LevelInfo.newMaxLvL(Isetting);
                    gameRoom = Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom);
                    gameRoom.SaveRoom();
                    ServerSend.sendMessage(_playerID, "Max level updated.");
                    ServerSend.getLevelInfo(_playerID, gameRoom);

                    //Updejtować istniejące postacie!!!
                }
                return;
            }
            if (options[0] == "NewGap")
            {
                try
                {                    
                    gameRoom = Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom);
                    int lvl = int.Parse(options[1]);
                    if (lvl < 1 || lvl > gameRoom.LevelInfo.MaxLvl)
                    {
                        ServerSend.sendMessage(_playerID,"error teapot2");
                        return;
                    }
                    gameRoom.LevelInfo.editGap(lvl, Isetting);
                    gameRoom.SaveRoom();
                    ServerSend.getLevelInfo(_playerID, gameRoom);

                    //Updejtować istniejące postacie!!!
                }
                catch (Exception e)
                {
                    Logger.Log(LogType.error, e.Message);
                    ServerSend.sendMessage(_playerID,"Something is wrong with the number");
                }
                return;
            }


                _buffer.Dispose();
        }
        private static void EditRace(int _playerID, byte[] _data)
        {
            try
            {
                string Name, Desc;
                int Min, Max;
                bool isRaceNew;
                ByteBuffer _buffer = new ByteBuffer();
                _buffer.WriteBytes(_data);
                _buffer.ReadInt();
                isRaceNew = _buffer.ReadBool();
                Name = _buffer.ReadString();
                Min = _buffer.ReadInt();
                Max = _buffer.ReadInt();
                Desc = _buffer.ReadString();

                List<string> Classes = new List<string>();
                int classesCount = _buffer.ReadInt();
                for(int i = 0; i < classesCount; i++)
                {
                    Classes.Add(_buffer.ReadString());
                }
                Dictionary<string, int> Equipment = new Dictionary<string, int>();
                int seqCount = _buffer.ReadInt();
                for (int i = 0; i < seqCount; i++)
                {
                    Equipment.Add(_buffer.ReadString(), _buffer.ReadInt());
                }
                _buffer.Dispose();


                CharacterRace race = new CharacterRace(Name, Min, Max, Desc, Classes, Equipment);
                if (isRaceNew == true)
                {
                    Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom).Races.Add(race);
                }
                else
                {
                    if (Desc == "ToDelete")
                    {
                        Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom).Races.RemoveAll(e => e.Name == Name);
                    }
                    else
                    {
                        Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom).Races.RemoveAll(e => e.Name == Name);
                        Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom).Races.Add(race);
                    }                   
                }
                Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom).SaveRoom();
                ServerSend.getRacesInfo(_playerID, Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom));
            }
            catch (Exception e)
            {
                Logger.Log(LogType.error, e.Message);
                ServerSend.sendMessage(_playerID, "error table5");
            }

        }


    }
}
