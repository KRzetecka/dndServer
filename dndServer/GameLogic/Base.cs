using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Server.GameLogic
{
    [Serializable]
    [XmlType(TypeName = "Base")]
    public class Base
    {
        //                 lvl, base
        [XmlArray]   
        public Dictionary <int, int> HP { get; set; }
        [XmlArray]
        public Dictionary <int, int> DEF { get; set; }
        [XmlArray]
        public Dictionary <int, int> ATT { get; set; }

        public Base()
        {
            HP = new Dictionary<int, int>();
            DEF = new Dictionary<int, int>();
            ATT = new Dictionary<int, int>();
        }
        public Base(BaseStatChosen _base , int _maxlvl) //for new rooms
        {
            int _baseValue = 10;
            if (_base == BaseStatChosen.HP)
            {
                HP = new Dictionary<int, int>();
                DEF = new Dictionary<int, int>();
                ATT = new Dictionary<int, int>();
                HP.Add(0, 0);
                for (int i = 1; i <= _maxlvl; i++)
                {
                    HP.Add(i, _baseValue);
                    _base += 10;
                }
            }
            else if (_base == BaseStatChosen.DEF)
            {
                DEF = new Dictionary<int, int>();
                ATT = new Dictionary<int, int>();
                HP = new Dictionary<int, int>();
                DEF.Add(0, 0);
                for (int i = 1; i <= _maxlvl; i++)
                {
                    DEF.Add(i, _baseValue);
                    _base += 10;
                }
            }
            else if(_base == BaseStatChosen.ATT)
            {
                ATT = new Dictionary<int, int>();
                DEF = new Dictionary<int, int>();
                HP = new Dictionary<int, int>();
                ATT.Add(0, 0);
                for (int i = 1; i <= _maxlvl; i++)
                {
                    ATT.Add(i, _baseValue);
                    _base += 10;
                }
            }
            else
            {

            }
        }


    }
}
