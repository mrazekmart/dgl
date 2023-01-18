using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemiesInRoom : MonoBehaviour
{
    [SerializeField] private GameObject m_enemy;
    [SerializeField] private GameObject m_enemyBoss;



    public void SpawnEnemies(RoomTemplate currentRoom)
    {
        List<GameObject> listOfEnemiesSpawned = new();

        int x = currentRoom.RoomWorldPosition.x * 40;
        int y = currentRoom.RoomWorldPosition.y * 20;

        foreach (Enemy enemy in currentRoom.EnemiesInRoom.Enemies)
        {

            GameObject newEnemy = InstantiateEnemy(enemy.EnemyType);

            newEnemy.transform.position = new Vector3(x + enemy.EnemyCoords.x, y + enemy.EnemyCoords.y, 0);
            newEnemy.SetActive(true);
            listOfEnemiesSpawned.Add(newEnemy);
        }
        currentRoom.EnemiesInRoomGameObjects = listOfEnemiesSpawned;
    }
    public void DespawnEnemies(List<GameObject> enemies)
    {
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }
    public GameObject InstantiateEnemy(int enemyType)
    {
        GameObject enemy;
        switch (enemyType)
        {
            case 0:
                enemy = Instantiate(m_enemy);
                break;
            case 10:
                enemy = Instantiate(m_enemyBoss);
                break;
            default:
                enemy = Instantiate(m_enemy);
                break;
        }
        return enemy;
    }
}
