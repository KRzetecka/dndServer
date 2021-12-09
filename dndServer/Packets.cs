using System;
using System.Collections.Generic;

public enum ServerPackets
{
    // Sent from server to client
    welcome,
    roomlist,
    roomdesc,
    passwordCheck,
    gameInit,
    registerUser,
    registerError,
    loginError,
    leftRoom,
    message,

    //for room init
    RoomData,
    CharacterData,
    ClassData,
    RaceData,
    EquipmentData,
    StatsData,
    LevelData

}
public enum ClientPackets
{
    // Sent from client to server
    welcomeReceived,
    getRoomList,
    getRoomDesc,
    isPasswordCorrect,
    userRegister,
    userLogin,
    leaveRoom,
    userLogout,
    newCharacter,
    refreshRoomData,

    //DM's settings edit
    changeSettings,
    newMaxLevel,
    newLevelGap,
    newBaseHP,
    //race
    editRace
}
