using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;



namespace Server
{
    class General
    {
        public static void StartServer()
        {
            InitServerData();
            ServerTCP.InitNetwork();
            Logger.Log(LogType.info2, "Server started");

           
        }
        private static void InitServerData()
        {
            for (int i = 1; i <= Constants.MAX_PLAYERS; i++)
            {
                Globals.clients.Add(i, new Client());
            }

            if (!Directory.Exists(Globals.root + @"rooms"))
            {
                Directory.CreateDirectory(Globals.root + @"rooms");
            }
            if (!File.Exists(Globals.root + @"rooms\roomsInfo.info"))
            {
                ConsoleInput.UpdateRoomsInfo(); 
            }
            if (!Directory.Exists(Globals.root + @"players"))
            {
                Directory.CreateDirectory(Globals.root + @"players");
            }
            if (!File.Exists(Globals.root + @"rooms\playersInfo.info"))
            {
                PlayersInfo tmp = new PlayersInfo();
                tmp.UpdatePlayersInfo();
            }

            RoomsHandle.instance = new RoomsHandle();
            RoomsHandle.initAllRooms();

        }
    }
}
