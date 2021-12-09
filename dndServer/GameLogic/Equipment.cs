using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Server
{
    [Serializable]
    [XmlType(TypeName = "Equipment")]
    [XmlRoot("Equipment")]
    public class Equipment
    {
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("Category")]
        internal EquipmentCategory Category { get; set; }
        [XmlElement("WearableType")]
        internal WearableType WearableType { get; set; }
        [XmlElement("WeaponType")]
        internal WeaponType WeaponType { get; set; }
        [XmlElement("Damage")]
        public int Damage { get; set; }
        [XmlElement("Defence")]
        public int Defence { get; set; }
        [XmlElement("HPRecovery")]
        public int HPRecovery { get; set; }
        [XmlElement("ManaRecovery")]
        public int ManaRecovery { get; set; }
        [XmlElement("Condition")]
        public int Condition { get; set; }
        [XmlElement("BaseValue")]
        public int BaseValue { get; set; }
        [XmlElement("PictureID")]
        public int PictureID { get; set; }
        [XmlElement("Description")]
        public string Description { get; set; }

        public Equipment() {
        }
        internal Equipment(string _name,string _desc, EquipmentCategory _category, WearableType _type, WeaponType _type2, int _value, int _pic)
        {
            Name = _name;
            Category = _category;
            PictureID = _pic;
            Description = _desc;
            if(_category == EquipmentCategory.Money)
            {
                BaseValue = 1;
                WearableType = WearableType.NotWearable;
                WeaponType = WeaponType.NotWeapon;
            }
            else if(_category == EquipmentCategory.Wearable){
                WeaponType = _type2;
                WearableType = _type;
                BaseValue = _value;
            }
            else
            {
                BaseValue = _value;
                WeaponType = WeaponType.NotWeapon;
                WearableType = WearableType.NotWearable;
            }
        }
        public void SetStats(string stat, int value)
        {
            switch (stat)
            {
                case "Damage":
                    break;
            }
        }
    }

    enum EquipmentCategory
    {
        Wearable,
        Money,
        Food,
        Misc,
        Liquid,
        Ingredient,
        Herb,
        Loot
    }
    enum WearableType
    {
        NotWearable,
        Torso,
        Legs,
        Feets,
        Head,
        Ears,
        Makeup,
        Face,
        Weapon,
        Necklese,
        Ring,
    }
    enum WeaponType
    {
        NotWeapon,

        //General
        Malee,
        Ranged,
        Magic,
        //Malee
        Sword,
        Axe,
        Club,
        Spear,
        Shield,
        Hammer,
        Flexible, //na sznurkach, np. nunczako
        Gloves,
        //Ranged
        Bow,
        Quiver,
        Gun,
        Boomerang,
        ThrowableExplosive,
        Throwable,
        Crossbow,
        //Magic
        Wand,
        Elemental,
        Summoning,
        Psychic,
        //Other
        Explosive,
        MaritalArt,
        Instrument,

        Other,
    }



}
