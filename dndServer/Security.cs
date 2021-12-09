using Server;
using System;
using System.Collections.Generic;
using System.Text;


public class Security
{
    public bool securityPlayerCheck(int _playerID)
    {
        GameRoom currentroom;
        try
        {
            currentroom = Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom);
        }
        catch
        {
            return false;
        }



        return true;
    }
    public static bool securityDMCheck(int _playerID)
    {
        GameRoom currentroom;
        try
        {
            currentroom = Globals.GameRooms.Find(e => e.Name == Globals.clients[_playerID].currentRoom);
        }
        catch
        {
            return false;
        }
        var nick = Globals.clients[_playerID].name;
        if (currentroom.Owner != nick) return false;
        return true;
    }




}

