using Server.GameLogic;
using Polenter.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Server
{
    class ServerSend
    {

        public static void SendDataTo(int _playerID, byte[] _data) {
            try {
                if (Globals.clients[_playerID].socket != null) {
                    ByteBuffer _buffer = new ByteBuffer();
                    _buffer.WriteInt(_data.GetUpperBound(0) - _data.GetLowerBound(0) + 1);
                    _buffer.WriteBytes(_data);

                    Globals.clients[_playerID].stream.BeginWrite(_buffer.ToArray(), 0, _buffer.ToArray().Length, null, null);
                    _buffer.Dispose();
                }
            }
            catch (Exception _ex) {
                int i = 0;
                Logger.Log(LogType.error, "Error sending data to player " + _playerID + ": " + _ex);
            }
        }

        public static void Welcome(int _sendToPlayer, string _msg)
        {
            ByteBuffer _buffer = new ByteBuffer();
            _buffer.WriteInt((int)ServerPackets.welcome);

            _buffer.WriteString(_msg);
            _buffer.WriteInt(_sendToPlayer);

            SendDataTo(_sendToPlayer, _buffer.ToArray());
            _buffer.Dispose();
        }

        public static void RoomList(int _sendToPlayer, string _msg)
        {
            ByteBuffer _buffer = new ByteBuffer();
            _buffer.WriteInt((int)ServerPackets.roomlist);

            _buffer.WriteString(_msg);
            _buffer.WriteInt(_sendToPlayer);

            SendDataTo(_sendToPlayer, _buffer.ToArray());
            _buffer.Dispose();
        }

        public static void RoomDesc(int _sendToPlayer, string _msg)
        {
            ByteBuffer _buffer = new ByteBuffer();
            _buffer.WriteInt((int)ServerPackets.roomdesc);

            _buffer.WriteString(_msg);
            _buffer.WriteInt(_sendToPlayer);

            SendDataTo(_sendToPlayer, _buffer.ToArray());
            _buffer.Dispose();
        }
        public static void PasswordCheckResult(int _sendToPlayer, string _msg)
        {
            ByteBuffer _buffer = new ByteBuffer();
            _buffer.WriteInt((int)ServerPackets.passwordCheck);

            _buffer.WriteString(_msg);
            _buffer.WriteInt(_sendToPlayer);

            SendDataTo(_sendToPlayer, _buffer.ToArray());
            _buffer.Dispose();
        }

        public static void GameInit(int _sendToPlayer, GameRoom _room, bool _isPlayerDM)
        {
            ByteBuffer _buffer;

            string s = "";

            //Room
            _buffer = new ByteBuffer();
            _buffer.WriteInt((int)ServerPackets.RoomData);

            if (_isPlayerDM == true) _buffer.WriteInt(1);
            else _buffer.WriteInt(0);
            if (_room.Characters.ContainsKey(Globals.clients[_sendToPlayer].name) == true) _buffer.WriteInt(1);
            else _buffer.WriteInt(0);

            SendDataTo(_sendToPlayer, _buffer.ToArray());
            _buffer.Dispose();
            //Characters
            getCharacterInfo(_sendToPlayer, _room);
            //Classes
            getClassesInfo(_sendToPlayer, _room);
            //Races
            getRacesInfo(_sendToPlayer, _room);
            //Equipment
            _buffer = new ByteBuffer();
            _buffer.WriteInt((int)ServerPackets.EquipmentData);
            int numOfEq = _room.Items.Count;
            _buffer.WriteInt(numOfEq);
            foreach (var item in _room.Items)
            {
                s = "";
                s = XmlTool.SerializeObjectToXML(item);
                _buffer.WriteString(s);
            }
            SendDataTo(_sendToPlayer, _buffer.ToArray());
            _buffer.Dispose();
            //Stats
            _buffer = new ByteBuffer();
            _buffer.WriteInt((int)ServerPackets.StatsData);

            _buffer.WriteBool(_room.Stats.statsRedist);
            _buffer.WriteInt(_room.Stats.statPoints);
            _buffer.WriteInt(_room.Stats.rollChances);

            _buffer = makeStatData(_room.Stats.Stat1, _buffer);
            _buffer = makeStatData(_room.Stats.Stat2, _buffer);
            _buffer = makeStatData(_room.Stats.Stat3, _buffer);
            _buffer = makeStatData(_room.Stats.Stat4, _buffer);
            _buffer = makeStatData(_room.Stats.Stat5, _buffer);
            _buffer = makeStatData(_room.Stats.Stat6, _buffer);
            _buffer = makeStatData(_room.Stats.Stat7, _buffer);
            _buffer = makeStatData(_room.Stats.Stat8, _buffer);

            SendDataTo(_sendToPlayer, _buffer.ToArray());
            _buffer.Dispose();
            //LevelGaps
            getLevelInfo(_sendToPlayer, _room);
        }
        private static ByteBuffer makeStatData(Stats s, ByteBuffer _buffer)
        {
            _buffer.WriteString(s.Base.ToString());
            SerializeBaseStats(s.Base1, _buffer);
            SerializeBaseStats(s.Base2, _buffer);
            _buffer.WriteString(s.Desc);
            _buffer.WriteString(s.Name);
            _buffer.WriteInt(s.Max);
            _buffer.WriteInt(s.Min);
            _buffer.WriteInt(s.Value);
            _buffer.WriteBool(s.TurnedOn);
            return _buffer;
        }
        private static ByteBuffer SerializeBaseStats(Base b, ByteBuffer _buffer)
        {
            _buffer.WriteInt(b.ATT.Count);
            if(b.ATT.Count != 0)
            {
                for (int i = 0; i <= b.ATT.Count; i++)
                {
                    _buffer.WriteInt(b.ATT[i]);
                }
            }
            _buffer.WriteInt(b.DEF.Count);
            if (b.DEF.Count != 0)
            {
                for (int i = 0; i <= b.DEF.Count; i++)
                {
                    _buffer.WriteInt(b.DEF[i]);
                }
            }
            _buffer.WriteInt(b.HP.Count);
            if (b.HP.Count != 0)
            {
                for (int i = 0; i <= b.HP.Count; i++)
                {
                    _buffer.WriteInt(b.HP[i]);
                }
            }
            return _buffer;
        }

        private static ByteBuffer getStatData(Stats s, ByteBuffer _buffer)
        {
            string sBase = _buffer.ReadString();
            if (sBase == "HP") s.Base = BaseStatChosen.HP;
            else if (sBase == "ATT") s.Base = BaseStatChosen.ATT;
            else if (sBase == "DEF") s.Base = BaseStatChosen.DEF;
            else s.Base = BaseStatChosen.NULL;
            DeserializeBaseStats(s.Base1, _buffer);
            DeserializeBaseStats(s.Base2, _buffer);
            s.Desc = _buffer.ReadString();
            s.Name = _buffer.ReadString();
            s.Max = _buffer.ReadInt();
            s.Min = _buffer.ReadInt();
            s.Value = _buffer.ReadInt();
            s.TurnedOn = _buffer.ReadBool();
            return _buffer;
        }
        private static ByteBuffer DeserializeBaseStats(Base b, ByteBuffer _buffer)
        {
            b.ATT = new Dictionary<int, int>();
            int Count = _buffer.ReadInt();
            for (int i = 0; i <= Count; i++)
            {
                b.ATT.Add(i, _buffer.ReadInt());
            }
            b.DEF = new Dictionary<int, int>();
            Count = _buffer.ReadInt();
            for (int i = 0; i <= Count; i++)
            {
                b.DEF.Add(i, _buffer.ReadInt());
            }
            b.HP = new Dictionary<int, int>();
            Count = _buffer.ReadInt();
            for (int i = 0; i <= Count; i++)
            {
                b.HP.Add(i, _buffer.ReadInt());
            }
            return _buffer;
        }




        public static void getLevelInfo(int _sendToPlayer, GameRoom _room)
        {
            ByteBuffer _buffer = new ByteBuffer();
            _buffer.WriteInt((int)ServerPackets.LevelData);
            int numOflvl = _room.LevelInfo.MaxLvl;
            _buffer.WriteInt(numOflvl);
            foreach (var item in _room.LevelInfo.LvlGaps.Values)
            {
                _buffer.WriteInt(item);
            }
            SendDataTo(_sendToPlayer, _buffer.ToArray());
            _buffer.Dispose();
        }
        public static void getCharacterInfo(int _sendToPlayer, GameRoom _room)
        {
            ByteBuffer _buffer = new ByteBuffer();
            _buffer.WriteInt((int)ServerPackets.CharacterData);
            int numOfCharacters = _room.Characters.Count;
            _buffer.WriteInt(numOfCharacters);
            string s;
            foreach (var character in _room.Characters.Values)
            {
                s = "";
                s = XmlTool.SerializeObjectToXML(character);
                _buffer.WriteString(s);
            }
            SendDataTo(_sendToPlayer, _buffer.ToArray());
            _buffer.Dispose();
        }
        public static void getRacesInfo(int _sendToPlayer, GameRoom _room)
        {
            ByteBuffer _buffer = new ByteBuffer();
            _buffer.WriteInt((int)ServerPackets.RaceData);
            int numOfRaces = _room.Races.Count;
            _buffer.WriteInt(numOfRaces);
            foreach (var race in _room.Races)
            {
                _buffer.WriteString(race.Name);
                _buffer.WriteInt(race.MaxAge);
                _buffer.WriteInt(race.MinAge);
                _buffer.WriteString(race.Description);

                _buffer.WriteInt(race.AllowedClasses.Count);
                foreach (var _class in race.AllowedClasses)
                {
                    _buffer.WriteString(_class);
                }
                _buffer.WriteInt(race.SEQ.Count);
                foreach (var _item in race.SEQ)
                {
                    _buffer.WriteString(_item.Key);
                    _buffer.WriteInt(_item.Value);
                }
            }
            SendDataTo(_sendToPlayer, _buffer.ToArray());
            _buffer.Dispose();
        }
        public static void getClassesInfo(int _sendToPlayer, GameRoom _room)
        {
            ByteBuffer _buffer = new ByteBuffer();
            _buffer.WriteInt((int)ServerPackets.ClassData);
            int numOfClasses = _room.Classes.Count;
            _buffer.WriteInt(numOfClasses);
            foreach (var roomClass in _room.Classes)
            {
                _buffer.WriteString(roomClass.ClassName);
                _buffer.WriteInt(roomClass.SEQ.Count);
                foreach (var item in roomClass.SEQ)
                {
                    _buffer.WriteString(item.Key);
                    _buffer.WriteInt(item.Value);
                }
            }
            SendDataTo(_sendToPlayer, _buffer.ToArray());
            _buffer.Dispose();
        }
        public static void getCharacterInfo()
        {

        }



        public static void RegisterError(int _sendToPlayer, string _msg)
        {
            ByteBuffer _buffer = new ByteBuffer();
            _buffer.WriteInt((int)ServerPackets.registerError);

            _buffer.WriteString(_msg);
            _buffer.WriteInt(_sendToPlayer);

            SendDataTo(_sendToPlayer, _buffer.ToArray());
            _buffer.Dispose();
        }
        public static void LoginMessage(int _sendToPlayer, string _msg, string _playerName)
        {
            ByteBuffer _buffer = new ByteBuffer();
            _buffer.WriteInt((int)ServerPackets.loginError);

            _buffer.WriteString(_msg);
            _buffer.WriteString(_playerName);
            _buffer.WriteInt(_sendToPlayer);
            
            SendDataTo(_sendToPlayer, _buffer.ToArray());
            _buffer.Dispose();
        }
        public static void sendMessage(int _sendToPlayer, string _m)
        {
            ByteBuffer _buffer = new ByteBuffer();
            _buffer.WriteInt((int)ServerPackets.message);
            _buffer.WriteString(_m);
            SendDataTo(_sendToPlayer, _buffer.ToArray());
            _buffer.Dispose();
        }



    }
}
