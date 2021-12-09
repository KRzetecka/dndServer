using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Linq;

namespace Server
{
    public class PlayersInfo
    {      
        public int count = 0;
        public void UpdatePlayersInfo()
        {
            PlayersInfo newPlayersInfo = new PlayersInfo();
            newPlayersInfo.count = Directory.GetDirectories(Globals.root + @"players").Length;
            XmlSerializer x;
            using (FileStream fs = File.OpenWrite(Globals.root + @"players\playersInfo.info"))
            {
                x = new XmlSerializer(newPlayersInfo.GetType());
                x.Serialize(fs, newPlayersInfo);
                fs.Close();
            }
            Logger.Log(LogType.info1, "Done.");
        }
    }
}
