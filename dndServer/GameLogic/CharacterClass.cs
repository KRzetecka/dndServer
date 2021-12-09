using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;


[Serializable]
public class CharacterClass
{
    [XmlAttribute("ClassName", DataType = "string")]
    public string ClassName { get; set; }
    [XmlArray]
    public Dictionary<string, int> SEQ { get; set; } //item+amount



    public CharacterClass()
    {
        SEQ = new Dictionary<string, int>();
    }
    public CharacterClass(string _name, Dictionary<string, int> _seq)
    {
        SEQ = _seq;
        ClassName = _name;
    }
}

