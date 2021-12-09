using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class Globals
    {
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public static bool serverIsRunning = false;
        public static string root = AppDomain.CurrentDomain.BaseDirectory;

        public static List<GameRoom> GameRooms = new List<GameRoom>();
        //public static List<Player> Players = new List<Player>();
        public static PlayersInfo instancePlayersInfo = new PlayersInfo();

    }
}
