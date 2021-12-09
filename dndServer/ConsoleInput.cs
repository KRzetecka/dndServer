using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace Server
{
    
    class ConsoleInput
    {
        private static string root = Globals.root;
        private static bool exit = true;

        public static void getInput()
        {
            while (exit == true)
            {
                string input;
                input = Console.ReadLine();
                Commands(input);
            }          
        }

        private static void Commands(string input)
        {
            switch (input)
            {
                case "/help":
                    Console.WriteLine("Commands: /help, /root, /newroom, /removeroom, /roominit, /roomlist, /updateroomsinfo, /updateplayersinfo, /rickroll, /newplayer, /loggedplayerslist");
                    break;
                case "/removeplayer":
                    string nameb;
                    Console.WriteLine("Player name: ");
                    nameb = Console.ReadLine();
                    if (Directory.Exists(root + @"players\" + nameb))
                    {
                        ClearFolder(root + @"players\" + nameb);
                    }

                    break;

                case "/newroom":
                    try { 
                    Console.WriteLine("New room name: ");
                    string name = Console.ReadLine();
                        if (File.Exists(root + @"rooms\" + name) && File.Exists(root + @"rooms\" + name + @"\"+name+".sav"))
                        {
                            Console.WriteLine("Room already exists. Abort.");
                            break;
                        }
                        else
                        {
                            

                            Console.WriteLine("Set a password to the Room? (y/n): ");
                            string isPassword = Console.ReadLine();
                            bool isprotected = false;
                            string password = "";
                            if (isPassword == "y")
                            {
                                Console.WriteLine("Set a room password: ");
                                password = Console.ReadLine();
                                isprotected = true;
                            }
                            Console.WriteLine("Set Owner: ");
                            string owner = Console.ReadLine();
                            Console.WriteLine("Set Owners password: ");
                            string ownerPassword = Console.ReadLine();

                            //find free ID number
                            int id2 = 1;
                            RoomsInfo rInfo = new RoomsInfo();
                            XmlSerializer xml = new XmlSerializer(rInfo.GetType());             
                            if(File.Exists(root + @"\rooms\roomsInfo.info"))
                            {
                                using (FileStream fs = File.OpenRead(root + @"\rooms\roomsInfo.info"))
                                {
                                    bool exit = false;
                                    rInfo = (RoomsInfo)xml.Deserialize(fs);
                                    if (rInfo.count == 0)
                                    {
                                        fs.Close();
                                        exit = true;
                                    }
                                    while (exit == false)
                                    {

                                        foreach (int number in rInfo.IDs)
                                        {

                                            if (id2 != number)
                                            {

                                                exit = true;
                                            }
                                            else
                                            {
                                                ++id2;
                                            }
                                        }
                                        fs.Close();
                                    }
                                }
                            }
                            if (!Directory.Exists(root + @"rooms\" + name))
                            {
                                Directory.CreateDirectory(root + @"rooms\" + name);
                            }
                            GameRoom newGameRoom = new GameRoom();
                            newGameRoom.CreateRoom(id2, name, isprotected, password, owner, ownerPassword);
                            //newGameRoom.SaveRoom();
                            Logger.Log(LogType.info1, "Room is done.");
                            
                            UpdateRoomsInfo();
                        }
                    }
                    catch (ArgumentException e)
                    {
                        Logger.Log(LogType.error, "Cannot create new room or update roomsInfo: \n" + e.Message + "\n Aborting!");
                    }
                    break;

                case "/roominit":
                    Console.WriteLine("Room name:");
                    string RoomNameToInit = Console.ReadLine();
                    RoomsHandle.initRoom(RoomNameToInit);
                    break;

                case "/killroom":
                    Console.WriteLine("Room name:");
                    string RoomNameToKill = Console.ReadLine();
                    RoomsHandle.killRoom(RoomNameToKill);
                    Logger.Log(LogType.info1, "Room killed.");
                    break;

                case "/root":
                    Console.WriteLine(root);
                    break;

                case "/removeroom":
                    Console.WriteLine("Which one: ");
                    try
                    {
                        string Name = Console.ReadLine();
                        if(Directory.Exists(root + @"rooms\" + Name))
                        {
                            File.Delete(root + @"rooms\" + Name + @"\" + Name + ".sav");
                            File.Delete(root + @"rooms\" + Name + @"\" + Name + "_ID.sav");
                            Directory.Delete(root + @"rooms\" + Name);
                            Logger.Log(LogType.info1, Name + " removed.");

                            UpdateRoomsInfo();
                        }
                        else
                        {
                            Logger.Log(LogType.info1, "Room with this name does not exist.");
                        }

                    }
                    catch (ArgumentException e)
                    {
                        Logger.Log(LogType.error, "Cannot remove room or update roomsInfo: \n" + e.Message + "\n Aborting!");
                    }
                    break;

                case "/roomlist":
                    string names = "";
                    foreach (string name in Directory.EnumerateDirectories(Globals.root + @"rooms", "*", SearchOption.AllDirectories).Select(f => Path.GetFileName(f)))
                    {
                        names += name + ", ";
                    }
                    Console.WriteLine(names);
                    break;

                case "/updateroomsinfo":
                    UpdateRoomsInfo();   
                    break;
                case "/updateplayersinfo":
                    Globals.instancePlayersInfo.UpdatePlayersInfo();
                    break;
                case "/loggedplayerslist":
                    GetLoggedPlayersList();
                    break;
                default:
                    Console.WriteLine("For commands write /help.");
                    break;
            }
        }

        private static void GetLoggedPlayersList()
        {
            string list = "";
            foreach(var player in Globals.clients)
            {
                list += player.Value.name + ", ";
            }
        }
        public static void UpdateRoomsInfo()
        {
            try
            {
                int ID;
                XmlSerializer x;
                RoomsInfo NewRoomsInfo = new RoomsInfo();
                //get all IDs
                foreach (string name in Directory.EnumerateDirectories(Globals.root + @"rooms", "*", SearchOption.AllDirectories).Select(f => Path.GetFileName(f)))
                { 
                    using (FileStream fs = File.OpenRead(Globals.root + @"rooms\" + name + @"\" + name + "_ID.sav"))
                    {
                        x = new XmlSerializer(typeof(int));
                        ID = (int)x.Deserialize(fs);
                        fs.Close();
                    }
                    NewRoomsInfo.IDs.Add(ID);
                    NewRoomsInfo.count = Directory.GetDirectories(Globals.root + @"rooms").Length;
                    
                }
                //update file
                if(NewRoomsInfo.IDs.Count > 1)
                {
                    NewRoomsInfo.IDs.Sort();
                }
                File.WriteAllText(Globals.root + @"rooms\roomsInfo.info", string.Empty);
                using (FileStream fs = File.OpenWrite(Globals.root + @"rooms\roomsInfo.info"))
                {
                    x = new XmlSerializer(NewRoomsInfo.GetType());
                    x.Serialize(fs, NewRoomsInfo);
                    fs.Close();
                }
            }
            catch(ArgumentException e)
            {
                Logger.Log(LogType.error, "Cannot update roomsInfo: \n" + e.Message);
            }
            Logger.Log(LogType.info1, "RoomsInfo updated.");
        }
        public static void ClearFolder(string dir)
        {
            foreach (string name in Directory.EnumerateDirectories(dir, "*", SearchOption.AllDirectories))
            {
                File.Delete(name);
            }
        }
    }
}
