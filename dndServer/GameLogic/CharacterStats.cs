using System;
using System.Collections.Generic;
using System.Text;

namespace Server.GameLogic
{
    public class CharacterStats
    {
        public CharacterStats()
        {
            statsRedist = true;
            statPoints = 20;
            rollChances = 0;
            Stat1 = new Stats();
            Stat2 = new Stats();
            Stat3 = new Stats();
            Stat4 = new Stats();
            Stat5 = new Stats();
            Stat6 = new Stats();
            Stat7 = new Stats();
            Stat8 = new Stats();
        }
        public Stats Stat1 { get; set; }
        public Stats Stat2 { get; set; }
        public Stats Stat3 { get; set; }
        public Stats Stat4 { get; set; }
        public Stats Stat5 { get; set; }
        public Stats Stat6 { get; set; }
        public Stats Stat7 { get; set; }
        public Stats Stat8 { get; set; }

        // false - Random (d20)
        // true - Points
        public bool statsRedist { get; set; }
        public int statPoints { get; set; }
        public int rollChances { get; set; } // how many times can player randomize his stats || 0 for unlimited
        public bool ChangeStatName(int _id, string _name)
        {
            if (_id > 0 && _id < 9)
            {
                if (_name != null || _name != "")
                {
                    switch (_id)
                    {
                        case 1:
                            Stat1.Name = _name;
                            break;
                        case 2:
                            Stat2.Name = _name;
                            break;
                        case 3:
                            Stat3.Name = _name;
                            break;
                        case 4:
                            Stat4.Name = _name;
                            break;
                        case 5:
                            Stat5.Name = _name;
                            break;
                        case 6:
                            Stat6.Name = _name;
                            break;
                        case 7:
                            Stat1.Name = _name;
                            break;
                        case 8:
                            Stat1.Name = _name;
                            break;
                    }
                    return true;
                }
            }
            return false;
        }
    }
    public class Stats
    {
        public string Name { get; set; }
        public string Desc { get; set; }
        public int Value { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public bool TurnedOn { get; set; }

        public BaseStatChosen Base;
        public Base Base1 { get; set; }
        public Base Base2 { get; set; }
        public Stats()
        {
            Name = "Needs Name";
            Desc = "";
            Min = 1;
            Max = 20;
            Value = 1;
            TurnedOn = true;
            Base = BaseStatChosen.NULL;
            Base1 = new Base(Base, Max);
            Base2 = new Base(Base, Max);
        }

    }
    public enum BaseStatChosen
    {
        HP,
        DEF,
        ATT,
        NULL
    }
}
