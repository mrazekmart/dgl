using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : RoomTemplate
{
    public BossRoom(int _id, Coord _roomWorldPosition, int _cameraSize, bool[] _doors, int _numberOfEnemies) :
        base(_id, _roomWorldPosition, _cameraSize, _doors)
    {
        NumberOfEnemies = _numberOfEnemies;
        EnemiesLeft = NumberOfEnemies;
        IsCleared = false;
        GenerateEnemies();
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
                RoomLayout[i, j] = new Block(InterfaceMapGen.BLOCK_TYPE_GROUND_BOSSROOM, true);
                WalkableTiles.Add(new Coord(i, j));
            }
        }
    }
    public override void GenerateEnemies()
    {
        List<Enemy> enemies = new List<Enemy>();
        for (int i = 0; i < NumberOfEnemies; i++)
        {
            int randomTile = Random.Range(0, WalkableTiles.Count);
            Enemy newEnemy = new Enemy(WalkableTiles[randomTile], 10);
            enemies.Add(newEnemy);
        }
        EnemiesInRoom = new EnemiesInRoom(enemies);
    }
}
