using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomIsland
{
    public List<Coord> tiles;
    public List<Coord> edgeTiles;
    public List<RoomIsland> connectedRooms;
    public int roomSize;
    public bool isAccessibleFromMainRoom;
    public bool isMainRoom;

    public RoomIsland()
    {

    }
    public RoomIsland(List<Coord> _tiles, List<Coord> _edgeTiles)
    {
        tiles = _tiles;
        edgeTiles = _edgeTiles;
        roomSize = _tiles.Count;
        connectedRooms = new List<RoomIsland>();
    }

    public void SetAccessibleFromMainRoom()
    {
        if (!isAccessibleFromMainRoom)
        {
            isAccessibleFromMainRoom = true;
            foreach (RoomIsland room in connectedRooms)
            {
                room.SetAccessibleFromMainRoom();
            }
        }
    }
    public bool IsConnected(RoomIsland otherRoom)
    {
        return connectedRooms.Contains(otherRoom);
    }
    public static void ConnectRooms(RoomIsland roomA, RoomIsland roomB)
    {
        if (roomA.isAccessibleFromMainRoom)
        {
            roomB.SetAccessibleFromMainRoom();
        }
        else if (roomB.isAccessibleFromMainRoom)
        {
            roomA.SetAccessibleFromMainRoom();
        }
        roomA.connectedRooms.Add(roomB);
        roomB.connectedRooms.Add(roomA);
    }
}
