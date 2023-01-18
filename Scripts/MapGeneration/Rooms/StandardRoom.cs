using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardRoom : RoomTemplate
{
    public StandardRoom(int _id, Coord _roomWorldPosition, int _cameraSize, bool[] _doors, int _numberOfEnemies) :
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

                float p = Mathf.PerlinNoise((i * randomX) * m_noiseScale, (j * randomY) * m_noiseScale);

                if (p < m_perlinNoise)
                {
                    RoomLayout[i, j] = new Block(InterfaceMapGen.BLOCK_TYPE_HOLE);
                    continue;
                }
                RoomLayout[i, j] = new Block(InterfaceMapGen.BLOCK_TYPE_GROUND, true);
                WalkableTiles.Add(new Coord(i, j));
            }
        }
    }

    public override void GenerateEnemies()
    {
        List<Enemy> enemies = new();
        for(int i = 0; i < NumberOfEnemies; i++)
        {
            int randomTile = Random.Range(0, WalkableTiles.Count);
            Enemy newEnemy = new Enemy(WalkableTiles[randomTile], 0);
            enemies.Add(newEnemy);
        }
        EnemiesInRoom = new EnemiesInRoom(enemies);      
    }
    public override void OnRoomExit()
    {
        if (!IsCleared)
        {
            EnemiesLeft = NumberOfEnemies;
        }
    }
    public override void OnEnemyKilled()
    {
        EnemiesLeft--;
        if (EnemiesLeft == 0) IsCleared = true;
    }
}
