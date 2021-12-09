using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Polenter.Serialization;
using System.Linq;

namespace Server
{
    class RoomsHandle
    {
        public static RoomsHandle instance;
        static string fileResult;


        public RoomsHandle()
        {
            Logger.Log(LogType.info1, "Rooms Handler is ready");
        }

        static public bool isRoomOn(string _name)
        {
            if (Globals.GameRooms.Exists(e => e.Name == _name))
            {
                return true;
            }
            else return false;
        }
        static internal string getInitiatedRoomsList()
        {
            string rooms = "";
            foreach(var room in Globals.GameRooms)
            {
                rooms += room.Name + ", ";
            }
            return rooms;
        }
        static internal void initRoom(string _name)
        {
            if(!Globals.GameRooms.Exists(x=>x.Name == _name))
            {
                string directory = Globals.root + @"rooms\" + _name;
                if (Directory.Exists(directory))
                {
                    GameRoom room = new GameRoom(_name);
                    var x = new SharpSerializer();
                    room = (GameRoom)x.Deserialize(Globals.root + @"rooms\" + _name + @"\" + _name + ".sav");

                    if(room.isRoomLocked == false)
                    {
                        Globals.GameRooms.Add(room);
                        Logger.Log(LogType.info1, "GameRoom '" + _name + "' is ready.");
                    }
                    else
                    {
                        Logger.Log(LogType.info1, "GameRoom '" + _name + "' is locked.");
                    }
                }
                else
                {
                    Logger.Log(LogType.info1, "GameRoom like this does not exist in files");
                }
            }
            else
            {
                Logger.Log(LogType.info1, "this Game Room ("+_name+") is already initialised");
            }
        }
        static internal void initAllRooms()
        {
            foreach (string name in Directory.EnumerateDirectories(Globals.root + @"rooms", "*", SearchOption.AllDirectories).Select(f => Path.GetFileName(f)))
            {
                initRoom(name);
            }
        }
        static internal void killRoom(string _name)
        {
            Globals.GameRooms.Find(e => e.Name == _name).SaveRoom();
            Globals.GameRooms.RemoveAll(e => e.Name == _name);
            Logger.Log(LogType.info1, _name + " room goes down");         
        }

        /*
        static internal void addPlayer(string roomName, int playerID)
        {
            int id = getRoomID(roomName);
            Player tmp = Globals.Players.Find(x => x.ID == playerID);
            if(tmp == null)
            {
                return;
            }
            if(Globals.GameRooms[id] == null)
            {
                initRoom(roomName);
            }
            Globals.GameRooms[id].LogInPlayer(tmp.name);     
        }
        static public void removePlayer(string roomName, int playerID)
        {
            int id = getRoomID(roomName);
            Player tmp = Globals.Players.Find(x => x.ID == playerID);
            Globals.GameRooms[id].LogOffPlayer(tmp.name);
        }
        static public void removePlayer(int playerID)
        {          
            Player tmp = Globals.Players.Find(x => x.ID == playerID);
            int roomID = getRoomID(tmp.roomName);
            Globals.GameRooms[roomID].LogOffPlayer(tmp.name);
        }*/
        static private int getRoomID(string name)
        {
            using (StreamReader sr = new StreamReader(Globals.root + @"rooms\" + name + @"\" + name + ".sav", Encoding.UTF8))
            {
                fileResult = null;
                fileResult = sr.ReadToEnd();
                GameRoom tmp = new GameRoom();
                tmp = XmlTool.DeserializeXmlToObject<GameRoom>(fileResult);
                return tmp.ID;
            }
        }
        static public void LockRoom(string _name)
        {
            if (isRoomOn(_name))
            {
                Globals.GameRooms.Find(e => e.Name == _name).LockRoom();
                killRoom(_name);
            }
        }
        static public void UnlockRoom(string _name)
        {
            if (isRoomOn(_name))
            {
                Globals.GameRooms.Find(e => e.Name == _name).UnlockRoom();
                initRoom(_name);
            }
        }
        static public string GetDesc(string _name)
        {
            if (Globals.GameRooms.Exists(e => e.Name == _name))
            {
                GameRoom tmp = Globals.GameRooms.Find(e => e.Name == _name);
                tmp.getInfo(out string _desc, out string _owner, out int _playingNow, out bool _isProtected);
                string Description = "Game Room's name: " + _name + "\nDescription: " + _desc + "\nActive players: " + _playingNow;
                if (_isProtected == true) Description += "\nThe room is password protected!";
                return Description;
            }
            return "Room is (probably) locked.";
        }
        static private GameRoom getRoomObject(string name)
        {
            using (StreamReader sr = new StreamReader(Globals.root + @"rooms\" + name + @"\" + name + ".sav", Encoding.UTF8))
            {
                fileResult = null;
                fileResult = sr.ReadToEnd();
                GameRoom tmp = new GameRoom();
                tmp = XmlTool.DeserializeXmlToObject<GameRoom>(fileResult);
                return tmp;
            }
        }

    }
}
