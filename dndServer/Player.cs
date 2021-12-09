using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Server
{
    class Player
    {
        public int ID;
        public string name;
        public string roomName = null;
        public string passw;

        public Dictionary<GameRoom, Character> Characters;

        public Player()
        {

        }

        public Player(string _name, string _passw)
        {
            name = _name;
            passw = _passw;
        }
        public Player(int _ID, string _name, string _passw)
        {
            ID = _ID;
            name = _name;
            passw = _passw;
        }

        public Player(int _ID, string _name, bool _logged)
        {
            ID = _ID;
            name = _name;
        }


        public void addChar(GameRoom _room, Character _char)
        {
            Characters.Add(_room, _char);
        }

        public void registerPlayer(int _playerID)
        {
            try
            {
                if (Directory.Exists(Globals.root + @"players\" + name) || File.Exists(Globals.root + @"players\" + name + @"\" + name + ".sav"))
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
                Directory.CreateDirectory(Globals.root + @"players\" + name);

                Player newPlayer = new Player(name, passw);
                newPlayer.savePlayer();

                Logger.Log(LogType.info1, "New Player registered: " + name);
                
                Globals.instancePlayersInfo.UpdatePlayersInfo();

            }
            catch (Exception e)
            {
                Logger.Log(LogType.error, e.Message);
            }

        }
        public void savePlayer()
        {
            using (FileStream fs = File.Create(Globals.root + @"players\" + name + @"\" + name + ".sav"))
            {
                XmlSerializer x = new XmlSerializer(typeof(Player));
                x.Serialize(fs, this);
                fs.Close();
            }
        }


    }
}
