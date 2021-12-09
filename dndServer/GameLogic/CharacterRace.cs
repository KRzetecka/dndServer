using Server;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

[Serializable]
public class CharacterRace
{
    [XmlElement]
    public string Name { get; set; }
    [XmlElement]
    public int MaxAge { get; set; }
    [XmlElement]
    public int MinAge { get; set; }
    [XmlElement]
    public string Description { get; set; }
    [XmlArray]
    public List<string> AllowedClasses {get; set;}
    [XmlArray]
    public Dictionary<string, int> SEQ { get; set; } //Starter Equipment
    public CharacterRace()
    {
        AllowedClasses = new List<string>();
        SEQ = new Dictionary<string, int>();
    }

    public CharacterRace(string _name, List<CharacterClass> _classes, Dictionary<string, int> _seq)
    {
        Name = _name;
        Description = "New Race";
        MinAge = 18;
        MaxAge = 100;
        Description = "Put here story of this race.";
        AllowedClasses = new List<string>();
        GetAllClasses(_classes);
        SEQ = _seq;
    }
    public CharacterRace(string _name, int _min, int _max, string _desc, List<string> _aClasses, Dictionary<string, int> _seq)
    {
        Name = _name;
        Description = _desc;
        MinAge = _min;
        MaxAge = _max;
        Description = _desc;
        AllowedClasses = new List<string>();
        AllowedClasses = _aClasses;
        SEQ = new Dictionary<string, int>();
        SEQ = _seq;
    }
    private void GetAllClasses(List<CharacterClass> _classes)
    {
        foreach(var _class in _classes)
        {
            AllowedClasses.Add(_class.ClassName);
        }
    }
}

