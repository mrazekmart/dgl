using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesInRoom { 
    private List<Enemy> m_enemies;
    public EnemiesInRoom(List<Enemy> _enemies)
    {
        Enemies = _enemies;
    }

    public List<Enemy> Enemies { get => m_enemies; set => m_enemies = value; }
}
