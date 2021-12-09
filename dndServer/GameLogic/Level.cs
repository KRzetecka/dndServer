using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Server.GameLogic
{
    public class Level
    {
        [XmlElement("MaxLvl")]
        public int MaxLvl { get; set; }

        [XmlElement("LvlGaps")]
        public Dictionary<int, int> LvlGaps { get; set; }  // id|value
        public Level()
        {

        }
        public Level(int _maxLvl)
        {
            MaxLvl = _maxLvl;
            LvlGaps = new Dictionary<int, int>(_maxLvl+1); //+1 to miejsce [0] nieużywane

            int xp = 500;
            LvlGaps[0] = 0;
            for (int i = 1; i<= _maxLvl; i++)
            {
                LvlGaps[i] = xp;
                xp += 500;
            }
            
        }
        public void editGap(int lvl, int value)
        {
            if(value > 0)
            {
                LvlGaps[lvl] = value;
            }
            else
            {
                //send error message Cannot be less than 1;
            }
        }
        public void newMaxLvL(int _lvl)
        {
            int oldMax = MaxLvl;
            MaxLvl = _lvl;

            if (MaxLvl > oldMax) //dodajemy nowe gapy
            {
                int newValue = 0;
                int i = oldMax + 1;
                for(; i <= MaxLvl+1; i++)
                {
                    newValue += 500;
                    editGap(i, newValue);
                }
            }
            else if (MaxLvl < oldMax) //usuwamy nadmiarowe gapy
            {
                int i = MaxLvl + 1;
                for (; i <= oldMax+1; i++)
                {
                    LvlGaps.Remove(i);
                }
            }
        }
    }
}
