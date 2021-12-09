using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Server.GameLogic;
using Polenter.Serialization;

namespace Server
{
    [XmlRoot("GameRoom")]
    [XmlType("GameRoom")]
    internal class GameRoom
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public string Password { get; set; }
        public string Owner { get; set; }
        public string OwnerPassword { get; set; }
        public bool isRoomLocked { get; set; }

        public int PlayersOn { get; set; }

        public bool IsTurnedOn { get; set; }
        public bool IsProtected { get; set; }


        //                  Currently logged in
        [XmlArray("Players", Order = 1)]
        public List<string> Players { get; set; }

        [XmlArray("Items", Order = 2)]
        [XmlArrayItem("Equipment")]
        public List<Equipment> Items { get; set; }

        //                Player, his char
        [XmlElement("Characters")]
        public Dictionary<string, Character> Characters { get; set; }

        public List<CharacterClass> Classes { get; set; }

        public List<CharacterRace> Races { get; set; }

        [XmlElement("Stats")]
        public CharacterStats Stats { get; set; } //Like attributes, str, agi etc.
        [XmlElement("LevelInfo")]
        public Level LevelInfo { get; set; }
        [XmlElement("BaseData")]
        public Base BaseData { get; set; }
        //For new rooms
        public GameRoom()
        {
            Players = new List<string>();
            Items = new List<Equipment>();
            Classes = new List<CharacterClass>();
            Characters = new Dictionary<string, Character>();
            Races = new List<CharacterRace>();
            Desc = "== New room ==";
            isRoomLocked = false;
            LevelInfo = new Level(20);
            Stats = new CharacterStats();
            BaseData = new Base(BaseStatChosen.HP,20);
        }

        //For loading existing rooms
        public GameRoom(string _name)
        {
            Name = _name;
            Players = new List<string>();
            Stats = new CharacterStats();
            BaseData = new Base();
        }
        
