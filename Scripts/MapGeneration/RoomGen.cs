using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGen
{
    public int roomType = 0;
    public int roomId = 0;

    public RoomGen(int _roomType, int roomId)
    {
        roomType = _roomType;
        this.roomId = roomId;
    }
}
