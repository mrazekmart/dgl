using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    private Coord m_enemyCoord;
    private int m_enemyType;
    public Enemy(Coord _enemyCoords, int _enemyType)
    {
        EnemyCoords = _enemyCoords;
        EnemyType = _enemyType;
    }

    public Coord EnemyCoords { get => m_enemyCoord; set => m_enemyCoord = value; }
    public int EnemyType { get => m_enemyType; set => m_enemyType = value; }
}