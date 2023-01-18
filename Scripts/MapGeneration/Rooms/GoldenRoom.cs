using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenRoom : RoomTemplate
{
    public GoldenRoom(int _id, Coord _roomWorldPosition, int _cameraSize, bool[] _doors) :
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
                WalkableTiles.Add(new Coord(i, j));

                if (i == RoomSize.x / 2 - 10 && j == RoomSize.y / 2 ||
                    i == RoomSize.x / 2 && j == RoomSize.y / 2 ||
                    i == RoomSize.x / 2 + 10 && j == RoomSize.y / 2)
                {
                    RoomLayout[i, j] = new Block(InterfaceMapGen.BLOCK_TYPE_GROUND_SPAWNITEM, true);
                    continue;
                }
                RoomLayout[i, j] = new Block(InterfaceMapGen.BLOCK_TYPE_GROUND, true);
            }
        }
    }

}