        public void CreateRoom(int id, string name, bool isprotected, string password, string owner, string ownerpassword)
        {
            ID = id;
            Name = name;
            Password = password;
            Owner = owner;
            OwnerPassword = ownerpassword;
            IsProtected = isprotected;

            Equipment apple = new Equipment("Apple", "It's red.", EquipmentCategory.Food, WearableType.NotWearable, WeaponType.NotWeapon, 5, 0);
            Equipment gold = new Equipment("Gold", "You can buy stuff with these.", EquipmentCategory.Money, WearableType.NotWearable, WeaponType.NotWeapon, 1, 0);
            Equipment woodenSword = new Equipment("Wooden Sword", "Better than nothing.", EquipmentCategory.Wearable, WearableType.Weapon, WeaponType.Sword, 1, 0);
            Equipment cottonShirt = new Equipment("Cotton Shirt", "It's nice in touch.", EquipmentCategory.Money, WearableType.NotWearable, WeaponType.NotWeapon, 1, 0);
            Items.Add(apple);
            Items.Add(gold);
            Items.Add(woodenSword);
            Items.Add(cottonShirt);

            Dictionary<string, int> seq = new Dictionary<string, int>();
            seq.Add(apple.Name, 1);
            seq.Add(gold.Name, 10);

            Dictionary<string, int> seq2 = new Dictionary<string, int>();
            seq2.Add(woodenSword.Name, 1);
            seq2.Add(cottonShirt.Name, 1);

            CharacterClass newClass = new CharacterClass("Knight", seq2);
            CharacterClass newClass2 = new CharacterClass("Mage", seq2);
            CharacterClass newClass3 = new CharacterClass("Ranger", seq2);
            Classes.Add(newClass);
            Classes.Add(newClass2);
            Classes.Add(newClass3);

            List<CharacterClass> Classes2 = new List<CharacterClass>();
            Classes2.Add(newClass);
            Classes2.Add(newClass2);

            CharacterRace newRace = new CharacterRace("Human", Classes2, seq);
            CharacterRace newRace2 = new CharacterRace("Elf", Classes, seq);
            CharacterRace newRace3 = new CharacterRace("Orc", Classes, seq);
            Races.Add(newRace);
            Races.Add(newRace2);
            Races.Add(newRace3);

            Stats.Stat1.Name = "Strenght"; Stats.Stat2.Name = "Dexterity"; Stats.Stat3.Name = "Constitution"; Stats.Stat4.Name = "Inteligence";
            Stats.Stat5.Name = "Wisdom"; Stats.Stat6.Name = "Charisma"; Stats.Stat7.Name = "Null"; Stats.Stat8.Name = "Null";


            SaveRoom();


        }
        public void LogInPlayer(string name)
        {
            Players.Add(name);
            PlayersOn++;            
        }
        public void LogOffPlayer(string name)
        {
            Players.RemoveAll(e=>e == name);
            PlayersOn--;
        }
        public bool GetPlayersCharacter(string _nick, out Character _ch)
        {
            if (Characters.ContainsKey(_nick))
            {
                _ch = Characters[_nick];
                return true;
            }
            _ch = null;
            return false;
        }
        public bool GetPlayersCharacter(int _playerID, out Character _ch)
        {
            if (Characters.ContainsKey(Globals.clients[_playerID].name))
            {
                _ch = Characters[Globals.clients[_playerID].name];
                return true;
            }
            _ch = null;
            return false;
        }
        internal List<Equipment> GetItemsSave(string _name)
        {
            var xml = new SharpSerializer();
            return (List<Equipment>)xml.Deserialize(Globals.root + @"rooms\" + _name + @"\" + _name + "_Items.sav");
        }
        internal List<CharacterClass> GetClassesSave(string _name)
        {
            var xml = new SharpSerializer();
            return (List<CharacterClass>)xml.Deserialize(Globals.root + @"rooms\" + _name + @"\" + _name + "_Classes.sav");
        }
        internal Dictionary<string, Character> GetCharactersSave(string _name)
        {
            var xml = new SharpSerializer();
            return (Dictionary<string, Character>)xml.Deserialize(Globals.root + @"rooms\" + _name + @"\" + _name + "_Chars.sav");
        }
        internal Dictionary<Equipment, int> GetSItemsSave(string _name)
        {
            var xml = new SharpSerializer();
            return (Dictionary< Equipment, int >)xml.Deserialize(Globals.root + @"rooms\" + _name + @"\" + _name + "_SItems.sav");
        }
        public void SaveRoom()
        {
            
            GameRoom tmp = this;
            int oldPlayersOn = tmp.PlayersOn;
            List<string> tmpPlayers = tmp.Players;
            tmp.Players.Clear();
            tmp.PlayersOn = 0;
            if (!File.Exists(Globals.root + @"rooms\" + Name))
            {
                Directory.CreateDirectory(Globals.root + @"rooms\" + Name);
            }
            using (FileStream fs = File.Create(Globals.root + @"rooms\" + Name + @"\" + Name + "_ID.sav"))
            {
                XmlSerializer xml = new XmlSerializer(typeof(int));
                xml.Serialize(fs, ID);
                fs.Close();
            }
            var x = new SharpSerializer();
            x.Serialize(tmp, Globals.root + @"rooms\" + Name + @"\" + Name + ".sav");
            //x.Serialize(ID, Globals.root + @"rooms\" + Name + @"\" + Name + "_ID.sav");
            //x.Serialize(Characters, Globals.root + @"rooms\" + Name + @"\" + Name + "_Chars.sav");
            //x.Serialize(Classes, Globals.root + @"rooms\" + Name + @"\" + Name + "_Classes.sav");
            //x.Serialize(Items, Globals.root + @"rooms\" + Name + @"\" + Name + "_Items.sav");
            //x.Serialize(StarterItems, Globals.root + @"rooms\" + Name + @"\" + Name + "_sItems.sav");
            PlayersOn = oldPlayersOn;
            Players = tmpPlayers;
        }
        public bool getInfo(out string _desc, out string _owner, out int _playingNow, out bool _isProtected)
        {
            _desc = Desc;
            _owner = Owner;
            _playingNow = PlayersOn;
            if (Password == "") _isProtected = false;
            else _isProtected = true;

            return true;
        }
        internal void AddItem(Equipment _newItem)
        {
            if(!Items.Exists(e => e.Name == _newItem.Name))
            {
                Items.Add(_newItem);
                //Send item added
                return;
            }
            //item not added
            return;
        }
        public void RemoveItem(string _itemName)
        {
            Items.RemoveAll(e => e.Name == _itemName);
            //send item removed
        }
        public void LockRoom()
        {
            isRoomLocked = true;
        }
        public void UnlockRoom()
        {
            isRoomLocked = false;
        }
        public bool IsPlayerDM(int _id)
        {
            if (Owner == Globals.clients[_id].name)
            {
                return true;
            }
            return false;
        }

        public bool PasswordCheck(string _password)
        {
            if(_password == Password || Password == "" || Password == null)
            {
                return true;
            }
            return false;
        }



    }




}
