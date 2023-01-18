using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingRoom : RoomTemplate
{
    public StartingRoom(int _id, Coord _roomWorldPosition, int _cameraSize, bool[] _doors) :
        base(_id, _roomWorldPosition, _cameraSize, _doors)
    {
    }
    public override void GenerateRoom()
    {
        float randomX = Random.Range(0, 1000);
        float randomY = Random.Range(0, 1000);

        for (int i = 0; i < RoomSize.x; i++)
        {
            for (int j = 0; j < RoomSize.y; j++)
            {
                if (i == 0 || i == RoomSize.x - 1 || j == 0 || j == RoomSize.y - 1)
                {
                    RoomLayout[i, j] = new Block(InterfaceMapGen.BLOCK_TYPE_WALL);
                    continue;
                }
                RoomLayout[i, j] = new Block(InterfaceMapGen.BLOCK_TYPE_GROUND, true);
                WalkableTiles.Add(new Coord(i, j));
            }
        }
    }
}
