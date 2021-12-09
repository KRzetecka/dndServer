using Server.GameLogic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Server
{
    [Serializable]
    [XmlType(TypeName = "Character")]
    class Character
    {
        [XmlElement]
        public string CharName { get; set; }
        [XmlElement]
        public string CharStory { get; set; }
        [XmlElement]
        public int CharAge { get; set; }
        [XmlElement]
        public CharacterSex CharSex {get;set;}
        [XmlElement]
        public int LvL { get; set; }
        [XmlElement]
        public int LvLPts { get; set; }
        [XmlElement]
        public int LvLGap { get; set; }
        [XmlElement]
        public int HP { get; set; }
        [XmlElement]
        public int DEF { get; set; }
        [XmlElement]
        public int ATT { get; set; }
        [XmlElement]
        public CharacterClass CharClass { get; set; }
        [XmlElement]
        public CharacterRace CharRace { get; set; }
        [XmlElement]
        public CharacterStats CharStats { get; set; }
        [XmlElement]
        public Dictionary<Equipment, int> CharEq { get; set; } //item + amount

        public Character()
        {

        }
        Character(string _charName, CharacterClass _class, Dictionary<Equipment, int> _starterEQ)
        {
            CharName = _charName;
            CharClass = _class;
            LvL = 0;
            CharEq = _starterEQ;
        }
        public void addItemToInventory(Equipment _item, int _amount)
        {
            if(_item.Category == EquipmentCategory.Wearable)
            {
                CharEq.Add(_item, 1);
            }
            if (CharEq.ContainsKey(_item))//czy item istnieje
            {
                int currentAmount;
                CharEq.TryGetValue(_item, out currentAmount);
                CharEq.Remove(_item);
                CharEq.Add(_item, _amount+currentAmount);
            }
            else
            {
                CharEq.Add(_item, _amount);
            }
        }
        public void removeItemFromInventory(Equipment _item, int _amount)
        {          
            if (CharEq.ContainsKey(_item))//czy item istnieje
            {
                if (_item.Category == EquipmentCategory.Wearable)//jeśli ubranie
                {
                    CharEq.Remove(_item);
                }
                else
                {
                    int currentAmount;
                    CharEq.TryGetValue(_item, out currentAmount);
                    if(currentAmount == 1)
                    {
                        CharEq.Remove(_item);
                    }
                    else if(currentAmount - _amount > 0)
                    {
                        CharEq.Remove(_item);
                        CharEq.Add(_item, currentAmount - _amount);
                    }else if (currentAmount - _amount <= 0)
                    {
                        CharEq.Remove(_item);
                    }
                }
            }
        }



    }
    enum CharacterSex
    {
        Male,
        Female,
        None,
        Other
    }

}
